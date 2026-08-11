using AlmacenDesktop.Data;
using AlmacenDesktop.Helpers;
using AlmacenDesktop.Modelos;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AlmacenDesktop.Services
{
    /// <summary>
    /// Catálogo compartido de productos entre instalaciones de Vendemax Desktop.
    /// Solo nombre + código de barras — nunca costo/precio/stock/proveedor, eso es
    /// información comercial de cada comercio, no del producto. Ambas operaciones
    /// son fire-and-forget/best-effort: sin internet, o si el servidor falla, no
    /// deben interrumpir el flujo normal de carga de productos.
    /// </summary>
    public class CatalogoCompartidoService
    {
        private static readonly HttpClient _http = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
        private const string ArchivoUltimoSync = "catalogo_ultimo_sync.txt";
        private const string ProveedorCatalogoNombre = "PROVEEDOR GENERAL";

        /// <summary>Busca un producto por código de barras en el catálogo compartido. Null si no está o falla.</summary>
        public async Task<string?> BuscarPorCodigoAsync(string codigoBarras)
        {
            if (string.IsNullOrWhiteSpace(codigoBarras)) return null;

            try
            {
                string url = $"{Constantes.API_LICENCIAS_URL}/catalogo/buscar?codigo={Uri.EscapeDataString(codigoBarras)}";
                using var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return null;

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("nombre", out var nombreProp))
                {
                    return nombreProp.GetString();
                }
                return null;
            }
            catch
            {
                // Sin conexión, servidor caído, etc. — no debe interrumpir la carga del producto.
                return null;
            }
        }

        /// <summary>Sube (en un solo lote) los productos nuevos guardados a la base compartida. Nunca tira excepción.</summary>
        public async Task SubirAsync(IEnumerable<(string CodigoBarras, string Nombre)> productos)
        {
            try
            {
                var lista = new List<object>();
                foreach (var p in productos)
                {
                    if (string.IsNullOrWhiteSpace(p.CodigoBarras) || string.IsNullOrWhiteSpace(p.Nombre)) continue;
                    lista.Add(new { codigoBarras = p.CodigoBarras, nombre = p.Nombre });
                }
                if (lista.Count == 0) return;

                string url = $"{Constantes.API_LICENCIAS_URL}/catalogo/subir";
                string json = JsonSerializer.Serialize(new { productos = lista });
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
                request.Headers.Add("X-Catalog-Key", Constantes.CATALOG_UPLOAD_SECRET);

                await _http.SendAsync(request);
            }
            catch
            {
                // Best-effort — nunca debe afectar al usuario que está guardando un producto.
            }
        }

        /// <summary>
        /// Sync incremental en segundo plano: trae del catálogo compartido los productos
        /// nuevos desde la última vez y los guarda localmente con stock/precio en 0 (solo
        /// nombre + código de barras — el dueño confirma precio/stock reales cuando
        /// efectivamente los empieza a vender). Se llama en cada arranque de la app.
        /// Nunca tira excepción — sin internet o con el servidor caído, no hace nada.
        /// </summary>
        public async Task SincronizarCatalogoLocalAsync()
        {
            try
            {
                DateTime desde = LeerUltimoSync();
                var (productos, hasta) = await ObtenerNuevosAsync(desde);
                if (productos.Count == 0)
                {
                    GuardarUltimoSync(hasta);
                    return;
                }

                using (var context = new AlmacenDbContext())
                {
                    var proveedor = context.Proveedores.FirstOrDefault(p => p.Nombre == ProveedorCatalogoNombre);
                    if (proveedor == null)
                    {
                        proveedor = new Proveedor
                        {
                            Nombre = ProveedorCatalogoNombre,
                            Cuit = "30-00000000-0",
                            Direccion = "-",
                            Telefono = "-",
                            Contacto = "-",
                        };
                        context.Proveedores.Add(proveedor);
                        context.SaveChanges();
                    }

                    foreach (var (codigo, nombre) in productos)
                    {
                        bool yaExiste = context.Productos.Any(p => p.CodigoBarras == codigo);
                        if (yaExiste) continue;

                        context.Productos.Add(new Producto
                        {
                            CodigoBarras = codigo,
                            Nombre = nombre,
                            Descripcion = "",
                            Costo = 0,
                            Precio = 0,
                            Stock = 0,
                            StockMinimo = 0,
                            Impuesto = 0,
                            Activo = true,
                            ProveedorId = proveedor.Id,
                        });
                    }
                    context.SaveChanges();
                }

                GuardarUltimoSync(hasta);
            }
            catch
            {
                // Sin conexión, servidor caído, etc. — nunca debe trabar el arranque de la app.
            }
        }

        private async Task<(List<(string Codigo, string Nombre)> Productos, DateTime Hasta)> ObtenerNuevosAsync(DateTime desde)
        {
            try
            {
                string url = $"{Constantes.API_LICENCIAS_URL}/catalogo/todos?desde={Uri.EscapeDataString(desde.ToString("o", CultureInfo.InvariantCulture))}";
                using var response = await _http.GetAsync(url);
                if (!response.IsSuccessStatusCode) return (new List<(string, string)>(), desde);

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var lista = new List<(string, string)>();
                if (root.TryGetProperty("productos", out var arr))
                {
                    foreach (var item in arr.EnumerateArray())
                    {
                        string codigo = item.TryGetProperty("codigoBarras", out var c) ? c.GetString() ?? "" : "";
                        string nombre = item.TryGetProperty("nombre", out var n) ? n.GetString() ?? "" : "";
                        if (!string.IsNullOrEmpty(codigo) && !string.IsNullOrEmpty(nombre))
                        {
                            lista.Add((codigo, nombre));
                        }
                    }
                }

                DateTime hasta = desde;
                if (root.TryGetProperty("hasta", out var hastaProp) &&
                    DateTime.TryParse(hastaProp.GetString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var h))
                {
                    hasta = h;
                }

                return (lista, hasta);
            }
            catch
            {
                return (new List<(string, string)>(), desde);
            }
        }

        private DateTime LeerUltimoSync()
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ArchivoUltimoSync);
                if (File.Exists(path))
                {
                    string texto = File.ReadAllText(path).Trim();
                    if (DateTime.TryParse(texto, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var fecha))
                    {
                        return fecha;
                    }
                }
            }
            catch
            {
                // Si el archivo está corrupto o no se puede leer, arrancamos de cero.
            }
            return DateTime.MinValue;
        }

        private void GuardarUltimoSync(DateTime fecha)
        {
            try
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ArchivoUltimoSync);
                File.WriteAllText(path, fecha.ToString("o", CultureInfo.InvariantCulture));
            }
            catch
            {
                // Si no se puede escribir, el próximo arranque vuelve a intentar desde el mismo punto.
            }
        }
    }
}

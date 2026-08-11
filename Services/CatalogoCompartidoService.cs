using AlmacenDesktop.Helpers;
using System;
using System.Collections.Generic;
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
    }
}

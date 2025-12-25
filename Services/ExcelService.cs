using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AlmacenDesktop.Services
{
    public class ResultadoImportacion
    {
        public int Procesados { get; set; }
        public int Nuevos { get; set; }
        public int Actualizados { get; set; }
        public int Errores { get; set; }
        public List<string> MensajesError { get; set; } = new List<string>();
    }

    public class ExcelService
    {
        // Palabras clave para Heurística (Detectar columnas automáticamente)
        private readonly string[] _keysCodigo = { "codigo", "código", "barra", "code", "barcode", "id", "sku" };
        private readonly string[] _keysNombre = { "nombre", "descripción", "descripcion", "producto", "articulo", "detalle" };
        private readonly string[] _keysPrecio = { "precio", "venta", "pvp", "importe", "monto", "salida" };
        private readonly string[] _keysCosto = { "costo", "compra", "pc" }; // Quitamos "proveedor" de aquí para evitar conflictos
        private readonly string[] _keysStock = { "stock", "cantidad", "existencia", "cant" };
        private readonly string[] _keysProvNombre = { "prov", "proveedor", "fabricante", "marca" };

        // =============================================================
        //  MÉTODO 1: IMPORTACIÓN INTELIGENTE (EL "KILLER FEATURE")
        // =============================================================
        public ResultadoImportacion ImportarProductosInteligente(string rutaArchivo)
        {
            var resultado = new ResultadoImportacion();

            using (var context = new AlmacenDbContext())
            {
                // Cache local de proveedores para velocidad (evita consultar BD por cada fila)
                var cacheProveedores = context.Proveedores.ToDictionary(p => p.Nombre.ToUpper(), p => p.Id);

                // Aseguramos un Proveedor General por defecto
                int idProvGeneral = ObtenerOCrearProveedorId(context, cacheProveedores, "PROVEEDOR GENERAL");

                using (var workbook = new XLWorkbook(rutaArchivo))
                {
                    var hoja = workbook.Worksheets.First();
                    var filas = hoja.RowsUsed();
                    var mapa = DetectarColumnas(hoja.Row(1));

                    foreach (var fila in filas.Skip(1)) // Saltar cabecera
                    {
                        try
                        {
                            resultado.Procesados++;
                            string codigo = LeerCelda(fila, mapa["codigo"]);
                            if (string.IsNullOrEmpty(codigo)) continue;

                            string nombre = LeerCelda(fila, mapa["nombre"]);
                            decimal precio = LeerDecimal(fila, mapa["precio"]);
                            decimal costo = LeerDecimal(fila, mapa["costo"]);
                            int stock = LeerInt(fila, mapa["stock"]);

                            // LÓGICA INTELIGENTE DE PROVEEDOR
                            int idProvFinal = idProvGeneral;
                            if (mapa.ContainsKey("proveedor") && mapa["proveedor"] != -1)
                            {
                                string nombreProvExcel = LeerCelda(fila, mapa["proveedor"]);
                                if (!string.IsNullOrWhiteSpace(nombreProvExcel))
                                {
                                    idProvFinal = ObtenerOCrearProveedorId(context, cacheProveedores, nombreProvExcel);
                                }
                            }

                            var productoExistente = context.Productos.FirstOrDefault(p => p.CodigoBarras == codigo);

                            if (productoExistente != null)
                            {
                                // UPDATE
                                if (!string.IsNullOrEmpty(nombre)) productoExistente.Nombre = nombre;
                                if (precio > 0) productoExistente.Precio = precio;
                                if (costo > 0) productoExistente.Costo = costo;
                                if (mapa["stock"] != -1) productoExistente.Stock = stock;

                                // Si el Excel especifica proveedor, actualizamos. Si no, dejamos el que tenía.
                                if (mapa.ContainsKey("proveedor") && mapa["proveedor"] != -1)
                                {
                                    productoExistente.ProveedorId = idProvFinal;
                                }

                                resultado.Actualizados++;
                            }
                            else
                            {
                                // INSERT
                                var nuevo = new Producto
                                {
                                    CodigoBarras = codigo,
                                    Nombre = string.IsNullOrEmpty(nombre) ? "NUEVO IMPORTADO" : nombre,
                                    Precio = precio,
                                    Costo = costo,
                                    Stock = stock,
                                    StockMinimo = 5,
                                    ProveedorId = idProvFinal,
                                    Impuesto = 0
                                };
                                context.Productos.Add(nuevo);
                                resultado.Nuevos++;
                            }
                        }
                        catch (Exception ex)
                        {
                            resultado.Errores++;
                            resultado.MensajesError.Add($"Fila {fila.RowNumber()}: {ex.Message}");
                        }
                    }
                    context.SaveChanges();
                }
            }
            return resultado;
        }

        private int ObtenerOCrearProveedorId(AlmacenDbContext context, Dictionary<string, int> cache, string nombreProveedor)
        {
            string key = nombreProveedor.ToUpper().Trim();

            if (cache.ContainsKey(key)) return cache[key];

            var nuevoProv = new Proveedor
            {
                Nombre = nombreProveedor,
                Cuit = "-",
                Direccion = "-",
                Telefono = "-",
                Contacto = "-"
            };

            context.Proveedores.Add(nuevoProv);
            context.SaveChanges();

            cache.Add(key, nuevoProv.Id);
            return nuevoProv.Id;
        }

        // =============================================================
        //  MÉTODO 2: EXPORTAR PRODUCTOS
        // =============================================================
        public void ExportarProductos(string rutaArchivo, List<Producto> productos)
        {
            using (var workbook = new XLWorkbook())
            {
                var hoja = workbook.Worksheets.Add("Inventario");
                ConfigurarCabecera(hoja);

                int fila = 2;
                foreach (var p in productos)
                {
                    hoja.Cell(fila, 1).Value = p.CodigoBarras;
                    hoja.Cell(fila, 1).Style.NumberFormat.Format = "@"; // Texto plano
                    hoja.Cell(fila, 2).Value = p.Nombre;
                    hoja.Cell(fila, 3).Value = p.Costo;
                    hoja.Cell(fila, 4).Value = p.Precio;
                    hoja.Cell(fila, 5).Value = p.Stock;
                    hoja.Cell(fila, 6).Value = p.Proveedor?.Nombre ?? "-";
                    fila++;
                }
                hoja.Columns().AdjustToContents();
                workbook.SaveAs(rutaArchivo);
            }
        }

        // =============================================================
        //  MÉTODO 3: GENERAR PLANTILLA (EL FALTANTE)
        // =============================================================
        public void GenerarPlantilla(string rutaGuardado)
        {
            using (var workbook = new XLWorkbook())
            {
                var hoja = workbook.Worksheets.Add("Plantilla Carga");
                ConfigurarCabecera(hoja);

                // Agregamos una fila de ejemplo para que el usuario entienda
                hoja.Cell(2, 1).Value = "779123456789"; // Código
                hoja.Cell(2, 1).Style.NumberFormat.Format = "@";
                hoja.Cell(2, 2).Value = "Ejemplo Gaseosa 1.5L"; // Nombre
                hoja.Cell(2, 3).Value = 500; // Costo
                hoja.Cell(2, 4).Value = 1000; // Precio
                hoja.Cell(2, 5).Value = 50; // Stock
                hoja.Cell(2, 6).Value = "Coca Cola Distribuidora"; // Proveedor

                hoja.Columns().AdjustToContents();
                workbook.SaveAs(rutaGuardado);
            }
        }

        // Helper para estilizar cabeceras igual en todos los Excels
        private void ConfigurarCabecera(IXLWorksheet hoja)
        {
            var cabecera = hoja.Row(1);
            cabecera.Style.Font.Bold = true;
            cabecera.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
            cabecera.Style.Font.FontColor = XLColor.White;

            hoja.Cell(1, 1).Value = "Código";
            hoja.Cell(1, 2).Value = "Nombre";
            hoja.Cell(1, 3).Value = "Costo";
            hoja.Cell(1, 4).Value = "Precio Venta";
            hoja.Cell(1, 5).Value = "Stock";
            hoja.Cell(1, 6).Value = "Proveedor";
        }

        // --- Detección de Columnas ---
        private Dictionary<string, int> DetectarColumnas(IXLRow filaCabecera)
        {
            var mapa = new Dictionary<string, int> {
                {"codigo", -1}, {"nombre", -1}, {"precio", -1}, {"costo", -1}, {"stock", -1}, {"proveedor", -1}
            };

            foreach (var celda in filaCabecera.CellsUsed())
            {
                string txt = celda.GetString().ToLower().Trim();
                int idx = celda.Address.ColumnNumber;

                if (mapa["codigo"] == -1 && _keysCodigo.Any(k => txt.Contains(k))) mapa["codigo"] = idx;
                else if (mapa["nombre"] == -1 && _keysNombre.Any(k => txt.Contains(k))) mapa["nombre"] = idx;
                else if (mapa["precio"] == -1 && _keysPrecio.Any(k => txt.Contains(k))) mapa["precio"] = idx;
                else if (mapa["proveedor"] == -1 && _keysProvNombre.Any(k => txt.Contains(k))) mapa["proveedor"] = idx;
                else if (mapa["costo"] == -1 && _keysCosto.Any(k => txt.Contains(k))) mapa["costo"] = idx;
                else if (mapa["stock"] == -1 && _keysStock.Any(k => txt.Contains(k))) mapa["stock"] = idx;
            }
            if (mapa["codigo"] == -1) throw new Exception("No encontré columna 'Código'.");
            return mapa;
        }

        private string LeerCelda(IXLRow fila, int colIdx) => colIdx == -1 ? "" : fila.Cell(colIdx).GetValue<string>().Trim();
        private decimal LeerDecimal(IXLRow fila, int colIdx) => (colIdx != -1 && decimal.TryParse(fila.Cell(colIdx).Value.ToString(), out decimal d)) ? d : 0;
        private int LeerInt(IXLRow fila, int colIdx) => (colIdx != -1 && int.TryParse(fila.Cell(colIdx).Value.ToString(), out int i)) ? i : 0;
    }
}
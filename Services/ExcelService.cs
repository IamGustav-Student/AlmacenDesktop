using AlmacenDesktop.Modelos;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace AlmacenDesktop.Services
{
    public class ExcelService
    {
        public void ExportarProductos(string rutaArchivo, List<Producto> productos)
        {
            // Exportar SIEMPRE generará un Excel real (.xlsx) porque es más rico en formato
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Productos");

                // Cabeceras
                worksheet.Cell(1, 1).Value = "CodigoBarras";
                worksheet.Cell(1, 2).Value = "Nombre";
                worksheet.Cell(1, 3).Value = "Costo";
                worksheet.Cell(1, 4).Value = "Impuesto";
                worksheet.Cell(1, 5).Value = "PrecioVenta";
                worksheet.Cell(1, 6).Value = "Stock";
                worksheet.Cell(1, 7).Value = "ProveedorId";

                // Estilo Cabecera
                var rangoCabecera = worksheet.Range("A1:G1");
                rangoCabecera.Style.Font.Bold = true;
                rangoCabecera.Style.Fill.BackgroundColor = XLColor.CornflowerBlue;
                rangoCabecera.Style.Font.FontColor = XLColor.White;

                // Datos
                int fila = 2;
                foreach (var prod in productos)
                {
                    worksheet.Cell(fila, 1).Style.NumberFormat.Format = "@";
                    worksheet.Cell(fila, 1).Value = prod.CodigoBarras;

                    worksheet.Cell(fila, 2).Value = prod.Nombre;
                    worksheet.Cell(fila, 3).Value = prod.Costo;
                    worksheet.Cell(fila, 4).Value = prod.Impuesto;
                    worksheet.Cell(fila, 5).Value = prod.Precio;
                    worksheet.Cell(fila, 6).Value = prod.Stock;
                    worksheet.Cell(fila, 7).Value = prod.ProveedorId;
                    fila++;
                }

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(rutaArchivo);
            }
        }

        public List<ProductoExcelDTO> LeerProductos(string rutaArchivo)
        {
            string extension = Path.GetExtension(rutaArchivo).ToLower();

            if (extension == ".csv")
            {
                return LeerDesdeCSV(rutaArchivo);
            }
            else
            {
                return LeerDesdeExcel(rutaArchivo);
            }
        }

        private List<ProductoExcelDTO> LeerDesdeExcel(string rutaArchivo)
        {
            var lista = new List<ProductoExcelDTO>();

            using (var workbook = new XLWorkbook(rutaArchivo))
            {
                var worksheet = workbook.Worksheet(1);
                var filas = worksheet.RangeUsed().RowsUsed().Skip(1); // Saltamos cabecera

                foreach (var fila in filas)
                {
                    try
                    {
                        var dto = new ProductoExcelDTO
                        {
                            CodigoBarras = fila.Cell(1).GetValue<string>(),
                            Nombre = fila.Cell(2).GetValue<string>(),
                            Costo = fila.Cell(3).GetValue<decimal>(),
                            Impuesto = fila.Cell(4).GetValue<decimal>(),
                            Stock = fila.Cell(6).GetValue<int>(),
                            ProveedorId = fila.Cell(7).GetValue<int>()
                        };

                        if (!string.IsNullOrWhiteSpace(dto.Nombre))
                        {
                            lista.Add(dto);
                        }
                    }
                    catch { continue; }
                }
            }
            return lista;
        }

        private List<ProductoExcelDTO> LeerDesdeCSV(string rutaArchivo)
        {
            var lista = new List<ProductoExcelDTO>();

            // Usamos Encoding.Default para intentar leer tildes si el archivo es ANSI, 
            // o UTF8 si es moderno.
            var lineas = File.ReadAllLines(rutaArchivo, Encoding.UTF8);

            // Saltamos la primera línea (cabecera)
            for (int i = 1; i < lineas.Length; i++)
            {
                var linea = lineas[i];
                if (string.IsNullOrWhiteSpace(linea)) continue;

                var columnas = linea.Split(',');

                // Validamos que tenga al menos las columnas necesarias
                if (columnas.Length >= 7)
                {
                    try
                    {
                        var dto = new ProductoExcelDTO
                        {
                            CodigoBarras = columnas[0].Trim(),
                            Nombre = columnas[1].Trim(),
                            // Parseamos números usando CultureInfo.InvariantCulture para asegurar que el punto sea decimal
                            Costo = decimal.Parse(columnas[2].Trim(), CultureInfo.InvariantCulture),
                            Impuesto = decimal.Parse(columnas[3].Trim(), CultureInfo.InvariantCulture),
                            // Columna 4 es PrecioVenta (ignorado)
                            Stock = int.Parse(columnas[5].Trim()),
                            ProveedorId = int.Parse(columnas[6].Trim())
                        };

                        lista.Add(dto);
                    }
                    catch
                    {
                        // Si falla el parseo de una línea, la saltamos
                        continue;
                    }
                }
            }

            return lista;
        }
    }

    public class ProductoExcelDTO
    {
        public string CodigoBarras { get; set; }
        public string Nombre { get; set; }
        public decimal Costo { get; set; }
        public decimal Impuesto { get; set; }
        public int Stock { get; set; }
        public int ProveedorId { get; set; }
    }
}
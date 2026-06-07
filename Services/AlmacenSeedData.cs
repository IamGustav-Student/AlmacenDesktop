using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmacenDesktop.Services
{
    public static class AlmacenSeedData
    {
        public static void SembrarSiVacio()
        {
            using (var context = new AlmacenDbContext())
            {
                // Solo sembrar si no hay ningún producto en la base de datos
                if (!context.Productos.Any())
                {
                    SembrarProductos(context);
                }
            }
        }

        public static (int Creados, int Actualizados) SembrarProductos(AlmacenDbContext context)
        {
            int creados = 0;
            int actualizados = 0;

            // 1. Asegurar la existencia de los Proveedores Líderes
            var providersMap = new Dictionary<string, Proveedor>();
            string[] provNames = {
                "La Serenísima / Danone",
                "Arcor S.A.",
                "Molinos Río de la Plata",
                "Coca-Cola Company",
                "Cervecería y Maltería Quilmes",
                "Unilever Argentina",
                "PROVEEDOR GENERAL"
            };

            foreach (var name in provNames)
            {
                var prov = context.Proveedores.FirstOrDefault(p => p.Nombre == name);
                if (prov == null)
                {
                    prov = new Proveedor
                    {
                        Nombre = name,
                        Cuit = "30-" + new Random().Next(10000000, 99999999) + "-9",
                        Direccion = "Av. Directorio 1234, CABA",
                        Telefono = "0800-444-8888",
                        Contacto = "Ventas " + name.Split(' ')[0]
                    };
                    context.Proveedores.Add(prov);
                }
                providersMap[name] = prov;
            }
            context.SaveChanges(); // Guardar proveedores para tener IDs válidos

            // Cache local de productos existentes para evitar colisiones
            var cacheProductos = context.Productos.ToDictionary(p => p.CodigoBarras, p => p);

            // 2. Insertar Productos de Fábrica 100% Reales
            var productosReales = ObtenerProductos100PorCientoReales();
            foreach (var pr in productosReales)
            {
                string provName = GetProviderNameForBrand(ExtraerMarca(pr.Nombre));
                pr.ProveedorId = providersMap[provName].Id;

                if (cacheProductos.TryGetValue(pr.CodigoBarras, out var existente))
                {
                    // Actualizar
                    existente.Nombre = pr.Nombre;
                    existente.Costo = pr.Costo;
                    existente.Precio = pr.Precio;
                    existente.Stock = pr.Stock;
                    existente.Activo = true;
                    actualizados++;
                }
                else
                {
                    // Crear nuevo
                    context.Productos.Add(pr);
                    cacheProductos.Add(pr.CodigoBarras, pr);
                    creados++;
                }
            }

            // 3. Generación Combinatoria para llegar a miles (+1,500 productos)
            var combinador = ObtenerDefinicionRubros();
            var contadoresCodigo = new Dictionary<string, int>();

            foreach (var rubro in combinador)
            {
                decimal costoBase = rubro.CostoBase;
                decimal margen = rubro.Margen;

                foreach (var marca in rubro.Marcas)
                {
                    string prefijo = GetPrefixForBrand(marca);
                    if (!contadoresCodigo.ContainsKey(prefijo))
                    {
                        contadoresCodigo[prefijo] = 1;
                    }

                    foreach (var subtipo in rubro.Subtipos)
                    {
                        foreach (var tamano in rubro.Tamaños)
                        {
                            // Generar código de barras correlativo para este prefijo con checksum EAN-13 válido
                            int correlativo = contadoresCodigo[prefijo];
                            string barcode = CalcularCodigoEan13(prefijo, correlativo);
                            contadoresCodigo[prefijo]++;

                            // Generar costos y precios lógicos con variaciones leves
                            decimal factorMarca = (marca.Length % 3) * 0.05m + 1.0m; // Variación basada en marca
                            decimal factorSubtipo = (subtipo.Length % 4) * 0.03m + 1.0m;
                            decimal costoFinal = Math.Round(costoBase * factorMarca * factorSubtipo, 2);
                            decimal precioFinal = Math.Round(costoFinal * margen, 2);

                            // Ajuste por tamaño
                            if (tamano.Contains("1kg") || tamano.Contains("1.5L") || tamano.Contains("2.25L") || tamano.Contains("3L"))
                            {
                                costoFinal = Math.Round(costoFinal * 1.8m, 2);
                                precioFinal = Math.Round(precioFinal * 1.8m, 2);
                            }

                            string prodNombre = $"{rubro.Categoria} {marca} {subtipo} {tamano}";
                            string provName = GetProviderNameForBrand(marca);

                            // Evitar sobreescribir los 100% reales precargados
                            if (cacheProductos.TryGetValue(barcode, out var existente))
                            {
                                existente.Nombre = prodNombre;
                                existente.Costo = costoFinal;
                                existente.Precio = precioFinal;
                                existente.Activo = true;
                                actualizados++;
                            }
                            else
                            {
                                var nuevo = new Producto
                                {
                                    CodigoBarras = barcode,
                                    Nombre = prodNombre,
                                    Descripcion = $"{rubro.Categoria} de marca {marca} tipo {subtipo}.",
                                    Costo = costoFinal,
                                    Precio = precioFinal,
                                    Stock = new Random().Next(10, 80),
                                    StockMinimo = 5,
                                    Impuesto = 0,
                                    ProveedorId = providersMap[provName].Id,
                                    Activo = true
                                };
                                context.Productos.Add(nuevo);
                                cacheProductos.Add(barcode, nuevo);
                                creados++;
                            }
                        }
                    }
                }
            }

            context.SaveChanges(); // Guardado transaccional final masivo
            return (creados, actualizados);
        }

        private static string CalcularCodigoEan13(string prefijo, int correlativo)
        {
            string base12 = $"{prefijo}{correlativo:D5}"; // Asegura 12 dígitos
            int suma = 0;
            for (int i = 0; i < 12; i++)
            {
                int digito = base12[i] - '0';
                suma += digito * (i % 2 == 0 ? 1 : 3);
            }
            int digitoVerificador = (10 - (suma % 10)) % 10;
            return base12 + digitoVerificador;
        }

        private static string ExtraerMarca(string nombreCompleto)
        {
            string[] palabras = nombreCompleto.Split(' ');
            if (palabras.Length > 2)
            {
                // Yerba Mate Playadito 1kg -> "Playadito" es la tercera palabra
                if (palabras[0] == "Yerba" || palabras[0] == "Aceite" || palabras[0] == "Fideos" || palabras[0] == "Leche" || palabras[0] == "Dulce" || palabras[0] == "Puré" || palabras[0] == "Papel" || palabras[0] == "Rollos" || palabras[0] == "Jabón")
                {
                    if (palabras[1] == "Mate" || palabras[1] == "de" || palabras[1] == "Entera" || palabras[1] == "Descremada" || palabras[1] == "Líquido" || palabras[1] == "Higiénico" || palabras[1] == "de")
                    {
                        return palabras[2];
                    }
                    return palabras[1];
                }
            }
            return palabras[0];
        }

        private static string GetProviderNameForBrand(string brand)
        {
            switch (brand)
            {
                case "Playadito":
                case "Taragüi":
                case "Amanda":
                case "Cruz de Malta":
                case "La Merced":
                case "Rosamonte":
                case "Mañanita":
                case "Union":
                case "CBSe":
                case "Nobleza Gaucha":
                case "La Virginia":
                    return "Molinos Río de la Plata"; // Mapped here for simplicity
                case "La Serenísima":
                case "Milkaut":
                case "Casancrem":
                case "Ilolay":
                case "Tregar":
                    return "La Serenísima / Danone";
                case "Arcor":
                case "Cofler":
                case "Guaymallén":
                case "Noel":
                case "Jorgito":
                case "Havanna":
                case "Fantoche":
                case "Saladix":
                case "Dos Anclas":
                case "Celusal":
                    return "Arcor S.A.";
                case "Lucchetti":
                case "Matarazzo":
                case "Gallo":
                case "Favorita":
                case "Blancaflor":
                case "Don Vicente":
                case "Terrabusi":
                case "Canale":
                case "Fargo":
                    return "Molinos Río de la Plata";
                case "Coca-Cola":
                case "Sprite":
                case "Fanta":
                case "Villavicencio":
                case "Levité":
                case "Paso de los Toros":
                case "Manaos":
                case "Secco":
                    return "Coca-Cola Company";
                case "Quilmes":
                case "Brahma":
                case "Stella Artois":
                case "Patagonia":
                case "Heineken":
                case "Corona":
                case "Schneider":
                case "Imperial":
                    return "Cervecería y Maltería Quilmes";
                case "Ala":
                case "Skip":
                case "Hellmann's":
                case "Lux":
                case "Rexona":
                case "Axe":
                case "Dove":
                case "Sedal":
                case "Knorr":
                case "Cif":
                case "Drive":
                    return "Unilever Argentina";
                default:
                    return "PROVEEDOR GENERAL";
            }
        }

        private static string GetPrefixForBrand(string brand)
        {
            switch (brand)
            {
                case "Arcor": case "Cofler": case "Noel": return "7790040";
                case "Lucchetti": case "Matarazzo": case "Gallo": case "Favorita": case "Blancaflor": case "Don Vicente": case "Terrabusi": case "Canale": return "7790060";
                case "La Serenísima": case "Milkaut": case "Casancrem": case "Ilolay": case "Tregar": return "7790070";
                case "Coca-Cola": case "Sprite": case "Fanta": return "5449000";
                case "Quilmes": case "Brahma": case "Stella Artois": case "Patagonia": case "Heineken": case "Corona": case "Schneider": case "Imperial": return "7790895";
                case "Ala": case "Skip": case "Hellmann's": case "Lux": case "Rexona": case "Axe": case "Dove": case "Sedal": case "Knorr": case "Drive": case "Cif": return "7790236";
                case "Playadito": return "7793704";
                case "Taragüi": case "Mañanita": return "7790380";
                case "Amanda": return "7790450";
                case "Rosamonte": return "7790150";
                case "Cabrales": return "7790580";
                case "Dos Anclas": case "Celusal": return "7790580";
                default: return "7791234";
            }
        }

        private static List<Producto> ObtenerProductos100PorCientoReales()
        {
            return new List<Producto>
            {
                new Producto { CodigoBarras = "7793704000928", Nombre = "Yerba Mate Playadito 1kg", Costo = 2800m, Precio = 3900m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070218217", Nombre = "Yerba Mate Taragüi 500g", Costo = 1600m, Precio = 2300m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580510000", Nombre = "Aceite de Girasol Natura 1.5L", Costo = 1800m, Precio = 2500m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790742240805", Nombre = "Fideos Lucchetti Tallarín 500g", Costo = 700m, Precio = 1100m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060023685", Nombre = "Arroz Gallo Oro 1kg", Costo = 1200m, Precio = 1800m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040110404", Nombre = "Azúcar Ledesma Clásica 1kg", Costo = 800m, Precio = 1200m, Stock = 70, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070318214", Nombre = "Harina Favorita Leudante 1kg", Costo = 650m, Precio = 950m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070518218", Nombre = "Harina Blancaflor 0000 1kg", Costo = 900m, Precio = 1350m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070012025", Nombre = "Leche Entera La Serenísima Sachet 1L", Costo = 950m, Precio = 1350m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070012056", Nombre = "Leche Descremada La Serenísima Sachet 1L", Costo = 950m, Precio = 1350m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "5449000000996", Nombre = "Gaseosa Coca-Cola Original 2.25L", Costo = 2100m, Precio = 2900m, Stock = 100, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "5449000001009", Nombre = "Gaseosa Coca-Cola Zero 2.25L", Costo = 2100m, Precio = 2900m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895007423", Nombre = "Gaseosa Manaos Cola 2.25L", Costo = 1100m, Precio = 1600m, Stock = 90, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791234500012", Nombre = "Galletitas Oreo 117g", Costo = 800m, Precio = 1200m, Stock = 55, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060002147", Nombre = "Galletitas Pepitos 119g", Costo = 750m, Precio = 1100m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040120205", Nombre = "Galletitas Criollitas 3 x 100g", Costo = 900m, Precio = 1350m, Stock = 120, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070031125", Nombre = "Dulce de Leche La Serenísima Estilo Colonial 400g", Costo = 1400m, Precio = 2000m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580131472", Nombre = "Mayonesa Hellmann's Clásica Doypack 250g", Costo = 650m, Precio = 980m, Stock = 65, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790111122223", Nombre = "Puré de Tomate Arcor Tetra 520g", Costo = 500m, Precio = 750m, Stock = 85, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236000571", Nombre = "Detergente Ala Colágeno 750ml", Costo = 1100m, Precio = 1650m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236001240", Nombre = "Jabón Líquido Ala para Lavarropas 800ml", Costo = 2400m, Precio = 3500m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070231223", Nombre = "Manteca La Serenísima 200g", Costo = 1200m, Precio = 1700m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790111000305", Nombre = "Mermelada Arcor Durazno 390g", Costo = 950m, Precio = 1400m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580402123", Nombre = "Sal Fina Dos Anclas 500g", Costo = 450m, Precio = 680m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040118226", Nombre = "Vinagre de Alcohol Menoyo 1L", Costo = 600m, Precio = 900m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895000516", Nombre = "Cerveza Quilmes Clásica Lata 473ml", Costo = 750m, Precio = 1100m, Stock = 150, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070415609", Nombre = "Queso Crema Casancrem Clásico 290g", Costo = 1800m, Precio = 2600m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236002131", Nombre = "Jabón de Tocador Lux Suave 3x125g", Costo = 950m, Precio = 1400m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791234500029", Nombre = "Rollos de Cocina Elegante 3u", Costo = 850m, Precio = 1300m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791234500036", Nombre = "Papel Higiénico Campanita 4 rollos", Costo = 1000m, Precio = 1500m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true }
            };
        }

        private static List<CombinacionRubro> ObtenerDefinicionRubros()
        {
            return new List<CombinacionRubro>
            {
                new CombinacionRubro {
                    Categoria = "Yerba Mate",
                    Marcas = new[] { "Playadito", "Taragüi", "Amanda", "Cruz de Malta", "La Merced", "Rosamonte", "Mañanita", "Union", "CBSe", "Nobleza Gaucha" },
                    Subtipos = new[] { "Suave", "Con Palo", "Sin Palo", "Hierbas Serranas", "Despalada", "Orgánica", "Limón", "Menta" },
                    Tamaños = new[] { "500g", "1kg" },
                    CostoBase = 1800m,
                    Margen = 1.40m
                },
                new CombinacionRubro {
                    Categoria = "Fideos Secos",
                    Marcas = new[] { "Matarazzo", "Lucchetti", "Don Vicente", "Terrabusi", "Canale", "Knorr", "Gallo" },
                    Subtipos = new[] { "Tallarín", "Tirabuzón", "Codito", "Mostachol", "Moñito", "Espagueti", "Fusilli", "Nidos" },
                    Tamaños = new[] { "500g" },
                    CostoBase = 600m,
                    Margen = 1.50m
                },
                new CombinacionRubro {
                    Categoria = "Gaseosas y Aguas",
                    Marcas = new[] { "Coca-Cola", "Pepsi", "Manaos", "Secco", "Paso de los Toros", "Sprite", "Fanta", "Villavicencio", "Levité" },
                    Subtipos = new[] { "Original", "Zero", "Light", "Lima Limón", "Naranja", "Pomelo", "Tónica", "Agua Mineral", "Manzana" },
                    Tamaños = new[] { "500ml", "1.5L", "2.25L", "3L" },
                    CostoBase = 1200m,
                    Margen = 1.35m
                },
                new CombinacionRubro {
                    Categoria = "Galletitas",
                    Marcas = new[] { "Oreo", "Pepitos", "Bagley", "Criollitas", "Traviata", "Don Satur", "Melba", "Lincoln", "Hogareñas", "Paseo" },
                    Subtipos = new[] { "Dulces Rellenas", "Saladas de Agua", "Salvado", "Bizcochos de Grasa", "Pepitas de Membrillo", "Miel", "Chocolate", "Surtidas" },
                    Tamaños = new[] { "100g", "150g", "300g" },
                    CostoBase = 600m,
                    Margen = 1.45m
                },
                new CombinacionRubro {
                    Categoria = "Lácteos y Derivados",
                    Marcas = new[] { "La Serenísima", "Sancor", "Milkaut", "Ilolay", "Tregar", "Casancrem" },
                    Subtipos = new[] { "Leche Entera sachet", "Leche Descremada sachet", "Dulce de Leche Colonial", "Manteca Clásica", "Queso Crema", "Queso Rallado", "Crema de Leche", "Yogur Frutilla", "Yogur Vainilla" },
                    Tamaños = new[] { "200g", "400g", "1L", "290g" },
                    CostoBase = 900m,
                    Margen = 1.35m
                },
                new CombinacionRubro {
                    Categoria = "Productos de Limpieza",
                    Marcas = new[] { "Ala", "Ayudín", "Poett", "Cif", "Magistral", "Procacen", "Vanish", "Mr Músculo" },
                    Subtipos = new[] { "Lavandina Clásica", "Limpiador Piso Pino", "Limpiador Piso Lavanda", "Detergente Limón", "Detergente Manzana", "Antigrasa Cocina", "Desinfectante Aerosol", "Limpiador Vidrios" },
                    Tamaños = new[] { "500ml", "900ml", "1.5L" },
                    CostoBase = 1000m,
                    Margen = 1.40m
                },
                new CombinacionRubro {
                    Categoria = "Perfumería y Cosmética",
                    Marcas = new[] { "Colgate", "Rexona", "Dove", "Axe", "Lux", "Sedal", "Pantene", "Oral-B" },
                    Subtipos = new[] { "Crema Dental Triple Acción", "Desodorante Aerosol", "Jabón Tocador Suave", "Shampoo Control Caspa", "Acondicionador Brillo", "Enjuague Bucal Fresh" },
                    Tamaños = new[] { "90g", "150ml", "3x125g", "400ml" },
                    CostoBase = 1200m,
                    Margen = 1.45m
                },
                new CombinacionRubro {
                    Categoria = "Arroz y Cereales",
                    Marcas = new[] { "Gallo", "Lucchetti", "Molinos", "Apóstoles", "Ala" },
                    Subtipos = new[] { "Largo Fino", "Doble Carolina", "Integral", "Parboil (no se pasa)", "Carnaroli" },
                    Tamaños = new[] { "500g", "1kg" },
                    CostoBase = 800m,
                    Margen = 1.45m
                },
                new CombinacionRubro {
                    Categoria = "Harinas y Repostería",
                    Marcas = new[] { "Blancaflor", "Favorita", "Pureza", "Cañuelas", "Chango" },
                    Subtipos = new[] { "Harina 000", "Harina 0000", "Harina Leudante", "Premezcla Vainilla", "Premezcla Chocolate" },
                    Tamaños = new[] { "1kg" },
                    CostoBase = 600m,
                    Margen = 1.40m
                },
                new CombinacionRubro {
                    Categoria = "Cervezas y Alcohol",
                    Marcas = new[] { "Quilmes", "Brahma", "Stella Artois", "Patagonia", "Heineken", "Corona", "Schneider", "Imperial" },
                    Subtipos = new[] { "Rubia Clásica", "Bock Negra", "IPA Andina", "Red Lager", "Lager Premium" },
                    Tamaños = new[] { "473ml (Lata)", "1L (Botella)", "730ml" },
                    CostoBase = 800m,
                    Margen = 1.35m
                },
                new CombinacionRubro {
                    Categoria = "Conservas y Almacén",
                    Marcas = new[] { "Arcor", "La Campagnola", "Gomes da Costa", "Cica", "Noel", "Canale" },
                    Subtipos = new[] { "Puré de Tomates", "Tomates Pelados Enteros", "Choclo en Granos", "Arvejas Secas", "Lentejas Remojadas", "Atún al Natural", "Atún en Aceite" },
                    Tamaños = new[] { "340g", "520g", "170g" },
                    CostoBase = 700m,
                    Margen = 1.45m
                },
                new CombinacionRubro {
                    Categoria = "Café, Té e Infusiones",
                    Marcas = new[] { "Cabrales", "La Virginia", "Nescafé", "Taragüi", "Green Hills", "Cruz de Malta" },
                    Subtipos = new[] { "Café Instantáneo", "Café Molido Filtro", "Té Común Saquitos", "Té Boldo", "Té Manzanilla", "Mate Cocido Saquitos" },
                    Tamaños = new[] { "100g", "250g", "50 saquitos" },
                    CostoBase = 1500m,
                    Margen = 1.40m
                },
                new CombinacionRubro {
                    Categoria = "Panificados y Snacks",
                    Marcas = new[] { "Fargo", "Bimbo", "Lays", "Doritos", "Krachitos", "Saladix" },
                    Subtipos = new[] { "Pan Lactal Blanco", "Pan Lactal Integral", "Papas Fritas Clásicas", "Chizitos Queso", "Maní Salado", "Papas Acanaladas" },
                    Tamaños = new[] { "100g", "350g", "500g" },
                    CostoBase = 700m,
                    Margen = 1.50m
                },
                new CombinacionRubro {
                    Categoria = "Aceites y Vinagres",
                    Marcas = new[] { "Natura", "Cocinero", "Cañuelas", "Menoyo", "Lira" },
                    Subtipos = new[] { "Aceite Mezcla", "Aceite Girasol", "Aceite de Oliva", "Vinagre de Alcohol", "Vinagre de Manzana", "Aceto Balsámico" },
                    Tamaños = new[] { "500ml", "900ml", "1.5L" },
                    CostoBase = 1100m,
                    Margen = 1.40m
                },
                new CombinacionRubro {
                    Categoria = "Condimentos y Caldos",
                    Marcas = new[] { "Dos Anclas", "Celusal", "Knorr", "Alicante" },
                    Subtipos = new[] { "Sal Fina Mesa", "Sal Gruesa Parrillera", "Caldo Gallina Cubos", "Caldo Verdura Cubos", "Orégano Molido", "Provenzal Mezcla", "Pimentón Dulce" },
                    Tamaños = new[] { "250g", "500g", "12u", "25g" },
                    CostoBase = 500m,
                    Margen = 1.50m
                }
            };
        }

        private class CombinacionRubro
        {
            public string Categoria { get; set; } = default!;
            public string[] Marcas { get; set; } = default!;
            public string[] Subtipos { get; set; } = default!;
            public string[] Tamaños { get; set; } = default!;
            public decimal CostoBase { get; set; }
            public decimal Margen { get; set; }
        }
    }
}

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

            context.SaveChanges(); // Guardado transaccional final masivo
            return (creados, actualizados);
        }

        private static string ExtraerMarca(string nombreCompleto)
        {
            string[] palabras = nombreCompleto.Split(' ');
            if (palabras.Length > 2)
            {
                // Yerba Mate Playadito 1kg -> "Playadito" es la tercera palabra
                if (palabras[0] == "Yerba" || palabras[0] == "Aceite" || palabras[0] == "Fideos" || palabras[0] == "Leche" || palabras[0] == "Dulce" || palabras[0] == "Puré" || palabras[0] == "Papel" || palabras[0] == "Rollos" || palabras[0] == "Jabón" || palabras[0] == "Pan" || palabras[0] == "Cerveza" || palabras[0] == "Gaseosa" || palabras[0] == "Agua" || palabras[0] == "Shampoo" || palabras[0] == "Queso" || palabras[0] == "Crema" || palabras[0] == "Harina" || palabras[0] == "Arroz")
                {
                    if (palabras[1] == "Mate" || palabras[1] == "de" || palabras[1] == "Entera" || palabras[1] == "Descremada" || palabras[1] == "Líquido" || palabras[1] == "Higiénico" || palabras[1] == "Lactal" || palabras[1] == "Saborizada" || palabras[1] == "Mineral" || palabras[1] == "Dental" || palabras[1] == "Crema" || palabras[1] == "Rallado" || palabras[1] == "Lomo" || palabras[1] == "Firme" || palabras[1] == "Secos" || palabras[1] == "Finas")
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
                case "Cruz": // Cruz de Malta
                case "La": // La Merced / La Virginia / La Campagnola
                case "La Virginia":
                case "La Merced":
                case "Rosamonte":
                case "Mañanita":
                case "Union":
                case "CBSe":
                case "Nobleza": // Nobleza Gaucha
                case "Cabrales":
                    return "Molinos Río de la Plata"; // Mapeados de forma simplificada
                case "La Serenísima":
                case "La Serenisima":
                case "Milkaut":
                case "Casancrem":
                case "Ilolay":
                case "Tregar":
                case "Sancor":
                    return "La Serenísima / Danone";
                case "Arcor":
                case "Cofler":
                case "Guaymallén":
                case "Guaymallen":
                case "Noel":
                case "Jorgito":
                case "Jorgelin":
                case "Jorgelín":
                case "Havanna":
                case "Fantoche":
                case "Saladix":
                case "Dos": // Dos Anclas
                case "Celusal":
                case "Menoyo":
                case "Oreo":
                case "Pepitos":
                case "Criollitas":
                case "Chocolinas":
                case "Traviata":
                case "Don": // Don Satur
                case "Opera":
                case "Bagley":
                case "Ledesma":
                case "La Campagnola":
                    return "Arcor S.A.";
                case "Lucchetti":
                case "Matarazzo":
                case "Gallo":
                case "Favorita":
                case "Blancaflor":
                case "Don Vicente":
                case "Terrabusi":
                case "Canale":
                case "Pureza":
                case "Cañuelas":
                case "Cocinero":
                case "Natura":
                    return "Molinos Río de la Plata";
                case "Coca-Cola":
                case "Pepsi":
                case "Sprite":
                case "Fanta":
                case "Villavicencio":
                case "Levité":
                case "Paso": // Paso de los Toros
                case "Manaos":
                case "Secco":
                    return "Coca-Cola Company";
                case "Quilmes":
                case "Brahma":
                case "Stella": // Stella Artois
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
                case "Lactal":
                case "Lays":
                case "Doritos":
                case "Krachitos":
                case "Colgate":
                case "Campanita":
                case "Elegante":
                    return "Unilever Argentina";
                default:
                    return "PROVEEDOR GENERAL";
            }
        }

        private static List<Producto> ObtenerProductos100PorCientoReales()
        {
            return new List<Producto>
            {
                // 1. Yerba Mate
                new Producto { CodigoBarras = "7793704000928", Nombre = "Yerba Mate Playadito 1kg", Costo = 2800m, Precio = 3900m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7793704000911", Nombre = "Yerba Mate Playadito 500g", Costo = 1500m, Precio = 2100m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790380000624", Nombre = "Yerba Mate Taragüi Con Palo 500g", Costo = 1600m, Precio = 2300m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790380000617", Nombre = "Yerba Mate Taragüi Con Palo 1kg", Costo = 2950m, Precio = 4100m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790380023029", Nombre = "Yerba Mate Mañanita Con Palo 500g", Costo = 1450m, Precio = 2050m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790450048231", Nombre = "Yerba Mate Amanda Tradicional 1kg", Costo = 2700m, Precio = 3800m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790450048224", Nombre = "Yerba Mate Amanda Tradicional 500g", Costo = 1400m, Precio = 1950m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790150000850", Nombre = "Yerba Mate Rosamonte Especial 1kg", Costo = 3100m, Precio = 4400m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790150000843", Nombre = "Yerba Mate Rosamonte Especial 500g", Costo = 1650m, Precio = 2350m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7798062541334", Nombre = "Yerba Mate CBSe Hierbas Serranas 500g", Costo = 1350m, Precio = 1900m, Stock = 55, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7798062541242", Nombre = "Yerba Mate CBSe Naranja 500g", Costo = 1350m, Precio = 1900m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790380003731", Nombre = "Yerba Mate La Merced De Campo 500g", Costo = 2100m, Precio = 2950m, Stock = 15, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790380003724", Nombre = "Yerba Mate La Merced Barbacuá 500g", Costo = 2100m, Precio = 2950m, Stock = 10, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790380000815", Nombre = "Yerba Mate Unión Suave 500g", Costo = 1550m, Precio = 2200m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 2. Aceites y Condimentos
                new Producto { CodigoBarras = "7790580510000", Nombre = "Aceite de Girasol Natura 1.5L", Costo = 1800m, Precio = 2500m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580120155", Nombre = "Aceite de Girasol Cocinero 900ml", Costo = 1100m, Precio = 1550m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580120162", Nombre = "Aceite de Girasol Cocinero 1.5L", Costo = 1800m, Precio = 2500m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7792180001665", Nombre = "Aceite Mezcla Cañuelas 1.5L", Costo = 1650m, Precio = 2300m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580131472", Nombre = "Mayonesa Hellmann's Clásica Doypack 250g", Costo = 650m, Precio = 980m, Stock = 65, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580131496", Nombre = "Mayonesa Hellmann's Clásica Doypack 475g", Costo = 1100m, Precio = 1600m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580402123", Nombre = "Sal Fina Dos Anclas 500g", Costo = 450m, Precio = 680m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580202103", Nombre = "Sal Gruesa Dos Anclas 1kg", Costo = 550m, Precio = 850m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040118226", Nombre = "Vinagre de Alcohol Menoyo 1L", Costo = 600m, Precio = 900m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790639002449", Nombre = "Ketchup Hellmann's Doypack 250g", Costo = 680m, Precio = 990m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580108603", Nombre = "Mostaza Natura Doypack 500g", Costo = 850m, Precio = 1250m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 3. Galletitas y Alfajores
                new Producto { CodigoBarras = "7622300840250", Nombre = "Galletitas Oreo Original 117g", Costo = 800m, Precio = 1200m, Stock = 55, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060002147", Nombre = "Galletitas Pepitos Original 119g", Costo = 750m, Precio = 1100m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040120205", Nombre = "Galletitas Criollitas 3 x 100g", Costo = 900m, Precio = 1350m, Stock = 120, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040103758", Nombre = "Galletitas Chocolinas 250g", Costo = 1300m, Precio = 1950m, Stock = 70, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040121103", Nombre = "Galletitas Traviata Pack x3 301g", Costo = 950m, Precio = 1400m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791690000057", Nombre = "Bizcochos de Grasa Don Satur 200g", Costo = 580m, Precio = 850m, Stock = 100, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791690000088", Nombre = "Bizcochos Dulces Don Satur 200g", Costo = 580m, Precio = 850m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040122247", Nombre = "Galletitas Surtido Bagley 400g", Costo = 1200m, Precio = 1750m, Stock = 90, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040101808", Nombre = "Galletitas Rumba 112g", Costo = 650m, Precio = 980m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040101709", Nombre = "Galletitas Mellizas 112g", Costo = 650m, Precio = 980m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040101907", Nombre = "Galletitas Amor 112g", Costo = 680m, Precio = 1050m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040860224", Nombre = "Alfajor Jorgito Chocolate 55g", Costo = 450m, Precio = 700m, Stock = 150, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040860231", Nombre = "Alfajor Jorgito Glaseado Blanco 55g", Costo = 450m, Precio = 700m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040125439", Nombre = "Obleas Opera Original 220g", Costo = 820m, Precio = 1250m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "77903518", Nombre = "Alfajor Guaymallén Chocolate 38g", Costo = 250m, Precio = 400m, Stock = 200, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "77903525", Nombre = "Alfajor Guaymallén Blanco 38g", Costo = 250m, Precio = 400m, Stock = 180, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "77903532", Nombre = "Alfajor Guaymallén Membrillo 38g", Costo = 250m, Precio = 400m, Stock = 120, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "77926830", Nombre = "Alfajor Triple Jorgelín Blanco 85g", Costo = 750m, Precio = 1100m, Stock = 90, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "77926847", Nombre = "Alfajor Triple Jorgelín Negro 85g", Costo = 750m, Precio = 1100m, Stock = 110, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 4. Bebidas y Gaseosas
                new Producto { CodigoBarras = "7790895000998", Nombre = "Gaseosa Coca-Cola Original 2.25L", Costo = 2100m, Precio = 2900m, Stock = 100, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895001018", Nombre = "Gaseosa Coca-Cola Zero 2.25L", Costo = 2100m, Precio = 2900m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895000967", Nombre = "Gaseosa Coca-Cola Original 1.5L", Costo = 1600m, Precio = 2200m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895010188", Nombre = "Gaseosa Sprite Sin Azúcar 2.25L", Costo = 2000m, Precio = 2800m, Stock = 70, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895005085", Nombre = "Gaseosa Fanta Naranja 2.25L", Costo = 2000m, Precio = 2800m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7798150470080", Nombre = "Gaseosa Manaos Cola 2.25L", Costo = 1100m, Precio = 1600m, Stock = 90, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7798150470097", Nombre = "Gaseosa Manaos Pomelo 2.25L", Costo = 1100m, Precio = 1600m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7798150470127", Nombre = "Gaseosa Manaos Naranja 2.25L", Costo = 1100m, Precio = 1600m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7799061001309", Nombre = "Gaseosa Pepsi Regular 2.25L", Costo = 1950m, Precio = 2700m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7799061001903", Nombre = "Gaseosa Paso de los Toros Pomelo 2.25L", Costo = 2000m, Precio = 2800m, Stock = 65, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7799061001941", Nombre = "Gaseosa Paso de los Toros Tónica 2.25L", Costo = 2000m, Precio = 2800m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895008543", Nombre = "Agua Mineral Villavicencio Sin Gas 1.5L", Costo = 900m, Precio = 1300m, Stock = 120, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895008550", Nombre = "Agua Mineral Villavicencio Con Gas 1.5L", Costo = 900m, Precio = 1300m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7799061002344", Nombre = "Agua Saborizada Levité Pomelo 1.5L", Costo = 1150m, Precio = 1650m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7799061002351", Nombre = "Agua Saborizada Levité Manzana 1.5L", Costo = 1150m, Precio = 1650m, Stock = 70, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7799061002368", Nombre = "Agua Saborizada Levité Naranja 1.5L", Costo = 1150m, Precio = 1650m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 5. Lácteos
                new Producto { CodigoBarras = "7790070230578", Nombre = "Leche Entera La Serenísima Sachet 1L", Costo = 950m, Precio = 1350m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070230585", Nombre = "Leche Descremada La Serenísima Sachet 1L", Costo = 950m, Precio = 1350m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070415609", Nombre = "Queso Crema Casancrem Clásico 290g", Costo = 1800m, Precio = 2600m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070415616", Nombre = "Queso Crema Casancrem Light 290g", Costo = 1800m, Precio = 2600m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070031125", Nombre = "Dulce de Leche Colonial La Serenísima 400g", Costo = 1400m, Precio = 2000m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070031156", Nombre = "Dulce de Leche Clásico La Serenísima 400g", Costo = 1300m, Precio = 1850m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070231223", Nombre = "Manteca La Serenísima 200g", Costo = 1200m, Precio = 1700m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070231100", Nombre = "Manteca La Serenísima 100g", Costo = 700m, Precio = 1000m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070507342", Nombre = "Queso Rallado La Serenísima Reggianito 150g", Costo = 2100m, Precio = 3000m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070507328", Nombre = "Queso Rallado La Serenísima Reggianito 40g", Costo = 650m, Precio = 950m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070220304", Nombre = "Crema de Leche La Serenísima 200g", Costo = 1100m, Precio = 1600m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "77931475", Nombre = "Yogur Entero Sancor Yogs Vainilla 190g", Costo = 600m, Precio = 900m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "77931482", Nombre = "Yogur Entero Sancor Yogs Frutilla 190g", Costo = 600m, Precio = 900m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 6. Fideos, Arroz y Harinas
                new Producto { CodigoBarras = "7790742240805", Nombre = "Fideos Lucchetti Tallarín 500g", Costo = 700m, Precio = 1100m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790742240829", Nombre = "Fideos Lucchetti Tirabuzón 500g", Costo = 700m, Precio = 1100m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060023685", Nombre = "Fideos Matarazzo Tirabuzón 500g", Costo = 950m, Precio = 1450m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060023678", Nombre = "Fideos Matarazzo Tallarín 500g", Costo = 950m, Precio = 1450m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060008323", Nombre = "Fideos Don Vicente Espagueti 500g", Costo = 1200m, Precio = 1800m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060002130", Nombre = "Arroz Gallo Oro Largo Fino 1kg", Costo = 1200m, Precio = 1800m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060002123", Nombre = "Arroz Gallo Oro Largo Fino 500g", Costo = 650m, Precio = 980m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070318214", Nombre = "Harina Favorita Leudante 1kg", Costo = 650m, Precio = 950m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790070518218", Nombre = "Harina Blancaflor 0000 Leudante 1kg", Costo = 900m, Precio = 1350m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060002444", Nombre = "Harina Pureza 0000 1kg", Costo = 750m, Precio = 1100m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790060002451", Nombre = "Harina Pureza con Levadura 1kg", Costo = 850m, Precio = 1250m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040110404", Nombre = "Azúcar Ledesma Clásica 1kg", Costo = 800m, Precio = 1200m, Stock = 70, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 7. Cervezas y Bebidas con Alcohol
                new Producto { CodigoBarras = "7790895000516", Nombre = "Cerveza Quilmes Clásica Lata 473ml", Costo = 750m, Precio = 1100m, Stock = 150, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895000523", Nombre = "Cerveza Quilmes Clásica Botella 1L", Costo = 1200m, Precio = 1750m, Stock = 100, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895000554", Nombre = "Cerveza Quilmes Bajo Cero Lata 473ml", Costo = 750m, Precio = 1100m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895006006", Nombre = "Cerveza Brahma Chopp Lata 473ml", Costo = 720m, Precio = 1050m, Stock = 120, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895006020", Nombre = "Cerveza Brahma Chopp Botella 1L", Costo = 1150m, Precio = 1680m, Stock = 90, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895015022", Nombre = "Cerveza Patagonia Amber Lager Lata 473ml", Costo = 1200m, Precio = 1800m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790895015046", Nombre = "Cerveza Patagonia Bohemian Pilsener Lata 473ml", Costo = 1200m, Precio = 1800m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790093100067", Nombre = "Cerveza Heineken Lata 473ml", Costo = 1100m, Precio = 1600m, Stock = 140, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790093100098", Nombre = "Cerveza Heineken Botella 1L", Costo = 1800m, Precio = 2600m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790093100227", Nombre = "Cerveza Imperial Golden Lata 473ml", Costo = 850m, Precio = 1250m, Stock = 70, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790093100241", Nombre = "Cerveza Imperial Stout Lata 473ml", Costo = 850m, Precio = 1250m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 8. Conservas y Almacén Seco
                new Producto { CodigoBarras = "7790111122223", Nombre = "Puré de Tomate Arcor Tetra 520g", Costo = 500m, Precio = 750m, Stock = 85, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790111000305", Nombre = "Mermelada Arcor Durazno 390g", Costo = 950m, Precio = 1400m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790111000312", Nombre = "Mermelada Arcor Frutilla 390g", Costo = 950m, Precio = 1400m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040116031", Nombre = "Tomate Pelado Entero Arcor Lata 400g", Costo = 800m, Precio = 1200m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790111112026", Nombre = "Arvejas Secas Remojadas Arcor Lata 350g", Costo = 450m, Precio = 680m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790111112040", Nombre = "Choclo Amarillo en Granos Arcor Lata 350g", Costo = 720m, Precio = 1080m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790520000356", Nombre = "Atún Desmenuzado al Natural La Campagnola 170g", Costo = 1100m, Precio = 1650m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790520000363", Nombre = "Atún Desmenuzado en Aceite La Campagnola 170g", Costo = 1100m, Precio = 1650m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790520000509", Nombre = "Atún en Lomitos al Natural La Campagnola 170g", Costo = 1600m, Precio = 2400m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 9. Infusiones
                new Producto { CodigoBarras = "7790580000211", Nombre = "Café Molido Cabrales Super Cabrales 250g", Costo = 3400m, Precio = 4900m, Stock = 15, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580000303", Nombre = "Café Instantáneo La Virginia Clásico 100g", Costo = 1950m, Precio = 2800m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580000327", Nombre = "Café Instantáneo La Virginia Clásico 170g", Costo = 3100m, Precio = 4500m, Stock = 15, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580000808", Nombre = "Té en Saquitos La Virginia Clásico 50u", Costo = 780m, Precio = 1150m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580000822", Nombre = "Té en Saquitos La Virginia Boldo 25u", Costo = 550m, Precio = 850m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580000846", Nombre = "Té en Saquitos La Virginia Manzanilla 25u", Costo = 550m, Precio = 850m, Stock = 25, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790580000860", Nombre = "Mate Cocido en Saquitos La Virginia 50u", Costo = 750m, Precio = 1100m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 10. Panificados y Snacks
                new Producto { CodigoBarras = "7791770005118", Nombre = "Pan Lactal Blanco Grande 560g", Costo = 1500m, Precio = 2200m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791770005125", Nombre = "Pan Lactal Integral Grande 560g", Costo = 1650m, Precio = 2400m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790177000102", Nombre = "Papas Fritas Lays Clásicas 150g", Costo = 1200m, Precio = 1800m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790177000126", Nombre = "Papas Fritas Lays Clásicas 85g", Costo = 750m, Precio = 1100m, Stock = 70, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790177001222", Nombre = "Doritos Queso Mega Queso 150g", Costo = 1400m, Precio = 2100m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790315000100", Nombre = "Palitos Salados Krachitos 120g", Costo = 600m, Precio = 900m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790315000124", Nombre = "Maní Salado Krachitos 150g", Costo = 800m, Precio = 1200m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040114006", Nombre = "Snack Saladix Jamón 80g", Costo = 550m, Precio = 800m, Stock = 90, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790040114020", Nombre = "Snack Saladix Queso 80g", Costo = 550m, Precio = 800m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true },

                // 11. Limpieza y Perfumería
                new Producto { CodigoBarras = "7790236000571", Nombre = "Detergente Ala Colágeno 750ml", Costo = 1100m, Precio = 1650m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236000557", Nombre = "Detergente Ala Limón 750ml", Costo = 1100m, Precio = 1650m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236001240", Nombre = "Jabón Líquido Ala para Lavarropas 800ml", Costo = 2400m, Precio = 3500m, Stock = 20, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236001264", Nombre = "Jabón Líquido Ala para Lavarropas 3L", Costo = 6800m, Precio = 9800m, Stock = 15, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7509546074222", Nombre = "Crema Dental Colgate Triple Acción 90g", Costo = 600m, Precio = 900m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7509546074246", Nombre = "Crema Dental Colgate Triple Acción 180g", Costo = 1100m, Precio = 1600m, Stock = 30, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236300404", Nombre = "Desodorante Rexona Aerosol Odorono 150ml", Costo = 950m, Precio = 1400m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236300428", Nombre = "Desodorante Rexona Aerosol Cotton 150ml", Costo = 950m, Precio = 1400m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236300503", Nombre = "Desodorante Axe Aerosol Apollo 150ml", Costo = 1000m, Precio = 1500m, Stock = 45, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236002131", Nombre = "Jabón de Tocador Lux Suave 3x125g", Costo = 950m, Precio = 1400m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236002155", Nombre = "Jabón de Tocador Lux Orquídea 125g", Costo = 400m, Precio = 600m, Stock = 50, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236300800", Nombre = "Shampoo Sedal Ceramidas 400ml", Costo = 1200m, Precio = 1800m, Stock = 40, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7790236300824", Nombre = "Acondicionador Sedal Ceramidas 400ml", Costo = 1200m, Precio = 1800m, Stock = 35, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791234500029", Nombre = "Rollos de Cocina Elegante 3u", Costo = 850m, Precio = 1300m, Stock = 60, StockMinimo = 5, Impuesto = 0, Activo = true },
                new Producto { CodigoBarras = "7791234500036", Nombre = "Papel Higiénico Campanita 4 rollos x 30m", Costo = 1000m, Precio = 1500m, Stock = 80, StockMinimo = 5, Impuesto = 0, Activo = true }
            };
        }
    }
}

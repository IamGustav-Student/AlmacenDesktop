using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmacenDesktop.Services
{
    // CEREBRO DE VENTAS: Encapsula toda la lógica de negocio.
    // Ventajas: Testeable, Reutilizable, Transaccional (ACID).
    public class VentaService
    {
        // Búsqueda inteligente de productos
        public Producto BuscarProducto(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return null;

            using (var context = new AlmacenDbContext())
            {
                // 1. Prioridad: Código de Barras Exacto
                var producto = context.Productos.FirstOrDefault(p => p.CodigoBarras == entrada);
                if (producto != null) return producto;

                // 2. Fallback: Búsqueda por nombre (contiene)
                var lista = context.Productos
                                   .Where(p => p.Nombre.ToLower().Contains(entrada.ToLower()) && p.Stock > 0)
                                   .ToList();

                // Regla: Solo retornamos si hay coincidencia ÚNICA para evitar errores
                return lista.Count == 1 ? lista.First() : null;
            }
        }

        // Verifica si el usuario tiene una caja abierta y retorna su ID
        public int? ObtenerCajaAbiertaId(int usuarioId)
        {
            using (var context = new AlmacenDbContext())
            {
                var caja = context.Cajas
                                  .FirstOrDefault(c => c.UsuarioId == usuarioId && c.EstaAbierta);
                return caja?.Id;
            }
        }

        // EL MÉTODO "GOD LEVEL": Transacción Atómica
        // Guarda Venta + Detalles + Descuenta Stock + Valida. Todo o Nada.
        public Venta RegistrarVenta(Venta nuevaVenta, List<DetalleVenta> carrito)
        {
            using (var context = new AlmacenDbContext())
            {
                // Iniciamos transacción de base de datos
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // 1. Guardar Cabecera de Venta
                        context.Ventas.Add(nuevaVenta);
                        context.SaveChanges(); // Obtenemos el ID generado

                        // 2. Procesar cada item del carrito
                        foreach (var item in carrito)
                        {
                            var detalle = new DetalleVenta
                            {
                                VentaId = nuevaVenta.Id,
                                ProductoId = item.ProductoId,
                                Cantidad = item.Cantidad,
                                PrecioUnitario = item.PrecioUnitario
                                // Subtotal se puede calcular aquí o en la entidad si tiene getter calculado
                            };
                            context.DetallesVenta.Add(detalle);

                            // 3. ACTUALIZAR STOCK CON VALIDACIÓN DE CONCURRENCIA
                            // Buscamos el producto nuevamente dentro de la transacción para asegurar consistencia
                            var productoDb = context.Productos.Find(item.ProductoId);

                            if (productoDb == null)
                                throw new Exception($"El producto ID {item.ProductoId} ya no existe.");

                            if (productoDb.Stock < item.Cantidad)
                                throw new Exception($"Stock insuficiente para '{productoDb.Nombre}'. Stock actual: {productoDb.Stock}.");

                            productoDb.Stock -= item.Cantidad;
                        }

                        // 4. Actualizar Caja (Sumar saldo si es Efectivo)
                        // Esto evita discrepancias entre ventas y dinero en caja
                        if (nuevaVenta.MetodoPago == "Efectivo")
                        {
                            var cajaDb = context.Cajas.Find(nuevaVenta.CajaId);
                            if (cajaDb != null)
                            {
                                cajaDb.TotalVentasEfectivo += nuevaVenta.Total;
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit(); // ¡Confirmar cambios!

                        return nuevaVenta;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback(); // Si algo falla, el sistema vuelve al estado anterior. 0 Corrupción de datos.
                        throw;
                    }
                }
            }
        }
    }
}
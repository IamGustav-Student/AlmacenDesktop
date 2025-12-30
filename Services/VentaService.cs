using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmacenDesktop.Services
{
    public class VentaService
    {
        public Producto BuscarProducto(string entrada)
        {
            if (string.IsNullOrWhiteSpace(entrada)) return null;

            using (var context = new AlmacenDbContext())
            {
                var producto = context.Productos.FirstOrDefault(p => p.CodigoBarras == entrada);
                if (producto != null) return producto;

                var lista = context.Productos
                                   .Where(p => p.Nombre.ToLower().Contains(entrada.ToLower()) && p.Stock > 0)
                                   .ToList();

                return lista.Count == 1 ? lista.First() : null;
            }
        }

        // --- VALIDACIÓN DE CAJA ---
        // Busca la última caja abierta de este usuario.
        // Debe ser FechaCierre NULL y EstaAbierta TRUE.
        public int? ObtenerCajaAbiertaId(int usuarioId)
        {
            using (var context = new AlmacenDbContext())
            {
                var caja = context.Cajas
                                  .OrderByDescending(c => c.FechaApertura)
                                  .FirstOrDefault(c => c.UsuarioId == usuarioId && c.EstaAbierta && c.FechaCierre == null);
                return caja?.Id;
            }
        }

        public Venta RegistrarVenta(Venta nuevaVenta, List<DetalleVenta> carrito)
        {
            if (nuevaVenta.ClienteId <= 0) throw new Exception("Cliente inválido o no seleccionado.");
            if (nuevaVenta.CajaId <= 0) throw new Exception("Caja inválida. Debe abrir una caja primero.");
            if (carrito == null || carrito.Count == 0) throw new Exception("El carrito está vacío.");

            if (nuevaVenta.TipoComprobante == null) nuevaVenta.TipoComprobante = "X";
            if (nuevaVenta.CAE == null) nuevaVenta.CAE = "";
            if (nuevaVenta.ObservacionesAFIP == null) nuevaVenta.ObservacionesAFIP = "";

            using (var context = new AlmacenDbContext())
            {
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        context.Ventas.Add(nuevaVenta);
                        context.SaveChanges();

                        foreach (var item in carrito)
                        {
                            var detalle = new DetalleVenta
                            {
                                VentaId = nuevaVenta.Id,
                                ProductoId = item.ProductoId,
                                Cantidad = item.Cantidad,
                                PrecioUnitario = item.PrecioUnitario,
                                Subtotal = item.Subtotal
                            };
                            context.DetallesVenta.Add(detalle);

                            var productoDb = context.Productos.Find(item.ProductoId);
                            if (productoDb == null) throw new Exception($"El producto ID {item.ProductoId} ya no existe.");

                            // Permitimos stock negativo temporalmente para no frenar la venta, 
                            // pero se podría bloquear aquí si quisieras ser estricto.
                            productoDb.Stock -= item.Cantidad;
                        }

                        if (nuevaVenta.MetodoPago == "Efectivo")
                        {
                            var cajaDb = context.Cajas.Find(nuevaVenta.CajaId);
                            if (cajaDb != null)
                            {
                                cajaDb.TotalVentasEfectivo += nuevaVenta.Total;
                            }
                        }
                        else
                        {
                            var cajaDb = context.Cajas.Find(nuevaVenta.CajaId);
                            if (cajaDb != null)
                            {
                                cajaDb.TotalVentasOtros += nuevaVenta.Total;
                            }
                        }

                        context.SaveChanges();
                        transaction.Commit();

                        return nuevaVenta;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        var mensaje = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                        throw new Exception("Error al registrar venta en base de datos: " + mensaje);
                    }
                }
            }
        }
    }
}
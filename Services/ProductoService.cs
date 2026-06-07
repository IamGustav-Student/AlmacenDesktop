using AlmacenDesktop.Data;
using AlmacenDesktop.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AlmacenDesktop.Services
{
    public class ProductoService
    {
        public List<Producto> ObtenerTodos(bool incluirInactivos = false)
        {
            using (var context = new AlmacenDbContext())
            {
                var query = context.Productos.Include(p => p.Proveedor).AsQueryable();

                if (!incluirInactivos)
                {
                    query = query.Where(p => p.Activo);
                }

                return query.OrderBy(p => p.Nombre).ToList();
            }
        }

        public Producto ObtenerPorId(int id)
        {
            using (var context = new AlmacenDbContext())
            {
                return context.Productos.Include(p => p.Proveedor).FirstOrDefault(p => p.Id == id);
            }
        }

        public List<Producto> Buscar(string termino)
        {
            using (var context = new AlmacenDbContext())
            {
                return context.Productos
                    .Include(p => p.Proveedor)
                    .Where(p => p.Activo &&
                               (p.Nombre.Contains(termino) ||
                                p.CodigoBarras.Contains(termino) ||
                                p.Proveedor.Nombre.Contains(termino)))
                    .OrderBy(p => p.Nombre)
                    .ToList();
            }
        }

        public void Guardar(Producto producto)
        {
            // Validaciones de Negocio Centralizadas
            if (producto.Precio < 0) throw new ArgumentException("El precio no puede ser negativo.");
            if (producto.Costo < 0) throw new ArgumentException("El costo no puede ser negativo.");
            if (string.IsNullOrWhiteSpace(producto.Nombre)) throw new ArgumentException("El nombre es obligatorio.");
            if (string.IsNullOrWhiteSpace(producto.CodigoBarras)) throw new ArgumentException("El código de barras es obligatorio.");

            using (var context = new AlmacenDbContext())
            {
                // Buscar si ya existe algún producto con este código de barras (activo o inactivo)
                var productoExistente = context.Productos.FirstOrDefault(p => p.CodigoBarras == producto.CodigoBarras);

                if (productoExistente != null)
                {
                    if (producto.Id == 0)
                    {
                        // Si el producto existente ya está activo, es un duplicado prohibido
                        if (productoExistente.Activo)
                        {
                            throw new InvalidOperationException("Ya existe un producto activo con este Código de Barras.");
                        }

                        // Si estaba inactivo, lo reactivamos y actualizamos con la nueva información
                        productoExistente.Nombre = producto.Nombre;
                        productoExistente.Descripcion = producto.Descripcion;
                        productoExistente.Costo = producto.Costo;
                        productoExistente.Precio = producto.Precio;
                        productoExistente.Stock = producto.Stock;
                        productoExistente.StockMinimo = producto.StockMinimo;
                        productoExistente.Impuesto = producto.Impuesto;
                        productoExistente.ProveedorId = producto.ProveedorId;
                        productoExistente.Activo = true; // Reactivación

                        context.SaveChanges();
                        producto.Id = productoExistente.Id; // Sincronizar ID de vuelta al formulario
                        return;
                    }
                    else if (productoExistente.Id != producto.Id)
                    {
                        // Si es un producto existente que intenta cambiar su código al de otro producto
                        throw new InvalidOperationException("Ya existe un producto con este Código de Barras.");
                    }
                }

                if (producto.Id == 0)
                {
                    context.Productos.Add(producto);
                }
                else
                {
                    context.Productos.Update(producto);
                }
                context.SaveChanges();
            }
        }

        public void Eliminar(int id) // Borrado Lógico
        {
            using (var context = new AlmacenDbContext())
            {
                var prod = context.Productos.Find(id);
                if (prod != null)
                {
                    prod.Activo = false; // Soft Delete
                    context.SaveChanges();
                }
            }
        }

        public bool ValidarStockSuficiente(int productoId, int cantidadRequerida)
        {
            using (var context = new AlmacenDbContext())
            {
                var stockActual = context.Productos
                    .Where(p => p.Id == productoId)
                    .Select(p => p.Stock)
                    .FirstOrDefault();

                return stockActual >= cantidadRequerida;
            }
        }
    }
}
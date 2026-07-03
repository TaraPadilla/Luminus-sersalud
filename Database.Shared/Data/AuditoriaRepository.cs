using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;

namespace Database.Shared.Data
{
    public class AuditoriaRepository : IAuditoria
    {
        private readonly Context _context = null;

        public AuditoriaRepository(Context context)
        {
            _context = context;
        }
        public void AddAuditoriaSp(object[] productos, string userId, string personaCreaAuditoria, bool actualizoStock)
        {
            var productosJson = productos.Select(p => JsonSerializer.Serialize(p)).ToArray();

            var productosParam = new NpgsqlParameter("productos", NpgsqlDbType.Array | NpgsqlDbType.Jsonb)
            {
                Value = productosJson
            };

            var userIdParam = new NpgsqlParameter("userid", userId);
            var actualizoStockParam = new NpgsqlParameter("actualizostock", actualizoStock);
            var personaCreacionAuditoriaParam = new NpgsqlParameter("personacreacionauditoria", personaCreaAuditoria);

            // Log para verificar el JSON que se está enviando
            // Console.WriteLine("JSON enviado a PostgreSQL: " + Newtonsoft.Json.JsonConvert.SerializeObject(productosJson, Newtonsoft.Json.Formatting.Indented));

            _context.Database.ExecuteSqlRaw("SELECT registrar_auditoria_producto(@productos,@userid,@actualizostock,@personacreacionauditoria)",
                productosParam, userIdParam, actualizoStockParam, personaCreacionAuditoriaParam);
        }

        public void AddAuditoria(Auditoria auditoria, bool saveChanges = true)
        {
            auditoria.Eliminada = false;

            if (auditoria.ActualizoStock)
            {
                _context.Auditoria.Add(auditoria);



                foreach (var prod in auditoria.Productos)
                {
                    if (prod.Stock != 0)
                    {
                        prod.Producto.Stock = prod.Stock;

                        if (prod.Producto.ProductosInventario.Count > 0 && prod.Producto.ProductosInventario != null)
                        {
                            foreach (var prodInv in prod.Producto.ProductosInventario)
                            {
                                prodInv.Stock = prod.Stock;
                            }
                        }
                    }
                }
            }
            else
            {
                _context.Auditoria.Add(auditoria);
            }
            _context.SaveChanges();
        }

        public void DeleteAuditoria(int auditoria)
        {
            var data = GetAuditoria(auditoria);
            data.Eliminada = true;
            _context.Auditoria.Update(data);
            _context.SaveChanges();
        }

        public List<Auditoria> GetAllAuditoria()
        {
            return _context.Auditoria
                .Include(x => x.Productos)
                    .ThenInclude(b => b.Producto)
                        .ThenInclude(c => c.ProductosInventario)
                .Include(x => x.User)
                    .ThenInclude(x => x.Persona)
                .Where(x => x.Eliminada == false)
                .ToList();
        }

        public Auditoria GetAuditoria(int id)
        {
            return _context.Auditoria.Where(x => x.Id == id)
                .Include(x => x.Productos)
                    .ThenInclude(b => b.Producto)
                        .ThenInclude(c => c.ProductosInventario)
                            .ThenInclude(x => x.UnidadMedidaCompra)
                .Include(x => x.Productos)
                    .ThenInclude(b => b.Producto)
                        .ThenInclude(c => c.ProductosInventario)
                            .ThenInclude(x => x.UnidadMedidaVenta)

                .Include(x => x.User)
                    .ThenInclude(x => x.Persona).FirstOrDefault();
        }

        public Auditoria GetDetalleAuditoria(int id, string codigo, string nombre, string unidadCompra, string unidadVenta)
        {
            var auditoria = GetAuditoria(id);
            List<AuditoriaProducto> auditoriaProductos = new List<AuditoriaProducto>();
            if (codigo != null)
            {
                auditoriaProductos = auditoria.Productos.Where(x => x.Producto.CodigoReferencia.ToLower().Trim() == codigo.ToLower().Trim()).ToList();
                auditoria.Productos = auditoriaProductos;
            }
            if (nombre != null)
            {
                auditoriaProductos = auditoria.Productos.Where(x => x.Producto.NombreProducto.ToLower().Trim().Contains(nombre.ToLower().Trim())).ToList();
                auditoria.Productos = auditoriaProductos;

            }
            if (unidadCompra != null)
            {
                foreach (var item in auditoriaProductos)
                {
                    var data = item.Producto.ProductosInventario.FirstOrDefault().UnidadMedidaCompra.Nombre.ToLower().Trim() == unidadCompra.ToLower().Trim();
                    if (!data)
                    {
                        auditoriaProductos.Remove(item);
                    }
                }
                auditoria.Productos = auditoriaProductos;
            }
            if (unidadVenta != null)
            {
                foreach (var item in auditoriaProductos)
                {
                    var data = item.Producto.ProductosInventario.FirstOrDefault().UnidadMedidaVenta.Nombre.ToLower().Trim() == unidadVenta.ToLower().Trim();
                    if (!data)
                    {
                        auditoriaProductos.Remove(item);
                    }
                }
                auditoria.Productos = auditoriaProductos;
            }

            return auditoria;
        }

        public void UpdateAuditoria(Auditoria auditoria)
        {
            _context.Auditoria.Update(auditoria);
            _context.SaveChanges();
        }
    }
}

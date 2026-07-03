using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface IAuditoria
    {
        void AddAuditoriaSp(object[] productos, string userId, string personaCreaAuditoria, bool actualizoStock);
        void AddAuditoria(Auditoria auditoria, bool saveChanges = true);
        List<Auditoria> GetAllAuditoria();
        Auditoria GetAuditoria(int id);
        Auditoria GetDetalleAuditoria(int id, string codigo, string nombre, string unidadCompra, string unidadVenta);
        void DeleteAuditoria(int auditoria);
        void UpdateAuditoria(Auditoria auditoria);

    }
}

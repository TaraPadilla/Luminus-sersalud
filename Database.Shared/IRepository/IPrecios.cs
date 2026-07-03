using Database.Shared.Models;
using System.Collections.Generic;
using Database.Shared.Paginacion;
using System;
using System.Linq;
using Database.Shared.DataBindings;

namespace Database.Shared.IRepository
{
    public interface IPrecios
    {
        IList<Precio> GetList();
        decimal? ObtenerPrecioPorSeguro(string tipo, string nombre, string seguro);
        IList<ExamenLabClinicoPrecio> GetPreciosExamenLabClinico(int examenLabClinicoId);
        void Add(Precio precio);
        void Update(Precio precio);
        Precio Get(int precioId);
        void Delete(int precioId);
        Precio GetByName(string nombrePrecio);
    }
}
using Database.Shared.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Database.Shared.IRepository
{
    public interface ITipoCuenta
    {
        public List<TipoCuenta> GetAll();
    }
}

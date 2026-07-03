using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;

namespace sistema.Models
{
    public class GrupoViewModel 
    {
        public Grupo Grupo {get;set;} = new Grupo();
      
        public int Id
        {
            get { return Grupo.Id; }
            set { Grupo.Id = value; }
        }
    }
} 
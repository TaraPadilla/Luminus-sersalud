using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using System;

namespace Database.Shared.Models
{
    public class User : IdentityUser
    {
        public User()
        {
            Envios = new List<Envio>();
            ResponsableEnviados = new List<TrasladosProductos>();
            ResponsableRecibidos = new List<TrasladosProductos>();
            Citas = new List<Cita>();
        }
        public static object Identity {get; internal set;}
        public int? EmpleadoId {get;set;}
        public Empleado Persona {get;set;}
        public ICollection<Envio> Envios { get; set; }

        [InverseProperty("ResponsableEnviado")]
        public virtual ICollection<TrasladosProductos> ResponsableEnviados { get; set; }

        [InverseProperty("ResponsableRecibido")]
        public virtual ICollection<TrasladosProductos> ResponsableRecibidos { get; set; }

        [InverseProperty("ResponsableApertura")]
        public virtual ICollection<Caja> ResponsableApertura { get; set; }

        [InverseProperty("ResponsableCierre")]
        public virtual ICollection<Caja> ResponsableCierre { get; set; }

        //[InverseProperty("ResponsableAperturaLab")]
        //public virtual ICollection<CajaLab> ResponsableAperturaLab { get; set; }

        //[InverseProperty("ResponsableCierreLab")]
        //public virtual ICollection<CajaLab> ResponsableCierreLab { get; set; }


        public ICollection<Cita> Citas { get; set; }

        public static implicit operator User(Task<User> v)
        {
            throw new NotImplementedException();
        }
    }
}
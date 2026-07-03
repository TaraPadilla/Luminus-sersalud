using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Shared.Models
{
    public class Empleado
    {
        public Empleado()
        {
            CalendarioFechasBloqueadas = new List<CalendarioFechaBloqueada>();
            Users = new List<User>();
            Ventas = new List<Venta>();
            Emergencias = new List<Emergencia>();
            //VentasServicios = new List<VentaServicio>();
            Compras = new List<Compra>();
            Citas = new List<Citas>();
            Examens = new List<Examen>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Telefono { get; set; }

        public string Telefono_2 { get; set; }

        public string Email { get; set; }

        public string Direccion { get; set; }

        public string Edad { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Dpi { get; set; }

        [Required(ErrorMessage = "* Este campo es obligatorio.")]
        public string Nit { get; set; }
        public string EstadoCivil { get; set; }
        public string TipoContrato { get; set; }
        public string Salario { get; set; }
        public DateTime FechaInicioLabores { get; set; }

        public DateTime VacacionesProgramadas { get; set; }
        public DateTime VacacionesProgramadasFinal { get; set; }

        public string JornadaTrabajo { get; set; }

        public string Observaciones { get; set; }
        public string ColorHexadecimalFondo { get; set; }
        public string ColorHexadecimalTexto { get; set; }
        public string Imagen { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal Sucursal { get; set; }
        public bool Eliminado { get; set; }
        public string TipoEmpleado { get; set; }
        // Nuevos campos para medicos
        public string Colegiado { get; set; }
        public string Genero { get; set; }
        public string Residente { get; set; }
        public string Credenciales { get; set; }
        public string DireccionClinica { get; set; }
        public string TelefonoClinica { get; set; }
        public string TipoBanco { get; set; }
        public string TipoCuenta { get; set; }
        public string NumeroCuenta { get; set; }
        public string NombreCuenta { get; set; }
        public string NitPropietarioCuenta { get; set; }
        public string NombrePropietarioNit { get; set; }
        public string TipoRegimen { get; set; }

        public ICollection<CalendarioFechaBloqueada> CalendarioFechasBloqueadas { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Venta> Ventas { get; set; }
        public ICollection<Emergencia> Emergencias { get; set; }
        //public ICollection<VentaServicio> VentasServicios { get; set; }
        public ICollection<Compra> Compras { get; set; }
        public ICollection<Citas> Citas { get; set; }
        public ICollection<Examen> Examens { get; set; }
        public string NombreYApellidos
        {
            get { return $"{Nombre} {Apellido}"; }
        }

        public string FirmaEmpleado { get; set; }

        // Nueva propiedad: ID de la especialidad asignada
        public int? EspecialidadId { get; set; }

        // Relación opcional con el modelo de Especialidad
        public Especialidad Especialidad { get; set; }

        // =============================
        // Ubicación organizacional
        // =============================

        // Unidad a la que pertenece el empleado (nullable para no romper data existente)
        public int? UnidadOrgId  { get; set; }

        [ForeignKey("UnidadOrgId")]
        public virtual UnidadOrg UnidadOrg { get; set; }

        // Sección dentro de la unidad (nullable y opcional)
        public int? SeccionOrgId  { get; set; }

        // Navegaciones (cuando existan las entidades)
        // public Unidad Unidad { get; set; }
        // public Seccion Seccion { get; set; }


    }

}
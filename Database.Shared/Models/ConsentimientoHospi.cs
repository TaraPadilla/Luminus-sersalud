using System;
using System.Collections.Generic;

namespace Database.Shared.Models
{
    public class ConsentimientoHospi
    {
        // Propiedades principales
        public int Id { get; set; } // Identificador único para el registro

        // Relación con Paciente
        public int PacienteId { get; set; } // Clave foránea al modelo Paciente
        public Paciente Paciente { get; set; } // Navegación al modelo Paciente

        // Relación con Habitacion
        public int HabitacionId { get; set; } // Clave foránea al modelo Habitacion
        public Habitacion Habitacion { get; set; } // Navegación al modelo Habitacion

        // Dato de la Hospitalización
        public string HospitalizacionId { get; set; } // Id de la Hospitalizacion

        // Datos del Paciente
        public string HoraIngreso { get; set; } // Hora de ingreso
        public string NumeroPaciente { get; set; } // Número de paciente
        public string NumeroHabitacion { get; set; } // Número de habitación
        public string NombreCompleto { get; set; } // Nombre completo
        public string EstadoCivil { get; set; } // Estado civil
        public string DPI { get; set; } // DPI o pasaporte
        public string FechaNacimiento { get; set; } // Fecha de nacimiento
        public string Edad { get; set; } // Edad (calculada automáticamente en el front)
        public string Nacionalidad { get; set; } // Nacionalidad
        public string Direccion { get; set; } // Dirección
        public string Celular { get; set; } // Celular
        public string Email { get; set; } // Email
        public string TipoSangre { get; set; } // Tipo de sangre
        public string Genero { get; set; } // Género
        public string Religion { get; set; } // Religión
        public string Ocupacion { get; set; } // Ocupación

        // Información del seguro médico
        public string PoseeSeguroMedico { get; set; } // ¿Posee seguro médico?
        public string Aseguradora { get; set; } // Aseguradora (si aplica)
        public string TipoPoliza { get; set; } // Tipo de póliza (Individual o Empresarial)
        public string NombreEmpresa { get; set; } // Nombre de la empresa (si aplica)
        public string FormularioPreAutorizacion { get; set; } // ¿Tiene formulario de pre-autorización?
        public string TratamientoMedico { get; set; } // Tratamiento médico

        // Datos del Responsable de la Cuenta
        public string NombreResponsable { get; set; } // Nombre completo del responsable
        public string DPIResponsable { get; set; } // DPI o pasaporte del responsable
        public string EdadResponsable { get; set; } // Edad del responsable
        public string DireccionResponsable { get; set; } // Dirección del responsable
        public string CelularResponsable { get; set; } // Celular del responsable
        public string EmailResponsable { get; set; } // Email del responsable
        public string NITResponsable { get; set; } // NIT del responsable
        public string NombreFacturacion { get; set; } // Nombre para facturación
        public string NacionalidadResponsable { get; set; } // Nuevo campo
        public string OcupacionResponsable { get; set; } // Nuevo campo

        // Contacto de Emergencia
        // public string NombreContactoEmergencia { get; set; } // Nombre del contacto de emergencia
        // public string CelularContactoEmergencia { get; set; } // Celular del contacto de emergencia
        // public string ParentescoContactoEmergencia { get; set; } // Parentesco del contacto de emergencia
        public virtual ICollection<ContactoEmergencia> ContactosEmergencia { get; set; }
       = new List<ContactoEmergencia>();

        // Información Adicional
        public string HospitalProporcionoMedico { get; set; } // ¿El hospital proporcionó médico tratante?
        public string MedicoAfiliado { get; set; } // ¿Cuenta con médico tratante afiliado?
        public string NombreMedicoTratante { get; set; } // Nombre del médico tratante
        public string RecetaMedica { get; set; } // ¿Tiene receta médica por ingresos programados?

        // Firmas y nombres de quienes firman
        public string URLFirmaPaciente { get; set; } // Url para acceder a la firma del paciente
        public string URLFirmaResponsable { get; set; } // Url para acceder a la firma del responsable
        public string NombreNotaria { get; set; } // Nombre de la persona que funje como Notaria
        public string NombreRepresentanteNarajo { get; set; } // Nombre de la persona que firma por el Hospital Naranjo
        public string URLFirmaNotaria { get; set; } // Url para acceder a la firma de la notaria
        public string URLFirmaRepresentanteNaranjo { get; set; } // Url para acceder a la firma del representante del Hospital Naranjo
        public int? CitaId { get; set; }
        public int? ConsultaId { get; set; }
    }
}

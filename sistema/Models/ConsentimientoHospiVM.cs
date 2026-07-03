using System;
using System.Collections.Generic;

namespace farmamest.Models
{
    public class ConsentimientoHospiVM
    {
        // Propiedades principales
        public int Id { get; set; } // Identificador único para el registro

        // Relación con Paciente
        public int PacienteId { get; set; } // Clave foránea al modelo Paciente - Este dato esta en: paciente.Id 
        public string NombrePaciente { get; set; } // Nombre del paciente - Este dato esta en: paciente.Nombre 

        // Relación con Habitacion
        public int HabitacionId { get; set; } // Clave foránea al modelo Habitacion - Este dato esta en: habitacion.Id 
        public string NumeroHabitacion { get; set; } // Número de la habitación  - Este dato esta en: habitacion.NombreNumeroHabitacion  

        // Datos de la Hospitalización
        public string HospitalizacionId { get; set; } // Id de la Hospitalización

        // Datos del Paciente
        public string HoraIngreso { get; set; } // Hora de ingreso
        public string NumeroPaciente { get; set; } // Número de paciente - Este dato esta en: paciente.Id 
        public string NombreCompleto { get; set; } // Nombre completo - Este dato esta en: paciente.Nombre 
        public string EstadoCivil { get; set; } // Estado civil - Este dato esta en: paciente.EstadoCivil
        public string DPI { get; set; } // DPI o pasaporte - Este dato esta en: paciente.Dpi 
        public string FechaNacimiento { get; set; } // Fecha de nacimiento - Este dato esta en: paciente.FechaNacimiento 
        public string Edad { get; set; } // Edad (calculada automáticamente en el front) - Este dato esta en: 
        public string Nacionalidad { get; set; } // Nacionalidad - Este dato esta en:
        public string Direccion { get; set; } // Dirección - Este dato esta en: paciente.Direccion 
        public string Celular { get; set; } // Celular - Este dato esta en: paciente.Celular (Si celular es null o esta vacio, asigna el dato de: paciente.Telefono) 
        public string Email { get; set; } // Email - Este dato esta en: paciente.Email 
        public string TipoSangre { get; set; } // Tipo de sangre - Este dato esta en: paciente.TipoDeSangre 
        public string Genero { get; set; } // Género - Este dato esta en: paciente.sexoText 
        public string Religion { get; set; } // Religión - Este dato esta en: paciente.Religion 
        public string Ocupacion { get; set; } // Ocupación - Este dato esta en: paciente.Ocupacion 

        // Información del seguro médico
        public string PoseeSeguroMedico { get; set; } // ¿Posee seguro médico? - Este dato esta en:
        public string Aseguradora { get; set; } // Aseguradora (si aplica) - Este dato esta en:
        public string TipoPoliza { get; set; } // Tipo de póliza (Individual o Empresarial) - Este dato esta en:
        public string NombreEmpresa { get; set; } // Nombre de la empresa (si aplica) - Este dato esta en:
        public string FormularioPreAutorizacion { get; set; } // ¿Tiene formulario de pre-autorización? - Este dato esta en:
        public string TratamientoMedico { get; set; } // Tratamiento médico - Este dato esta en:

        // Datos del Responsable de la Cuenta
        public string NombreResponsable { get; set; } // Nombre completo del responsable - Este dato esta en: cita.ResponsableNombre 
        public string DPIResponsable { get; set; } // DPI o pasaporte del responsable - Este dato esta en: cita.ResponsableDPI 
        public string EdadResponsable { get; set; } // Edad del responsable - Este dato esta en: 
        public string DireccionResponsable { get; set; } // Dirección del responsable - Este dato esta en: cita.ResponsableDireccion
        public string CelularResponsable { get; set; } // Celular del responsable - Este dato esta en: cita.ResponsableTelefono
        public string EmailResponsable { get; set; } // Email del responsable - Este dato esta en: 
        public string NITResponsable { get; set; } // NIT del responsable - Este dato esta en: cita.ResponsableNit 
        public string NombreFacturacion { get; set; } // Nombre para facturación - Este dato esta en: cita.ResponsableNombre
        public string NacionalidadResponsable { get; set; } // Nuevo campo
        public string OcupacionResponsable { get; set; } // Nuevo campo


        // Contacto de Emergencia
        // public string NombreContactoEmergencia { get; set; } // Nombre del contacto de emergencia - Este dato esta en: cita.AcompananteNombre 
        // public string CelularContactoEmergencia { get; set; } // Celular del contacto de emergencia - Este dato esta en: cita.AcompananteTelefono
        // public string ParentescoContactoEmergencia { get; set; } // Parentesco del contacto de emergencia - Este dato esta en: 
        public List<ContactoEmergenciaVM> ContactosEmergencia { get; set; }

        public class ContactoEmergenciaVM
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Telefono { get; set; }
            public string Parentesco { get; set; }
        }

        // Información Adicional
        public string HospitalProporcionoMedico { get; set; } // ¿El hospital proporcionó médico tratante? - Este dato esta en:
        public string MedicoAfiliado { get; set; } // ¿Cuenta con médico tratante afiliado? - Este dato esta en:
        public string NombreMedicoTratante { get; set; } // Nombre del médico tratante - Este dato esta en:
        public string RecetaMedica { get; set; } // ¿Tiene receta médica por ingresos programados? - Este dato esta en:

        // Firmas y nombres de quienes firman
        public string URLFirmaPaciente { get; set; } // Url para acceder a la firma del paciente
        public string URLFirmaResponsable { get; set; } // Url para acceder a la firma del responsable
        public string NombreNotaria { get; set; } // Nombre de la persona que funje como Notaria
        public string NombreRepresentanteNarajo { get; set; } // Nombre de la persona que firma por el Hospital Naranjo
        public string URLFirmaNotaria { get; set; } // Url para acceder a la firma de la notaria
        public string URLFirmaRepresentanteNaranjo { get; set; } // Url para acceder a la firma del representante del Hospital Naranjo

        // NUEVOS CAMPOS: Relación con la Cita
        public int? CitaId { get; set; }
        public int? ConsultaId { get; set; }

        // NUEVOS CAMPOS PARA EL PDF DEL MÉDICO
        public string EspecialidadMedico { get; set; }
        public string ColegiadoMedico { get; set; }
        public string UrlFirmaMedico { get; set; }

        public string ProcedimientoProgramado { get; set; }

        // NUEVOS CAMPOS PARA SALA DE OPERACIONES
        public string FechaAdmision { get; set; }

        public string NombrePrimerAyudante { get; set; }
        public string ColegiadoPrimerAyudante { get; set; }

        public string NombreSegundoAyudante { get; set; }
        public string ColegiadoSegundoAyudante { get; set; }

        public string NombreAnestesista { get; set; }
        public string ColegiadoAnestesista { get; set; }

        public List<MedicamentoNoControladoPdfVM> MedicamentosNoControlados { get; set; } = new();

        public string UrlFirmaAnestesista { get; set; }


    }
}

using Database.Shared.Data;
using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace farmamest.Service
{
    public class HospitalizacionService : IHospitalizacionService
    {
        private readonly IHospitalizacion _hospitalizacionRepository;
        private readonly IHabitacion _habitacionRepository;
        private readonly IEmpleado _empleadoRepository;
        private readonly IPacientes _pacientesRepository;
        private readonly IConsultas _consultasRepository;
        private readonly IUser _userRepository;
        public HospitalizacionService(
            IHospitalizacion hospitalizacionRepository,
            IHabitacion habitacionRepository,
            IEmpleado empleadoRepository,
            IUser userRepository,
            IConsultas consultasRepository,
            IPacientes pacientesRepository)
        {
            _hospitalizacionRepository = hospitalizacionRepository;
            _habitacionRepository = habitacionRepository;
            _consultasRepository = consultasRepository;
            _pacientesRepository = pacientesRepository;
            _userRepository = userRepository;
            _empleadoRepository = empleadoRepository;
        }
        public List<PacientesBaseViewModel> GetPacientesExistentes()
        {
            var listaPacientes = new List<PacientesBaseViewModel>();
            var pacientes = _pacientesRepository.GetList();
            if (pacientes != null && pacientes.Count > 0)
            {
                foreach (var paciente in pacientes)
                {
                    listaPacientes.Add(new PacientesBaseViewModel
                    {
                        Nombre = paciente.PacienteWithDPI,
                    });
                }
            }
            return listaPacientes;
        }
        public List<HospitalizacionViewModel> GetListaHospitalizaciones()
        {
            var listaHospitalizaciones = new List<HospitalizacionViewModel>();

            var hospitalizacionesBd = _hospitalizacionRepository.GetHospitalizaciones();
            if (hospitalizacionesBd != null)
            {
                foreach (var hospitalizacion in hospitalizacionesBd.OrderByDescending(h => h.Id)) // Ordenar por ID descendente
                {
                    var especialidad = hospitalizacion.Especialidad ?? new Especialidad();
                    var paciente = hospitalizacion.Paciente ?? new Paciente();
                    var habitacion = hospitalizacion.Habitacion ?? new Habitacion();
                    var consultas = hospitalizacion.Consultas;
                    var consulta = (consultas != null && consultas.Count > 0)
                        ? consultas.FirstOrDefault()
                        : null;
                    int? consultaId = consulta?.Id;

                    listaHospitalizaciones.Add(new HospitalizacionViewModel
                    {
                        HospitalizacionId = hospitalizacion.Id,
                        PacienteNombre = paciente.Nombre,
                        ConsultaId = consultaId,
                        PacienteDpi = paciente.Dpi,
                        Periodo = hospitalizacion.FechaInicio + " - " + hospitalizacion.FechaFin,
                        EspecialidadNombre = especialidad.NombreEspecialidad,
                        HabitacionNumeroNombre = habitacion.NombreNumeroHabitacion,
                        HospitalizacionEstado = hospitalizacion.Finalizada ? "Finalizada" : "En curso"
                    });
                }
            }

            return listaHospitalizaciones;
        }

        public List<HospitalizacionHabitacionViewModel> GetHabitaciones(bool disponibles, bool ocupadas)
        {
            var habitacionesConsultadas = new List<HospitalizacionHabitacionViewModel>();
            var habitaciones = _habitacionRepository.GetHabitaciones();

            if (habitaciones != null)
            {
                foreach (var habitacion in habitaciones)
                {
                    var ocupante = "-";
                    if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
                    {
                        var paciente = _habitacionRepository.GetPacienteOcupante(habitacion.Id);
                        ocupante = paciente != null ? paciente.Nombre : "-";
                    }
                    int? hospitalizacionId = null;
                    var hospitalizacionActualId = _habitacionRepository.GetHospitalizacionActual(habitacion.Id) == null
                        ? 0
                        : _habitacionRepository.GetHospitalizacionActual(habitacion.Id).Id;

                    if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
                    {
                        hospitalizacionId = hospitalizacionActualId;
                    }
                    if ((disponibles && habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Disponible)
                        || (ocupadas && habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada))
                    {
                        habitacionesConsultadas.Add(new HospitalizacionHabitacionViewModel
                        {
                            HabitacionId = habitacion.Id,
                            HospitalizacionId = hospitalizacionId,
                            HabitacionNombre = habitacion.NombreNumeroHabitacion,
                            HabitacionCategoria = habitacion.CategoriaHabitacion.NombreCategoria,
                            HabitacionEstadoId = habitacion.EstadoHabitacionId,
                            HabitacionEstado = habitacion.EstadoHabitacion.NombreEstado,
                            HabitacionOcupante = ocupante,
                            HabitacionNumeroCamas = habitacion.NumeroCamas,
                            HabitacionCapacidadPersonas = habitacion.CapacidadPersonas
                        });
                    }
                }
            }
            return habitacionesConsultadas;
        }
        public List<HospitalizacionServicioViewModel> GetServiciosHospitalizacion(int hospitalizacionId)
        {
            // Creación de lista que se va a retornar
            var listaServicios = new List<HospitalizacionServicioViewModel>();

            // Console.WriteLine("Iniciando método GetServiciosHospitalizacion...");

            var hospitalizacion = _hospitalizacionRepository.Get(hospitalizacionId, false, true, false, false, true);
            // Console.WriteLine($"Hospitalización obtenida para ID {hospitalizacionId}: {(hospitalizacion != null ? "OK" : "NULL")}");

            // Servicios de hospitalización
            if (hospitalizacion?.HospitalizacionesServicios != null)
            {
                // Console.WriteLine($"Cantidad de servicios en hospitalización: {hospitalizacion.HospitalizacionesServicios.Count}");

                foreach (var servicioHospi in hospitalizacion.HospitalizacionesServicios)
                {
                    var servicioNombre = servicioHospi.Servicio?.NombreServicio ?? "-";
                    // Console.WriteLine($"Procesando servicio: {servicioNombre}");

                    var precioServicioValor = servicioHospi.PrecioServicio?.Valor ?? 0;
                    var persona = "-";

                    if (servicioHospi.UsuarioAplica != null)
                    {
                        // Console.WriteLine($"Usuario aplica encontrado: {servicioHospi.UsuarioAplica}");

                        var usuario = _userRepository.GetbyId(servicioHospi.UsuarioAplica);
                        // Console.WriteLine($"Usuario obtenido del repositorio: {(usuario != null ? "OK" : "NULL")}");

                        if (usuario?.EmpleadoId != null)
                        {
                            try
                            {
                                var empleadoId = usuario.EmpleadoId;
                                if (empleadoId.HasValue)
                                {
                                    // Console.WriteLine($"EmpleadoId encontrado: {empleadoId.Value}");
                                    // Console.WriteLine(_empleadoRepository == null ? "_empleadoRepository es NULL" : "_empleadoRepository NO es NULL");
                                    var empleado = _empleadoRepository.Get((int)empleadoId); // Conversión explícita
                                    // Console.WriteLine($"Empleado obtenido: {(empleado != null ? empleado.NombreYApellidos : "NULL")}");
                                    persona = empleado?.NombreYApellidos ?? "Empleado no encontrado";
                                }
                                else
                                {
                                    persona = "Admin";
                                    // Console.WriteLine("EmpleadoId es NULL.");
                                }
                            }
                            catch (Exception ex)
                            {
                                persona = "Error al obtener el empleado";
                                Console.WriteLine($"Error al obtener el empleado: {ex.Message}");
                            }
                        }
                        else
                        {
                            // Console.WriteLine("EmpleadoId no encontrado para el usuario.");
                            persona = "Admin";
                        }
                    }
                    else
                    {
                        // Console.WriteLine("Usuario aplica es NULL.");
                    }

                    var fechaAplicacion = servicioHospi.FechaHoraAplicacion.HasValue
                        ? servicioHospi.FechaHoraAplicacion.Value.ToString("dd/MM/yyyy HH:mm:ss")
                        : "-";

                    // if (servicioHospi.Aplicado)
                    // {
                    //     fechaAplicacion = servicioHospi.FechaHoraAplicacion.ToString();
                    // }

                    var usuarioCrea = "-";
                    if (servicioHospi.UsuarioCreaId != null)
                    {
                        var usuarioCreador = _userRepository.GetbyId(servicioHospi.UsuarioCreaId);
                        usuarioCrea = usuarioCreador?.NormalizedUserName ?? "Usuario no encontrado";
                        // Console.WriteLine($"Usuario creador obtenido: {usuarioCrea}");
                    }

                    listaServicios.Add(new HospitalizacionServicioViewModel
                    {
                        Id = servicioHospi.Id,
                        Nombre = servicioNombre,
                        Cantidad = servicioHospi.Cantidad,
                        Precio = precioServicioValor,
                        Subtotal = precioServicioValor * servicioHospi.Cantidad,
                        Aplicado = servicioHospi.Aplicado,
                        PersonaAplica = persona,
                        PersonaCrea = usuarioCrea,
                        FechaHoraAplicacion = fechaAplicacion
                    });

                    // Console.WriteLine($"Servicio procesado: {servicioNombre}, Persona aplica: {persona}");
                }
            }
            else
            {
                // Console.WriteLine("Hospitalización o servicios de hospitalización son NULL.");
            }

            // Servicios de consulta externa
            if (hospitalizacion?.Consultas != null && hospitalizacion.Consultas.Count > 0)
            {
                // Console.WriteLine($"Cantidad de consultas en hospitalización: {hospitalizacion.Consultas.Count}");

                var consulta = hospitalizacion.Consultas.FirstOrDefault();
                if (consulta?.ConsultasServicios != null)
                {
                    foreach (var servicioConsulta in consulta.ConsultasServicios)
                    {
                        var servicio = servicioConsulta.Servicio ?? new Servicio();
                        listaServicios.Add(new HospitalizacionServicioViewModel
                        {
                            Id = servicioConsulta.Id,
                            Nombre = servicio.NombreServicio,
                            Cantidad = servicioConsulta.Cantidad,
                            Precio = servicioConsulta.PrecioValor ?? 0,
                            Subtotal = (servicioConsulta.PrecioValor ?? 0) * servicioConsulta.Cantidad,
                            Aplicado = true,
                            AgregadoConsulta = true
                        });
                    }
                }
            }

            // Console.WriteLine("Finalizando método GetServiciosHospitalizacion...");
            return listaServicios;
        }

    }
}

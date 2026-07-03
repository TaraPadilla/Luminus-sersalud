using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Database.Shared;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using sistema.Models;
using Database.Shared.IRepository;
using Database.Shared.Data;
using Database.Shared.Models;
using System.Web;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using sistema.Json;
using System.Net;
using Database.Shared.Paginacion;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using sistema.Service.IService;
using Database.Shared.Enumeraciones;
using farmamest.Service.IService;
using farmamest.Service;

namespace sistema.Controllers
{
    [Authorize]
    public class EmergenciasController : Controller
    {

        private readonly ICotizacion _cotizacionRepository = null;
        private readonly IProducto _productoRepository = null;
        private readonly IServicio _servicioRepository = null;
        private readonly IVenta _ventaRepository = null;
        //private readonly IVentaServicio _ventaServicioRepository = null;
        private readonly ICliente _clienteRepository = null;
        private readonly IPacientes _pacientesRepository = null;
        private readonly IEmpleado _empleadoRepository = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IEnvio _envioRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        //private readonly ICajaClinica _cajaClinicaRepository = null;

        //Service (logica de negocio)
        private readonly IPacientesService _pacientesService = null;
        private readonly IEmergenciaService _emergenciaService = null;
        private readonly IProductosService _productosService = null;
        private readonly IHospitalizacionService _hospitalizacionService = null;



        public EmergenciasController(ICotizacion cotizacionRepository,
            IProducto productoRepository,
            IVenta ventaRepository,
            IPacientes pacientesRepository,
            ICliente clienteRepository,
            IEmpleado empleadoRepository,
            ICaja cajaRepository,
            IEnvio envioRepository,
            IServicio servicioRepository,
            ISucursal sucursalRepository,

            //Servicio (logica de negocio)
            IPacientesService pacientesService,
            IHospitalizacionService hospitalizacionService,
            IEmergenciaService emergenciaService,
            IProductosService productosService)
        {
            _cotizacionRepository = cotizacionRepository;
            _productoRepository = productoRepository;
            _ventaRepository = ventaRepository;
            _clienteRepository = clienteRepository;
            _pacientesRepository = pacientesRepository;
            _empleadoRepository = empleadoRepository;
            _cajaRepository = cajaRepository;
            _envioRepository = envioRepository;
            _servicioRepository = servicioRepository;
            _sucursalRepository = sucursalRepository;

            //Servicio (logica de negocio)
            _pacientesService = pacientesService;
            _emergenciaService = emergenciaService;
            _productosService = productosService;
            _hospitalizacionService = hospitalizacionService;
        }

        //SE CREÓ UN NUEVO CONTROLLER EN BASE A LA BUSQUEDA DE HABITACIONES PARA EMERGENCIAS MAS SU FUNCIONALIDAD 
        //TAMBIEN ES EN BASE A LA DE HOSPITALIZACIONCONTROLLER  EL CONTROLER SE LLAMA EmergenciaController

        // public IActionResult HabitacionesEmergencias()
        // {
        //     var habitacionesConsultadas = new List<HospitalizacionHabitacionViewModel>();
        //     var habitaciones = _habitacionRepository.GetHabitaciones();
        //     if (habitaciones != null)
        //     {
        //         foreach (var habitacion in habitaciones)
        //         {
        //             var ocupante = "-";
        //             if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
        //             {
        //                 var paciente = _habitacionRepository.GetPacienteOcupante(habitacion.Id);
        //                 ocupante = paciente != null ? paciente.Nombre : "-";
        //             }
        //             int? hospitalizacionId = null;
        //             var hospitalizacionActualId = _habitacionRepository.GetHospitalizacionActual(habitacion.Id) == null ? 0 : _habitacionRepository.GetHospitalizacionActual(habitacion.Id).Id;

        //             if (habitacion.EstadoHabitacionId == (int)EstadoHabitacionEnum.Ocupada)
        //             {
        //                 hospitalizacionId = hospitalizacionActualId;
        //             }
        //             habitacionesConsultadas.Add(new HospitalizacionHabitacionViewModel
        //             {
        //                 HabitacionId = habitacion.Id,
        //                 HospitalizacionId = hospitalizacionId,
        //                 HabitacionNombre = habitacion.NombreNumeroHabitacion,
        //                 HabitacionCategoria = habitacion.CategoriaHabitacion.NombreCategoria,
        //                 HabitacionEstadoId = habitacion.EstadoHabitacionId,
        //                 HabitacionEstado = habitacion.EstadoHabitacion.NombreEstado,
        //                 HabitacionOcupante = ocupante,
        //                 HabitacionNumeroCamas = habitacion.NumeroCamas,
        //                 HabitacionCapacidadPersonas = habitacion.CapacidadPersonas
        //             });
        //         }
        //     }
        //     return View(habitacionesConsultadas);
        // }

        // // en caso de funcionar la hospitalizacion


        public IActionResult Nueva()
        {
            var model = new EmergenciaViewModel();
            model.HabilitarEdicion = true;
            model.Init(_sucursalRepository);
            return View(model);
        }
        public IActionResult Lista(bool ingresadas)
        {
            ViewData["Estado"] = ingresadas ? "Ingresadas" : "No ingresadas";
            var model = new EmergenciasListaVM
            {
                Ingresadas = ingresadas
            };
            return View(model);
        }
        public IActionResult Detalles(int emergenciaId)
        {
            var emergencia = _emergenciaService.Get(emergenciaId, true);
            emergencia.HabilitarEdicion = false;
            emergencia.Init(_sucursalRepository);
            return View(emergencia);
        }
        public IActionResult Editar(int emergenciaId)
        {
            var emergencia = _emergenciaService.Get(emergenciaId, true);
            emergencia.HabilitarEdicion = true;
            emergencia.Init(_sucursalRepository);
            return View(emergencia);
        }
        [HttpPost]
        public string GuardarEmergencia(EmergenciaViewModel model)
        {
            try
            {
                _emergenciaService.RegistrarEmergencia(model);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al registrar venta: " + ex.Message
                });
            }

        }
        [HttpPost]
        public string EditarEmergencia(EmergenciaViewModel model)
        {
            try
            {
                _emergenciaService.EditarEmergencia(model);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al editar emergencia: " + ex.Message
                });
            }

        }
        [HttpPost]
        public string ConsultarProductosExistentes()
        {
            try
            {
                int bodegaId = 10;

                // var inventario = _productosService.GetInventarioEmergencias(tipoProductoId,null,bodegaId);
                var inventario = _productosService.GetInventario(null, bodegaId);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = inventario
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar productos existentes. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarPacientes()
        {

            try
            {
                var lista = _pacientesService.GetPacientesExistentes();

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = lista
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar los pacientes. " + ex.Message
                });
            }
        }




        [HttpPost]
        public string ObtenerPacientePorEmergencia(int emergenciaId)
        {
            try
            {
                var emergencia = _emergenciaService.Get(emergenciaId, false);

                if (emergencia == null || emergencia.PacienteId == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontró el paciente para esta emergencia"
                    });
                }

                var paciente = _pacientesRepository.Get((int)emergencia.PacienteId);

                if (paciente == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "No se encontraron los datos del paciente"
                    });
                }

                var resultado = new[]
                {
            new
            {
                PacienteId = paciente.Id,
                Nombre = paciente.Nombre,
                Nit = paciente.Nit,
                Direccion = paciente.Direccion,
                //Correo = paciente.Correo,
                FechaNacimiento = paciente.FechaNacimiento?.ToString("yyyy-MM-ddTHH:mm:ss")
            }
        };

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = resultado
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al obtener paciente: " + ex.Message
                });
            }
        }


        [HttpPost]
        public string ConsultarHabitacionesDisponibles()
        {
            try
            {
                var habitacionesDisponibles = _hospitalizacionService.GetHabitaciones(true, false);

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = habitacionesDisponibles
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar habitaciones disponibles: " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarListaEmergencias(bool ingresadas)
        {
            try
            {
                var emergencias = _emergenciaService.GetEmergencias(ingresadas);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = emergencias
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar emergencias. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarElementosEmergencia(int emergenciaId)
        {
            try
            {
                var emergencia = _emergenciaService.Get(emergenciaId, false, true);
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = emergencia
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error de servidor al consultar elementos de emergencia. " + ex.Message
                });
            }
        }



        [HttpPost]
        public IActionResult AgregarProductoEmergencia([FromBody] DetalleEmergenciaViewModel data)
        {
            try
            {
                _emergenciaService.AgregarProducto(data);
                return Json(new { Exitoso = true, Mensaje = "Producto agregado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AgregarServicioEmergencia([FromBody] DetalleEmergenciaViewModel data)
        {
            try
            {
                _emergenciaService.AgregarServicio(data);
                return Json(new { Exitoso = true, Mensaje = "Servicio agregado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error: " + ex.Message });
            }
        }

        [HttpPost]
        public IActionResult AgregarExamenEmergencia([FromBody] DetalleEmergenciaViewModel data)
        {
            try
            {
                _emergenciaService.AgregarExamen(data);
                return Json(new { Exitoso = true, Mensaje = "Examen agregado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error: " + ex.Message });
            }
        }


        [HttpPost]
        public IActionResult EliminarDetalleEmergencia(int detalleId)
        {
            try
            {
                _emergenciaService.EliminarDetalle(detalleId);
                return Json(new { exitoso = true, mensaje = "Elemento eliminado correctamente" });
            }
            catch (Exception ex)
            {
                return Json(new { exitoso = false, mensaje = "Error al eliminar: " + ex.Message });
            }
        }
    }
}
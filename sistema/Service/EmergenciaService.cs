using Database.Shared.Enumeraciones;
using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Identity;
using sistema.Models;
using sistema.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Service
{
    public class EmergenciaService : IEmergenciaService
    {
        private readonly UserManager<User> _userManager = null;
        private readonly IUser _userRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly ICuentasPorCobrar _cuentasCobrarRepo = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IEmergencias _emergenciaRepository = null;
        private readonly IPacientes _pacientesRepository = null;
        private readonly ILaboratorioClinico _laboratorioRepository = null;

        //SERVICIO
        private readonly IPacientesService _pacientesService = null;

        public EmergenciaService(
            ISucursal sucursalRepository,
            ICuentasPorCobrar cuentasCobrarRepo,
            ICaja cajaRepository,
            IUser userRepository,
            IEmergencias emergenciaRepository,
            IPacientes pacientesRepository,
            ILaboratorioClinico laboratorioRepository,
            UserManager<User> userManager,
            //SERVICIO
            IPacientesService pacientesService)
        {
            _sucursalRepository = sucursalRepository;
            _cajaRepository = cajaRepository;
            _cuentasCobrarRepo = cuentasCobrarRepo;
            _userManager = userManager;
            _userRepository = userRepository;
            _emergenciaRepository = emergenciaRepository;
            _pacientesRepository = pacientesRepository;
            _laboratorioRepository = laboratorioRepository;

            //SERVICIO
            _pacientesService = pacientesService;
        }

        public void RegistrarEmergencia(EmergenciaViewModel model)
        {
            var fechaHora = DateTime.Now;

            //Paciente
            var pacienteVerificado = _pacientesService.ValidarExistenciaPaciente(new PacientesBaseViewModel
            {
                PacienteId = model.PacienteId ?? 0,
                Nombre = model.PacienteNombre,
                Direccion = model.PacienteDireccion,
                Nit = model.PacienteNit
            });
            model.PacienteId = pacienteVerificado.PacienteId;

            #region CREAR OBJETO EMERGENCIA

            var emergencia = new Emergencia
            {
                PacienteId = model.PacienteId,
                EmpleadoId = model.CodigoVendedor,
                FechaEmergencia = fechaHora,
                EmergenciaDetalles = new List<EmergenciaDetalle>(),
                Responsable = model.Responsable
            };

            #endregion

            #region PRODUCTOS

            if (model.Productos != null)
            {
                foreach (var producto in model.Productos)
                {
                    if (!producto.Eliminado)
                    {
                        emergencia.EmergenciaDetalles.Add(new EmergenciaDetalle
                        {
                            ProductoId = producto.ProductoId,
                            Cantidad = producto.Cantidad,
                            UnidadMedidaVentaId = producto.UnidadMedidaVentaId,
                            PrecioId = producto.PrecioId,
                            PrecioValor = producto.ValorUnitario,
                            Subtotal = producto.ValorUnitario * producto.Cantidad,
                            Total = producto.ValorUnitario * producto.Cantidad,
                            DescuentoPorcentaje = Convert.ToDecimal(producto.DescuentoPorcentaje),
                            Descuento = producto.DescuentoValor
                        });
                    }
                }
            }

            #endregion

            #region SERVICIOS

            if (model.Servicios != null)
            {
                foreach (var servicio in model.Servicios)
                {
                    emergencia.EmergenciaDetalles.Add(new EmergenciaDetalle
                    {
                        ServicioId = servicio.ServicioId,
                        Cantidad = servicio.Cantidad,
                        PrecioId = servicio.PrecioId,
                        PrecioValor = servicio.ValorUnitario,
                        Subtotal = servicio.ValorUnitario * servicio.Cantidad,
                        Total = servicio.ValorUnitario * servicio.Cantidad,
                        Descuento = servicio.DescuentoValor,
                        DescuentoPorcentaje = servicio.DescuentoPorcentaje,
                    });
                }
            }

            #endregion

            #region EXAMENES

            if (model.Examenes != null)
            {
                foreach (var examen in model.Examenes)
                {
                    if (!examen.Eliminado)
                    {
                        emergencia.EmergenciaDetalles.Add(new EmergenciaDetalle
                        {
                            ExamenLabClinicoId = examen.ExamenId,
                            Cantidad = examen.Cantidad,
                            PrecioValor = examen.ValorUnitario,
                            PrecioId = examen.PrecioId,
                            Subtotal = examen.ValorUnitario * examen.Cantidad,
                            Total = examen.ValorUnitario * examen.Cantidad,
                            Descuento = examen.DescuentoValor,
                            DescuentoPorcentaje = examen.DescuentoPorcentaje
                        });
                    }
                }
            }

            #endregion

            #region AGREGAR A CUENTA POR COBRAR

            var cuentaPendientePaciente = _cuentasCobrarRepo.GetUltimaCuentaPendientePaciente((int)model.PacienteId);
            if (cuentaPendientePaciente == null)
            {
                //Si no hay ninguna cuenta pendiente
                //se debe crear una nueva
            }
            else
            {
                //SI hay una se debe agregar el costo a dicha cuenta
            }

            #endregion

            //Registro de emergencia en BD
            _emergenciaRepository.Add(emergencia);

            #region CREACION DE EXAMEN PARA CARGAR RESULTADOS

            if (model.Examenes != null && model.Examenes.Any(a => !a.Eliminado))
            {
                var examenClinico = new Examen
                {
                    PacienteId = model.PacienteId,
                    FechaRealizacion = fechaHora,
                    EstadoExamenId = (int)EstadoExamenEnum.Solictiado,
                    DetalleExamenes = new List<DetalleExamen>()
                };
                foreach (var examen in model.Examenes)
                {
                    if (!examen.Eliminado && examen.ExamenId != null)
                    {
                        var detalleExamen = new DetalleExamen
                        {
                            ExamenLabClinicoId = (int)examen.ExamenId,
                            Cantidad = examen.Cantidad,
                            PrecioId = examen.PrecioId,
                            PrecioValor = examen.ValorUnitario,
                            Resultados = new List<Resultados>()
                        };
                        var datos = _laboratorioRepository.DatosLabList((int)examen.ExamenId);
                        foreach (var dato in datos)
                        {
                            var newDato = new Resultados
                            {
                                DatosExamenesLabClinico = dato
                            };
                            detalleExamen.Resultados.Add(newDato);
                        }
                        examenClinico.DetalleExamenes.Add(detalleExamen);
                    }
                }
                _laboratorioRepository.Add(examenClinico);
            }

            #endregion
        }
        public List<EmergenciaViewModel> GetEmergencias(bool ingresadas)
        {
            var listaEmergencias = new List<EmergenciaViewModel>();
            var emergenciasBd = _emergenciaRepository.GetEmergencias(ingresadas);
            if (emergenciasBd != null)
            {
                foreach (var emergencia in emergenciasBd)
                {
                    var paciente = emergencia.Paciente ?? new Paciente();
                    var emergenciaAgregada = new EmergenciaViewModel
                    {
                        EmergenciaId = emergencia.Id,
                        CodigoVendedor = emergencia.EmpleadoId,
                        PacienteId = emergencia.PacienteId,
                        SucursalId = emergencia.SucursalId,
                        EmergenciasFechaRegistro = emergencia.FechaEmergencia.ToString("dd-MM-yyyy"),
                        PacienteNombre = paciente.Nombre,
                        PacienteNit = paciente.Nit,
                        //EmergenciaValorTotal = emergencia.EmergenciaDetalles.Sum(a => a.Total),
                        // EmergenciaValorTotal = emergencia.EmergenciaDetalles.Sum(a => Math.Round((decimal)a.Total, 2)).ToString("0.00"),
                        EmergenciaValorTotal = emergencia.EmergenciaDetalles.Sum(a => a.Total).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture),
                        PacienteDireccion = paciente.Direccion,
                        Servicios = new List<EmergenciaServicioAgregadoViewModel>(),
                        Productos = new List<EmergenciaProductoAgregadoViewModel>(),
                        Examenes = new List<EmergenciaExamenAgregadoViewModel>(),
                        Pagado = emergencia.Pagado
                    };
                    //PRODUCTOS
                    if (emergencia.EmergenciaDetalles != null
                        && emergencia.EmergenciaDetalles.Any(a => a.ProductoId != null))
                    {
                        var productos = emergencia.EmergenciaDetalles.Where(a => a.ProductoId != null).ToList();
                        foreach (var producto in productos)
                        {
                            emergenciaAgregada.Productos.Add(new EmergenciaProductoAgregadoViewModel
                            {
                                ProductoId = producto.ProductoId
                            });
                        }
                    }

                    //SERVICIOS
                    if (emergencia.EmergenciaDetalles != null
                        && emergencia.EmergenciaDetalles.Any(a => a.ServicioId != null))
                    {
                        var servicios = emergencia.EmergenciaDetalles.Where(a => a.ServicioId != null).ToList();
                        foreach (var servicio in servicios)
                        {
                            emergenciaAgregada.Servicios.Add(new EmergenciaServicioAgregadoViewModel
                            {
                                ServicioId = servicio.ServicioId
                            });
                        }
                    }

                    //EXAMENES
                    if (emergencia.EmergenciaDetalles != null
                        && emergencia.EmergenciaDetalles.Any(a => a.ExamenLabClinicoId != null))
                    {
                        var examenes = emergencia.EmergenciaDetalles.Where(a => a.ExamenLabClinicoId != null).ToList();
                        foreach (var examen in examenes)
                        {
                            emergenciaAgregada.Examenes.Add(new EmergenciaExamenAgregadoViewModel
                            {
                                ExamenId = examen.ExamenLabClinicoId
                            });
                        }
                    }

                    listaEmergencias.Add(emergenciaAgregada);
                }
            }
            return listaEmergencias;
        }


        public int GetEmergenciaIdByHospitalizacion(int hospitalizacionId)
        {
            var id = _emergenciaRepository.GetIdByHospitalizacion(hospitalizacionId);

            if (id == null)
            {
                throw new Exception("No se encontró una emergencia vinculada a esta hospitalización.");
            }

            return id.Value;
        }

        public EmergenciaViewModel Get(int emergenciaId, bool includePaciente = false, bool includeElementos = false)
        {
            EmergenciaViewModel emergencia;

            var emergenciaBd = _emergenciaRepository.Get(emergenciaId, includePaciente, includeElementos);

            if (emergenciaBd == null)
            {
                return null;
            }
            else
            {
                var paciente = emergenciaBd.Paciente ?? new Paciente();
                var hospitalizacion = emergenciaBd.Hospitalizacion ?? new Hospitalizacion();
                var habitacion = hospitalizacion.Habitacion ?? new Habitacion();
                emergencia = new EmergenciaViewModel
                {
                    PacienteId = emergenciaBd.PacienteId,
                    PacienteNombre = paciente.Nombre,
                    Responsable = emergenciaBd.Responsable,
                    CodigoVendedor = emergenciaBd.EmpleadoId,
                    EmergenciaIngresada = emergenciaBd.Ingresada,
                    HospitalizacionId = emergenciaBd.HospitalizacionId,
                    HabitacionNombre = habitacion.NombreNumeroHabitacion,
                    PacienteNit = paciente.Nit,
                    PacienteDireccion = paciente.Direccion,
                    Productos = new List<EmergenciaProductoAgregadoViewModel>(),
                    Servicios = new List<EmergenciaServicioAgregadoViewModel>(),
                    Examenes = new List<EmergenciaExamenAgregadoViewModel>(),
                    Observaciones = emergenciaBd.Observaciones
                };

                #region ELEMENTOS AGREGADOS EMERGENCIA

                if (includeElementos)
                {
                    if (emergenciaBd.EmergenciaDetalles != null)
                    {
                        foreach (var elemento in emergenciaBd.EmergenciaDetalles)
                        {
                            if (elemento.ProductoId != null)
                            {
                                //Si es un producto
                                var producto = elemento.Producto ?? new Producto();
                                var precio = elemento.Precio ?? new Precio();
                                var unidadVenta = elemento.UnidadMedidaVenta ?? new UnidadMedidaVenta();
                                emergencia.Productos.Add(new EmergenciaProductoAgregadoViewModel
                                {
                                    Id = elemento.Id,
                                    Cantidad = elemento.Cantidad,
                                    ProductoId = elemento.ProductoId,
                                    ProductoNombre = producto.NombreProducto,
                                    ProductoCodigo = producto.CodigoReferencia,
                                    UnidadMedidaVentaId = elemento.UnidadMedidaVentaId,
                                    UnidadMedidaVentaNombre = unidadVenta.Nombre,
                                    ValorUnitario = elemento.PrecioValor,
                                    PrecioNombre = precio.NombrePrecio,
                                    DescuentoPorcentaje = (float)elemento.DescuentoPorcentaje,
                                    DescuentoValor = elemento.Descuento,
                                    TipoProductoId = (int)producto.TipoProductoId
                                });
                            }
                            else if (elemento.ServicioId != null)
                            {
                                //Si es un servicio
                                var servicio = elemento.Servicio ?? new Servicio();
                                var precio = elemento.Precio ?? new Precio();
                                emergencia.Servicios.Add(new EmergenciaServicioAgregadoViewModel
                                {
                                    Id = elemento.Id,
                                    ServicioId = elemento.ServicioId,
                                    ServicioNombre = servicio.NombreServicio,
                                    ServicioCodigo = servicio.CodigoInterno,
                                    Cantidad = elemento.Cantidad,
                                    PrecioNombre = precio.NombrePrecio,
                                    PrecioId = elemento.PrecioId,
                                    ValorUnitario = elemento.PrecioValor,
                                    DescuentoPorcentaje = elemento.DescuentoPorcentaje,
                                    DescuentoValor = elemento.Descuento
                                });
                            }
                            else if (elemento.ExamenLabClinicoId != null)
                            {
                                //Si es un examen
                                var examenLabClinico = elemento.ExamenLabClinico ?? new ExamenLabClinico();
                                var precio = elemento.Precio ?? new Precio();
                                emergencia.Examenes.Add(new EmergenciaExamenAgregadoViewModel
                                {
                                    Id = elemento.Id,
                                    ExamenId = elemento.ExamenLabClinicoId,
                                    ExamenNombre = examenLabClinico.NombreExamen,
                                    ExamenCodigo = examenLabClinico.CodigoInterno,
                                    Cantidad = Convert.ToInt32(elemento.Cantidad),
                                    PrecioId = elemento.PrecioId,
                                    PrecioNombre = precio.NombrePrecio,
                                    ValorUnitario = elemento.PrecioValor,
                                    DescuentoPorcentaje = elemento.DescuentoPorcentaje,
                                    DescuentoValor = elemento.Descuento
                                });
                            }
                        }
                    }
                }

                #endregion
            }

            return emergencia;
        }
        public void EditarEmergencia(EmergenciaViewModel model)
        {
            var fecha = DateTime.Now;
            var emergencia = _emergenciaRepository.Get((int)model.EmergenciaId, true, true);
            if (emergencia != null)
            {
                //PACIENTE
                var pacienteVerificado = _pacientesService.ValidarExistenciaPaciente(new PacientesBaseViewModel
                {
                    PacienteId = model.PacienteId ?? 0,
                    Nombre = model.PacienteNombre,
                    Direccion = model.PacienteDireccion,
                    Nit = model.PacienteNit
                });
                model.PacienteId = pacienteVerificado.PacienteId;

                //Modificacion de datos generales de emergencia
                emergencia.PacienteId = model.PacienteId;
                emergencia.FechaUltimaModificacion = fecha;
                emergencia.Responsable = model.Responsable;


                emergencia.EmergenciaDetalles = new List<EmergenciaDetalle>();
                emergencia.Observaciones = model.Observaciones;


                #region PRODUCTOS

                if (model.Productos != null)
                {
                    model.Productos = model.Productos.Where(a => !a.Eliminado).ToList();
                    foreach (var producto in model.Productos)
                    {
                        emergencia.EmergenciaDetalles.Add(new EmergenciaDetalle
                        {
                            Cantidad = producto.Cantidad,
                            PrecioId = producto.PrecioId,
                            UnidadMedidaVentaId = producto.UnidadMedidaVentaId,
                            PrecioValor = producto.ValorUnitario,
                            DescuentoPorcentaje = (decimal)producto.DescuentoPorcentaje,
                            Descuento = producto.DescuentoValor,
                            ProductoId = producto.ProductoId,
                            Subtotal = producto.Subtotal,
                            Total = producto.ValorTotal
                        });
                    }
                }

                #endregion

                #region SERVICIOS

                if (model.Servicios != null)
                {
                    model.Servicios = model.Servicios.Where(a => !a.Eliminado).ToList();
                    foreach (var servicio in model.Servicios)
                    {
                        emergencia.EmergenciaDetalles.Add(new EmergenciaDetalle
                        {
                            Cantidad = servicio.Cantidad,
                            PrecioId = servicio.PrecioId,
                            PrecioValor = servicio.ValorUnitario,
                            DescuentoPorcentaje = servicio.DescuentoPorcentaje,
                            Descuento = servicio.DescuentoValor,
                            ServicioId = servicio.ServicioId,
                            Subtotal = servicio.ValorSubtotal,
                            Total = servicio.ValorTotal
                        });
                    }
                }

                #endregion

                #region EXAMENES

                if (model.Examenes != null)
                {
                    model.Examenes = model.Examenes.Where(a => !a.Eliminado).ToList();
                    foreach (var examen in model.Examenes)
                    {
                        emergencia.EmergenciaDetalles.Add(new EmergenciaDetalle
                        {
                            Cantidad = examen.Cantidad,
                            PrecioId = examen.PrecioId,
                            PrecioValor = examen.ValorUnitario,
                            DescuentoPorcentaje = examen.DescuentoPorcentaje,
                            Descuento = examen.DescuentoValor,
                            ExamenLabClinicoId = examen.ExamenId,
                            Subtotal = examen.ValorSubtotal,
                            Total = examen.ValorTotal,
                        });
                    }
                }

                #endregion

                //Guardar cambios
                _emergenciaRepository.Update(emergencia);
            }
        }


        public void AgregarProducto(DetalleEmergenciaViewModel detalle)
        {
            var detalleEmergencia = new EmergenciaDetalle
            {
                EmergenciaId = detalle.EmergencialId,
                ProductoId = detalle.ProductId,
                Cantidad = detalle.Cantidad,
                PrecioValor = detalle.PrecioValor,
                DescuentoPorcentaje = detalle.DescuentoPorcentaje,
                Descuento = detalle.Descuento,
                Subtotal = detalle.Subtotal,
                Total = detalle.Total,
                PrecioId = detalle.Preciold,
                UnidadMedidaVentaId = detalle.UnidadMedidaVentad,
            };

            _emergenciaRepository.AddDetalle(detalleEmergencia);

        }

        public void AgregarServicio(DetalleEmergenciaViewModel detalle)
        {
            var detalleEmergencia = new EmergenciaDetalle
            {
                EmergenciaId = detalle.EmergencialId,
                ServicioId = detalle.ServicioId,
                Cantidad = detalle.Cantidad,
                PrecioValor = detalle.PrecioValor,
                Subtotal = detalle.Subtotal,
                Total = detalle.Total,
                PrecioId = detalle.Preciold,
            };

            _emergenciaRepository.AddDetalle(detalleEmergencia);

        }

        public void AgregarExamen(DetalleEmergenciaViewModel detalle)
        {
            var detalleEmergencia = new EmergenciaDetalle
            {
                EmergenciaId = detalle.EmergencialId,
                ExamenLabClinicoId = detalle.ExamenLabClinicId,
                Cantidad = detalle.Cantidad,
                PrecioValor = detalle.PrecioValor,
                Subtotal = detalle.Subtotal,
                Total = detalle.Total,
                PrecioId = detalle.Preciold,
            };

            _emergenciaRepository.AddDetalle(detalleEmergencia);

        }

        public void EliminarDetalle(int detalleId)
        {
            _emergenciaRepository.DeleteDetalle(detalleId);
        }
    }
}

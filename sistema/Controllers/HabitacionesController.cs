using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using Database.Shared.IRepository;
using sistema.Models;
using Database.Shared.Models;

namespace sistema.Controllers
{
    [Authorize]
    public class HabitacionesController : Controller
    {
        private readonly IHabitacion _habitacionRepository = null;

        public HabitacionesController(
            IHabitacion habitacionRepository
            )
        {
            _habitacionRepository = habitacionRepository;
        }
        public IActionResult Lista()
        {
            var habitaciones = _habitacionRepository.GetHabitaciones();
            return View(habitaciones);
        }
        public IActionResult Nueva()
        {
            var model = new HabitacionViewModel();
            return View(model);
        }
        [HttpPost]
        public string ConsultarCategoriasExistentes()
        {
            try
            {
                var categoriasConsultadas = new List<HabitacionCategoriaExistenteViewModel>();
                var categoriasBd = _habitacionRepository.GetCategorias();
                if (categoriasBd != null)
                {
                    foreach (var categoria in categoriasBd)
                    {
                        categoriasConsultadas.Add(new HabitacionCategoriaExistenteViewModel
                        {
                            Id = categoria.Id,
                            NombreCategoria = categoria.NombreCategoria
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = categoriasConsultadas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar categorias existentes. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarEstadosExistentes()
        {
            try
            {
                var estadosConsultados = new List<HabitacionEstadoExistenteViewModel>();
                var estadosBd = _habitacionRepository.GetEstados();
                if (estadosBd != null)
                {
                    foreach (var estado in estadosBd)
                    {
                        estadosConsultados.Add(new HabitacionEstadoExistenteViewModel
                        {
                            Id = estado.Id,
                            NombreEstado = estado.NombreEstado
                        });
                    }
                }

                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = estadosConsultados
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar estados existentes. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string Nueva(HabitacionViewModel model)
        {
            try
            {
                var habitacion = new Habitacion
                {
                    NombreNumeroHabitacion = model.NumeroNombre,
                    CategoriaHabitacionId = model.CategoriaId,
                    EstadoHabitacionId = model.EstadoId,
                    NumeroCamas = model.NumeroCamas,
                    CapacidadPersonas = model.CapacidadPersonas,
                    Eliminada = false
                };

                _habitacionRepository.Add(habitacion);

                TempData["Message"] = "La habitacion se ha creado con éxito";

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
                    Mensaje = "Error al crear habitacion. " + ex.Message
                });
            }
        }
        public IActionResult Modificar(int habitacionId)
        {
            var habitacion = _habitacionRepository.Get(habitacionId);
            var model = new HabitacionViewModel
            {
                CategoriaId = habitacion.CategoriaHabitacionId,
                CategoriaNombre = habitacion.CategoriaHabitacion.NombreCategoria,
                EstadoId = habitacion.EstadoHabitacionId,
                EstadoNombre = habitacion.EstadoHabitacion.NombreEstado,
                NumeroCamas = habitacion.NumeroCamas,
                CapacidadPersonas = habitacion.CapacidadPersonas,
                NumeroNombre = habitacion.NombreNumeroHabitacion
            };
            return View(model);
        }
        [HttpPost]
        public string Modificar(HabitacionViewModel model)
        {
            try
            {
                var habitacion = _habitacionRepository.Get((int)model.HabitacionId);

                habitacion.NombreNumeroHabitacion = model.NumeroNombre;
                habitacion.CategoriaHabitacionId = model.CategoriaId;
                habitacion.EstadoHabitacionId = model.EstadoId;
                habitacion.NumeroCamas = model.NumeroCamas;
                habitacion.CapacidadPersonas = model.CapacidadPersonas;

                _habitacionRepository.Update(habitacion);

                TempData["Message"] = "La habitacion se ha modificado con éxito";

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
                    Mensaje = "Error al modifcar habitacion. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string Eliminar(int? habitacionId)
        {
            try
            {
                _habitacionRepository.Delete((int)habitacionId);

                TempData["Message"] = "La habitacion se ha eliminado con éxito";

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
                    Mensaje = "Error al eliminar habitacion. " + ex.Message
                });
            }
        }
        public IActionResult ListaCategorias()
        {
            var categorias = (List<CategoriaHabitacion>)_habitacionRepository.GetCategorias();
            return View(categorias);
        }
        public IActionResult NuevaCategoria()
        {
            return View();
        }
        [HttpPost]
        public string NuevaCategoria(HabitacionesCategoriaViewModel model)
        {
            try
            {
                var categoria = new CategoriaHabitacion
                {
                    NombreCategoria = model.Nombre,
                    Eliminada = false
                };
                //Tarifas
                if (model.Tarifas != null)
                {
                    foreach (var tarifaModel in model.Tarifas)
                    {
                        categoria.CategoriaHabitacionTarifas.Add(new CategoriaHabitacionTarifa
                        {
                            NombreTarifa = tarifaModel.Nombre,
                            Lunes = tarifaModel.Lunes,
                            Martes = tarifaModel.Martes,
                            Miercoles = tarifaModel.Miercoles,
                            Jueves = tarifaModel.Jueves,
                            Viernes = tarifaModel.Viernes,
                            Sabado = tarifaModel.Sabado,
                            Domingo = tarifaModel.Domingo,
                            FechaEspecial = tarifaModel.FechaEspecial,
                            FechaTarifa = Convert.ToDateTime(tarifaModel.Fecha),
                            ValorTarifa = tarifaModel.Valor,
                            Eliminada = false
                        });
                    }
                }
                _habitacionRepository.AddCategoria(categoria);

                TempData["Message"] = "La categoría se ha registrado con éxito";

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
                    Mensaje = "Error al crear categoría. " + ex.Message
                });
            }
        }
        public IActionResult ModificarCategoria(int categoriaId)
        {
            var categoria = _habitacionRepository.GetCategoria(categoriaId);
            var model = new HabitacionesCategoriaViewModel
            {
                CategoriaId = categoriaId,
                Nombre = categoria.NombreCategoria
                //No se envia en este modelo las tarifas ya que ellas
                //se consultan por un llamado ajax desde JS
            };
            return View(model);
        }
        [HttpPost]
        public string ModificarCategoria(HabitacionesCategoriaViewModel model)
        {
            try
            {
                var categoria = _habitacionRepository.GetCategoria(model.CategoriaId);
                categoria.NombreCategoria = model.Nombre;
                if (categoria.CategoriaHabitacionTarifas != null)
                {
                    foreach (var tarifaCategoria in categoria.CategoriaHabitacionTarifas)
                    {
                        tarifaCategoria.Eliminada = true;
                    }
                }

                if (model.Tarifas != null)
                {
                    foreach (var tarifaModel in model.Tarifas)
                    {
                        if (tarifaModel.Id != null && tarifaModel.Id != 0)
                        {
                            //Codigo si la tarifa que viene en el modelo ya tiene Id,
                            //es decir ya existe en BD
                            foreach (var tarifaCategoria in categoria.CategoriaHabitacionTarifas)
                            {
                                if (tarifaCategoria.Id == tarifaModel.Id)
                                {
                                    tarifaCategoria.NombreTarifa = tarifaModel.Nombre;
                                    tarifaCategoria.Lunes = tarifaModel.Lunes;
                                    tarifaCategoria.Martes = tarifaModel.Martes;
                                    tarifaCategoria.Miercoles = tarifaModel.Miercoles;
                                    tarifaCategoria.Jueves = tarifaModel.Jueves;
                                    tarifaCategoria.Viernes = tarifaModel.Viernes;
                                    tarifaCategoria.Sabado = tarifaModel.Sabado;
                                    tarifaCategoria.Domingo = tarifaModel.Domingo;
                                    tarifaCategoria.FechaEspecial = tarifaModel.FechaEspecial;
                                    tarifaCategoria.FechaTarifa = Convert.ToDateTime(tarifaModel.Fecha);
                                    tarifaCategoria.ValorTarifa = tarifaModel.Valor;
                                    tarifaCategoria.Eliminada = false;
                                }
                            }
                        }
                        else
                        {
                            //Codigo si la tarifa no existe en BD,
                            //es decir no trae un Id
                            categoria.CategoriaHabitacionTarifas.Add(new CategoriaHabitacionTarifa
                            {
                                NombreTarifa = tarifaModel.Nombre,
                                Lunes = tarifaModel.Lunes,
                                Martes = tarifaModel.Martes,
                                Miercoles = tarifaModel.Miercoles,
                                Jueves = tarifaModel.Jueves,
                                Viernes = tarifaModel.Viernes,
                                Sabado = tarifaModel.Sabado,
                                Domingo = tarifaModel.Domingo,
                                FechaEspecial = tarifaModel.FechaEspecial,
                                FechaTarifa = Convert.ToDateTime(tarifaModel.Fecha),
                                ValorTarifa = tarifaModel.Valor,
                                Eliminada = false
                            });
                        }
                    }
                }
                _habitacionRepository.UpdateCategoria(categoria);

                TempData["Message"] = "La categoría se ha modificado con éxito";

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
                    Mensaje = "Error al modificar categoría. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string ConsultarTarifasCategoria(int? categoriaId)
        {
            try
            {
                var tarifas = new List<HabitacionesCategoriaTarifaViewModel>();
                var categoria = _habitacionRepository.GetCategoria((int)categoriaId);
                if (categoria != null)
                {
                    if (categoria.CategoriaHabitacionTarifas != null)
                    {
                        foreach (var tarifa in categoria.CategoriaHabitacionTarifas)
                        {
                            tarifas.Add(new HabitacionesCategoriaTarifaViewModel
                            {
                                Id = tarifa.Id,
                                Nombre = tarifa.NombreTarifa,
                                Lunes = tarifa.Lunes,
                                Martes = tarifa.Martes,
                                Miercoles = tarifa.Miercoles,
                                Jueves = tarifa.Jueves,
                                Viernes = tarifa.Viernes,
                                Sabado = tarifa.Sabado,
                                Domingo = tarifa.Domingo,
                                FechaEspecial = tarifa.FechaEspecial,
                                Fecha = tarifa.FechaEspecial ? (tarifa.FechaTarifa == null ? null
                                : Convert.ToDateTime(tarifa.FechaTarifa).ToString("yyyy-MM-dd"))
                                : null,
                                Valor = tarifa.ValorTarifa,
                                Nueva = false
                            });
                        }
                    }
                }
                return JsonSerializer.Serialize(new
                {
                    Exitoso = true,
                    Resultado = tarifas
                });
            }
            catch (Exception ex)
            {
                return JsonSerializer.Serialize(new
                {
                    Exitoso = false,
                    Mensaje = "Error al consultar tarifas. " + ex.Message
                });
            }
        }
        [HttpPost]
        public string EliminarCategoria(int? categoriaId)
        {
            try
            {
                _habitacionRepository.DeleteCategoria((int)categoriaId);

                TempData["Message"] = "La categoría se ha eliminado con éxito";

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
                    Mensaje = "Error al eliminar categoría. " + ex.Message
                });
            }
        }
    }
}
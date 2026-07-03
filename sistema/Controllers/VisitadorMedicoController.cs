using Database.Shared.IRepository;
using Database.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using sistema.Models;
using sistema.Utilidades;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace sistema.Controllers
{
    public class VisitadorMedicoController : Controller
    {
        private readonly IVisitadorMedico _visitadorMedicoRepository;

        public VisitadorMedicoController(IVisitadorMedico visitadorMedicoRepository)
        {
            _visitadorMedicoRepository = visitadorMedicoRepository;
        }
        public IActionResult Lista()
        {
            VisitadorMedicoViewModel visitadorMedicoViewModel = new VisitadorMedicoViewModel()
            {
                    VisitadorMedico = _visitadorMedicoRepository.GetAllVisitadorMedico()
            };
            return View(visitadorMedicoViewModel);
        }
        public IActionResult Nuevo()
        {
            var model = new VisitadorMedico();
            return View(model);
        }

        [HttpPost]
        public string Nuevo(VisitadorMedico model)
        {
            try
            {
                string urlCatalogo = null;
                if (!string.IsNullOrWhiteSpace(model.UrlCatalogo))
                {
                    urlCatalogo = CatalogUrlHelper.Normalize(model.UrlCatalogo);
                    if (urlCatalogo == null)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "La URL del catálogo no es válida. Use un enlace completo, por ejemplo: https://www.ejemplo.com/catalogo"
                        });
                    }
                }

                var visitadorMedico = new VisitadorMedico
                {
                    NombreVisitador = model.NombreVisitador,
                    ContactoVisitador = model.ContactoVisitador,
                    NombreFarmaceutica = model.NombreFarmaceutica,
                    ContactoFarmaceutica = model.ContactoFarmaceutica,
                    Observaciones = model.Observaciones,
                    UrlCatalogo = urlCatalogo,

                };

                _visitadorMedicoRepository.AddVisitadorMedico(visitadorMedico);
                TempData["Message"] = "El Visitador Médico se ha registrado con éxito!";

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
                    Mensaje = "Error al registrar el Visitador Médico. " + ex.Message
                });
            }
        }

        public IActionResult Modificar(int VisitadorMedicoId)
        {
            var visitadorMedico = _visitadorMedicoRepository.GetVisitadorMedicoById(VisitadorMedicoId);

            if (visitadorMedico == null)
            {
                return NotFound();
            }

            return View(visitadorMedico);
        }

        [HttpPost]
        public string Modificar(VisitadorMedico model)
        {
            try
            {
                var visitadorMedico = _visitadorMedicoRepository.GetVisitadorMedicoById(model.Id);

                if (visitadorMedico == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El visitador médico no existe."
                    });
                }

                string urlCatalogo = null;
                if (!string.IsNullOrWhiteSpace(model.UrlCatalogo))
                {
                    urlCatalogo = CatalogUrlHelper.Normalize(model.UrlCatalogo);
                    if (urlCatalogo == null)
                    {
                        return JsonSerializer.Serialize(new
                        {
                            Exitoso = false,
                            Mensaje = "La URL del catálogo no es válida. Use un enlace completo, por ejemplo: https://www.ejemplo.com/catalogo"
                        });
                    }
                }

                visitadorMedico.NombreVisitador = model.NombreVisitador;
                visitadorMedico.ContactoVisitador = model.ContactoVisitador;
                visitadorMedico.NombreFarmaceutica = model.NombreFarmaceutica;
                visitadorMedico.ContactoFarmaceutica = model.ContactoFarmaceutica;
                visitadorMedico.Observaciones = model.Observaciones;
                visitadorMedico.UrlCatalogo = urlCatalogo;

                _visitadorMedicoRepository.UpdateVisitadorMedico(visitadorMedico);

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
                    Mensaje = "Error al actualizar el Visitador Médico. " + ex.Message
                });
            }
        }

        [HttpPost]
        public string Eliminar(int id)
        {
            try
            {
                var visitadorMedico = _visitadorMedicoRepository.GetVisitadorMedicoById(id);

                if (visitadorMedico == null)
                {
                    return JsonSerializer.Serialize(new
                    {
                        Exitoso = false,
                        Mensaje = "El visitador médico no existe o ya fue eliminado."
                    });
                }

                _visitadorMedicoRepository.DeleteVisitadorMedico(id);

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
                    Mensaje = "Error al intentar eliminar el visitador médico: " + ex.Message
                });
            }
        }
    }
}
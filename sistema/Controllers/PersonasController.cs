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
using sistema.Json;
using Microsoft.AspNetCore.Authorization;
using Database.Shared.Enumeraciones;

namespace sistema.Controllers
{
    [Authorize]
    public class PersonasController : Controller
    {
        private readonly IPersonas _personasRepository = null;
        private readonly IPacientes _pacientesRepository = null;

        public PersonasController(IPersonas personasRepository, IPacientes pacientesRepository)
        {
            _personasRepository = personasRepository;
            _pacientesRepository = pacientesRepository;
        }

        public IActionResult Lista()
        {
            return View();
        }
        [HttpPost]
        public JsonResult ConsultarPersonas()
        {
            try
            {
                var resultado = _personasRepository.GetPersonas();
                return Json(new { Exitoso = true, Resultado = resultado });
            }
            catch (Exception ex)
            {
                return Json(new { Exitoso = false, Mensaje = "Error al consultar personas. " + ex.Message });
            }
        }

        public IActionResult Nueva()
        {
            var model = new PersonasViewModel();
            model.Init(_personasRepository);
            return View(model);
        }

        [HttpPost]
        public IActionResult Nueva(PersonasViewModel model)
        {
            if (ModelState.IsValid)
            {
                var persona = new Persona
                {
                    Nombre = model.Nombre,
                    SexoId = model.SexoId,
                    FechaContacto = model.FechaContacto,
                    Email = model.Email,
                    Telefono = model.Telefono,
                    TipoRedSocialId = model.TipoRedSocialId,
                    Eliminada = false,
                    Paciente = model.TomaServicio,
                    TomaServicio = model.TomaServicio,
                    MotivoNoTomarServicio = model.MotivoNoTomarServicio,
                    TipificacionComunicacionId = model.TipificacionComunicacionId
                };
                persona = _personasRepository.Add(persona);

                if (persona.TomaServicio ?? false)
                {
                    var paciente = new Paciente
                    {
                        FechaRegistro = DateTime.Today,
                        Nombre = persona.Nombre,
                        SexoId = persona.SexoId,
                        Email = persona.Email,
                        Telefono = persona.Telefono,
                        ModalidadAtencion = "presencial",
                        //Id=persona.Id
                    };

                    //Cuenta por cobrar
                    var fasesTratamientos = _pacientesRepository.GetFasesTratamiento();
                    var cuentaPorCobrar = new CuentaPorCobrar();
                    cuentaPorCobrar.FechaLimitePago = Convert.ToDateTime(paciente.FechaRegistro).AddMonths(1);
                    cuentaPorCobrar.Valor = Convert.ToDecimal(fasesTratamientos
                        .Where(f => f.Id == (int)FaseTratamientoEnum.Adelgazamiento)
                        .Select(f => f.Valor));
                    cuentaPorCobrar.Pagada = false;
                    cuentaPorCobrar.Eliminada = false;
                    paciente.CuentasPorCobrar.Add(cuentaPorCobrar);

                    _pacientesRepository.Add(paciente);
                }

                TempData["Message"] = "¡La persona se ha registrado con éxito.!";
                return RedirectToAction("Lista");
            }
            model.Init(_personasRepository);
            return View(model);
        }

        public IActionResult Modificar(int? id)
        {
            if (id == null)
            {
                return BadRequest("Request is incorrect");
            }

            var persona = _personasRepository.Get((int)id);

            if (persona == null)
            {
                return StatusCode(404);
            }

            var model = new PersonasViewModel()
            {
                Nombre = persona.Nombre,
                SexoId = (int)persona.SexoId,
                FechaContacto = (DateTime)persona.FechaContacto,
                Email = persona.Email,
                Telefono = persona.Telefono,
                TipoRedSocialId = persona.TipoRedSocialId,
                TomaServicio = persona.TomaServicio ?? false,
                MotivoNoTomarServicio = persona.MotivoNoTomarServicio
            };

            model.Init(_personasRepository);

            return View(model);
        }

        [HttpPost]
        public IActionResult Modificar(PersonasViewModel model)
        {
            if (ModelState.IsValid)
            {
                var persona = _personasRepository.Get((int)model.Id);
                persona.Nombre = model.Nombre;
                persona.SexoId = model.SexoId;
                persona.FechaContacto = model.FechaContacto;
                persona.Email = model.Email;
                persona.Telefono = model.Telefono;
                persona.TipoRedSocialId = model.TipoRedSocialId;
                persona.TomaServicio = model.TomaServicio;
                persona.MotivoNoTomarServicio = model.MotivoNoTomarServicio;

                _personasRepository.Update(persona);
                TempData["Message"] = "¡La persona se ha modificado con exito.!";
                return RedirectToAction("Lista");
            }

            model.Init(_personasRepository);
            return View(model);
        }

        [HttpPost]
        public JsonResult Eliminar(int? id)
        {
            if (id == null)
            {
                return Json(new { Exitoso = false, Mensaje = "Request is incorrect" });
            }

            var persona = _personasRepository.Get((int)id);


            if (persona == null)
            {
                return Json(new { Exitoso = false, Mensaje = "El registro de persona no existe" });
            }

            persona.Eliminada = true;

            _personasRepository.Update(persona);
            TempData["Message"] = "¡La persona se ha eliminado con exito.!";
            return Json(new { Exitoso = true });
        }
        [HttpPost]
        public JsonResult ConvertirPaciente(int? id)
        {
            if (id == null)
            {
                return Json(new { Exitoso = false, Mensaje = "Request is incorrect" });
            }

            var persona = _personasRepository.Get((int)id);

            if (persona == null)
            {
                return Json(new { Exitoso = false, Mensaje = "El registro de persona no existe" });
            }

            persona.Paciente = true;
            _personasRepository.Update(persona);

            var paciente = new Paciente
            {
                FechaRegistro = DateTime.Today,
                Nombre = persona.Nombre,
                SexoId = persona.SexoId,
                Email = persona.Email,
                Telefono = persona.Telefono,
                ModalidadAtencion = "presencial"
            };

            //Cuenta por cobrar
            var fasesTratamientos = _pacientesRepository.GetFasesTratamiento();
            var cuentaPorCobrar = new CuentaPorCobrar();
            cuentaPorCobrar.FechaLimitePago = Convert.ToDateTime(paciente.FechaRegistro).AddMonths(1);
            cuentaPorCobrar.Valor = fasesTratamientos.Where(f => f.Id == (int)FaseTratamientoEnum.Adelgazamiento).Select(f => f.Valor).FirstOrDefault();
            cuentaPorCobrar.Pagada = false;
            cuentaPorCobrar.Eliminada = false;
            paciente.CuentasPorCobrar.Add(cuentaPorCobrar);
            paciente.EstadoPacienteId = (int)EstadoPacienteEnum.Activo;
            paciente.TipoPacienteId = (int)TipoPacienteEnum.Nuevo;

            _pacientesRepository.Add(paciente);

            TempData["Message"] = "¡La persona se ha registrado como paciente!";
            return Json(new { Exitoso = true });
        }
    }
}

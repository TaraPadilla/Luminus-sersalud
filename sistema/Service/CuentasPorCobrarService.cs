using Database.Shared.IRepository;
using Database.Shared.Models;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
using farmamest.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using sistema.Models;
using sistema.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;

namespace sistema.Service
{
    public class CuentasPorCobrarService : ICuentasPorCobrarService
    {
        private readonly ICuentasPorCobrar _cuentasPorCobrarRepository;
        private readonly IHospitalizacion _hospitalizacionRepository;

        public CuentasPorCobrarService(ICuentasPorCobrar cuentasPorCobrarRepository, IHospitalizacion hospitalizacionRepository)
        {
            _cuentasPorCobrarRepository = cuentasPorCobrarRepository;
            _hospitalizacionRepository = hospitalizacionRepository;
        }

        public CuentasPorCobrarViewModel GetDetalleCuentaPorCobrar(int cuentaId)
        {

            var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);

            var hospitalizaciones = cuenta.Paciente.Hospitalizaciones.Where(x => !x.Eliminada && !x.Finalizada).ToList();

            var medicamentos = hospitalizaciones.SelectMany(x => x.HospitalizacionesProductos).Where(x => !x.Eliminado).ToList(); //Lista de medicamentos de la hospitalizacion
            var servicios = hospitalizaciones.SelectMany(x => x.HospitalizacionesServicios).Where(x => !x.Eliminado).ToList(); //Lista de servicios de la hospitalizacion
            var examenes = hospitalizaciones.SelectMany(x => x.HospitalizacionesExamenes).Where(x => !x.Eliminado).ToList(); //Lista de medicamentos de la hospitalizacion

            var totalMedicamentos = CalculadoMedicamentos(medicamentos) ?? 0; //Calculo de medicamentos

            var totalPaquetes = CalculadoPaquetes(hospitalizaciones) ?? 0; // Calculo de paquetes

            var totalServicios = CalculadoServicios(servicios) ?? 0; // Calculo de servicios

            var totalExamenes = CalculadoExamenes(examenes) ?? 0; //Calculo de examenes

            var totalHospitalizacion = CalculadoHospitalizacion(hospitalizaciones) ?? 0; //Calculo de hospitalizacion

            var totalTotal = totalMedicamentos + totalPaquetes + totalServicios + totalExamenes + totalHospitalizacion;

            CuentasPorCobrarViewModel viewModel = new CuentasPorCobrarViewModel()
            {
                CuentasPorCobrar = cuenta,
                TotalHospitalizacion = totalHospitalizacion,
                TotalMedicamentos = totalMedicamentos,
                TotalTotal = Math.Round((decimal)totalTotal, 2)
            };


            return viewModel;
        }

        private decimal? CalculadoHospitalizacion(List<Hospitalizacion> listaHospitalizaciones)
        {
            decimal? total = 0;
            foreach (var item in listaHospitalizaciones)
            {
                total += item.CategoriaHabitacionTarifa?.ValorTarifa * item.NochesInt;
            }
            return Math.Round((decimal)total, 2);
        }

        private decimal? CalculadoPaquetes(List<Hospitalizacion> listaHospitalizacion)
        {
            decimal? total = 0;
            foreach (var hospitalizacion in listaHospitalizacion)
            {
                var paquetesHospitalizacion = _hospitalizacionRepository.GetPaquetesAgregados(hospitalizacion.Id);

                foreach (var paquete in paquetesHospitalizacion)
                {
                    if (paquete.HospitalizacionDetallePaqueteHospitalizacion != null)
                    {
                        foreach (var detalle in paquete.HospitalizacionDetallePaqueteHospitalizacion)
                        {
                            if (detalle.Aplicacion)
                            {
                                total += detalle.PrecioProducto;
                            }
                        }
                    }
                    //Pendiente por revisar el total de servicios y laboratorios
                    //foreach (var detallePaquete in paquete.PaqueteHospitalizacion.DetallePaquetesHospitalizacion)
                    //{
                    //    total += (detallePaquete.ServicioPrecio?.Valor ?? 0) * detallePaquete.Cantidad;
                    //    total += (detallePaquete.ProductoInventarioPrecio?.Valor ?? 0) * detallePaquete.Cantidad;
                    //    total += (detallePaquete.LaboratorioPrecio?.PrecioValor ?? 0) * detallePaquete.Cantidad;
                    //}
                }

            }
            return Math.Round((decimal)total, 2); ;
        }

        private decimal? CalculadoMedicamentos(List<HospitalizacionProducto> listaMedicamentos)
        {
            decimal? total = 0;

            foreach (var item in listaMedicamentos)
            {
                if (item.PrecioProducto != null && item.PrecioProducto?.Valor != 0)
                {
                    total += item.PrecioProducto?.Valor * item.Cantidad;
                }
                else
                {
                    if (item.PrecioValor != 0)
                    {
                        total += item.PrecioValor;
                    }
                }
            }
            return Math.Round((decimal)total, 2);
        }
        private decimal? CalculadoServicios(List<HospitalizacionServicio> listaServicio)
        {
            decimal? total = 0;

            foreach (var item in listaServicio)
            {
                if (item.PrecioServicio != null && item.PrecioServicio?.Valor != 0)
                {
                    total += item.PrecioServicio?.Valor * item.Cantidad;
                }
                else
                {
                    if (item.Precio != 0)
                    {
                        total += item.Precio;
                    }
                }
            }
            return Math.Round((decimal)total, 2);
        }
        private decimal? CalculadoExamenes(List<HospitalizacionExamen> listaExamenes)
        {
            decimal? total = 0;

            foreach (var item in listaExamenes)
            {
                if (item.ExamenLabClinicoPrecio != null && item.ExamenLabClinicoPrecio?.PrecioValor != 0)
                {
                    total += item.ExamenLabClinicoPrecio?.PrecioValor;
                }
            }
            return Math.Round((decimal)total, 2);
        }
        public List<HospitalizacionProductoViewModel> GetMedicamentosNoPagadosHospitalizaciones(int cuentaId)
        {
            var listaMedicamentosNoPagados = new List<HospitalizacionProductoViewModel>();
            var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
            if (cuenta.Paciente != null)
            {
                if (cuenta.Paciente.Hospitalizaciones != null)
                {
                    foreach (var hospitalizacion in cuenta.Paciente.Hospitalizaciones)
                    {
                        if (!hospitalizacion.Pagada)
                        {
                            if (hospitalizacion.HospitalizacionesProductos != null)
                            {
                                foreach (var productoFormulado in hospitalizacion.HospitalizacionesProductos)
                                {
                                    if (productoFormulado.HospitalizacionesProductosAplicaciones != null)
                                    {
                                        var hayAplicacionProductos = productoFormulado.HospitalizacionesProductosAplicaciones
                                            .Any(a => !a.Eliminado && a.Aplicado);
                                        var producto = productoFormulado.Producto ?? new Producto();
                                        var cantidadAplicada = productoFormulado.HospitalizacionesProductosAplicaciones
                                            .Where(a => a.Aplicado && !a.Eliminado)
                                            .Sum(a => a.Cantidad);
                                        if (hayAplicacionProductos)
                                        {
                                            listaMedicamentosNoPagados.Add(new HospitalizacionProductoViewModel
                                            {
                                                Nombre = producto.NombreProducto,
                                                CantidadAplicada = cantidadAplicada,
                                                Precio = productoFormulado.PrecioValor,
                                                Subtotal = productoFormulado.PrecioValor * cantidadAplicada
                                            });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return listaMedicamentosNoPagados;
        }
        public List<HospitalizacionPaqueteViewModel> GetPaquetesNoPagadosHospitalizacion(int cuentaId)
        {
            var listaPaquetesNoPagados = new List<HospitalizacionPaqueteViewModel>();
            var cuenta = _cuentasPorCobrarRepository.Get(cuentaId);
            if (cuenta.Paciente != null)
            {
                if (cuenta.Paciente.Hospitalizaciones != null)
                {
                    foreach (var hospitalizacion in cuenta.Paciente.Hospitalizaciones)
                    {
                        if (!hospitalizacion.Pagada)
                        {
                            if (hospitalizacion.HospitalizacionesPaquetesHospitalizacion != null)
                            {
                                foreach (var paquete in hospitalizacion.HospitalizacionesPaquetesHospitalizacion)
                                {
                                    if (!paquete.Eliminado)
                                    {
                                        var infoPaquete = paquete.PaqueteHospitalizacion ?? new PaqueteHospitalizacion();
                                        listaPaquetesNoPagados.Add(new HospitalizacionPaqueteViewModel
                                        {
                                            Fecha = paquete.FechaHora != null ? paquete.FechaHora.ToString() : "-",
                                            Nombre = infoPaquete.NombrePaquete,
                                            PrecioPaquete = infoPaquete.Precio ?? 0
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return listaPaquetesNoPagados;
        }
    }
}

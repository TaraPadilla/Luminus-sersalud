using Database.Shared.IRepository;
using Database.Shared.Models;
using Database.Shared.Paginacion;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using static Database.Shared.Data.GraficaRepository;

namespace Database.Shared.Data
{
    public class GraficaRepository : IGrafica
    {

        private readonly Context _context = null;
        //private List<VentasDia> ventasGeneralesxDia;

        public GraficaRepository(Context context)
        {
            _context = context;
        }
        public List<Venta> GetVentasClinica() => _context.Ventas
              .Where(a => a.Eliminado == false && a.TipoVenta == "clinica").ToList();

        public List<Venta> GetVentasFarmacia() => _context.Ventas
              .Where(a => a.Eliminado == false && a.TipoVenta == "farmacia").ToList();


        //public List<VentasLab> GetVentaLaboratorio() => _context.VentasLabs
        //     .Where(a => a.Eliminado == false).ToList();


     
        public List<decimal> GetVentasGenerales()
        {

            var ventasGenerales = new List<decimal>();

            var ventasClinica = GetVentasClinica();
            var ventasFarmacia = GetVentasFarmacia();
            //var ventasLaboratorio = GetVentaLaboratorio();

            for (int mes = 1; mes <= 12; mes++)
            {
                var ventasMes =
                    ventasClinica.Where(v => v.FechaVenta.Month == mes)
                     .Select(v => v.MontoPago)
                     .Concat(ventasFarmacia.Where(v => v.FechaVenta.Month == mes)
                     .Select(v => v.MontoPago))
                     //.Concat(ventasLaboratorio.Where(v => v.FechaVenta.Month == mes)
                     //.Select(v => v.MontoPagado))
                     .ToList();
                ventasGenerales.Add(ventasMes.Sum());

            }


            return ventasGenerales;


        }
        public class VentasDia
        {
            //public string Name { get; set; }
            public string Id { get; set; }

            public List<ValoresGrafica> Data { get; set; }
        }

        public class PagosDia
        {
            //public string Name { get; set; }
            public string Id { get; set; }

            public List<ValoresGrafica> Data { get; set; }
        }

        public class IngresosDia
        {
            //public string Name { get; set; }
            public string Id { get; set; }

            public List<ValoresGrafica> Data { get; set; }
        }

        public class IngresosVentasDia
        {
            //public string Name { get; set; }
            public string Id { get; set; }

            public List<ValoresGrafica> Data { get; set; }
        }

        public class GastosComprasDia
        {
            //public string Name { get; set; }
            public string Id { get; set; }

            public List<ValoresGrafica> Data { get; set; }
        }
        public class GastosDia
        {
            //public string Name { get; set; }
            public string Id { get; set; }

            public List<ValoresGrafica> Data { get; set; }
        }
        public class ValoresGrafica
        {
            public string Name { get; set; }
            public decimal Y { get; set; }

            public string drilldown { get; set; }

            //         public decimal ventasClinicaDia { get; set; }
            //public decimal ventasFarmaciaDia { get; set; }

            //public decimal ventasLaboratorioDia { get; set; }
        }
        public List<VentasDia> GetVentasGeneralesxDia()
        {
            var ventasGeneralesxMes = new List<VentasDia>();
            var ventasClinica = GetVentasClinica();
            var ventasFarmacia = GetVentasFarmacia();
            //var ventasLaboratorio = GetVentaLaboratorio();

            for (int mes = 1; mes <= 12; mes++)
            {

                var ventasMontoxDia = new List<ValoresGrafica>();
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var ventasMesDia =
                        ventasClinica.Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                         .Select(v => v.MontoPago)
                         .Concat(ventasFarmacia.Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                         .Select(v => v.MontoPago))
                         //.Concat(ventasLaboratorio.Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                         //.Select(v => v.MontoPagado))
                         .ToList();
                    ventasMontoxDia
                     .Add(new ValoresGrafica
                     {
                         Name = dia.ToString(),
                         Y = Convert.ToDecimal(ventasMesDia.Sum()),
                         drilldown = dia.ToString() + mes.ToString()

                     }); ; ;

                }

                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

                var ventasxDia = new VentasDia
                {
                    //Name = nombreMes,
                    Id = nombreMes,
                    Data = ventasMontoxDia,

                };

                ventasGeneralesxMes.Add(ventasxDia);
            }


            for (int mes = 1; mes <= 12; mes++)
            {

                var ventasxAreaDia = new List<ValoresGrafica>();
                int diasEnElMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);

                for (int dia = 1; dia <= diasEnElMes; dia++)
                {
                    var ventasMontoTipoxDia = new List<ValoresGrafica>();

                    var ventasClinicaDia = ventasClinica
                        .Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                        .Select(v => v.MontoPago)
                        .Sum();

                    ventasMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "VentasClinica",
                        Y = ventasClinicaDia
                    });

                    var ventasFarmaciaDia = ventasFarmacia
                        .Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                        .Select(v => v.MontoPago)
                        .Sum();

                    ventasMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "VentasFarmacia",
                        Y = ventasFarmaciaDia
                    });

                    //var ventasLaboratorioDia = ventasLaboratorio
                    //    .Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                    //    .Select(v => v.MontoPagado)
                    //    .Sum();

                    //ventasMontoTipoxDia.Add(new ValoresGrafica
                    //{
                    //    Name = "VentasLaboratorio",
                    //    Y = ventasLaboratorioDia
                    //});

                    ventasGeneralesxMes.Add(new VentasDia
                    {
                        Id = dia.ToString() + mes.ToString(),
                        Data = ventasMontoTipoxDia // Clonar la lista para evitar referencias compartidas
                    });



                }


            }




            return ventasGeneralesxMes;


        }


        public List<Pagos> GetTiposDePago() => _context.Pagos
            .Include(v => v.Venta)
           .Where(a => a.Eliminado == false).ToList();

        public List<decimal> GetPagosGenerales()
        {

            var pagosGenerales = new List<decimal>();

            var todosPagos = GetTiposDePago();

            for (int mes = 1; mes <= 12; mes++)
            {
                var pagosMensuales = todosPagos
                   .Where(v => v.Venta != null && v.Venta.FechaVenta.Month == mes)
                     .Select(v => v.Monto)
                     .ToList();
                pagosGenerales.Add(pagosMensuales.Sum());

            }

            return pagosGenerales;


        }


        public List<PagosDia> GetPagosGeneralesxDia()
        {
            var pagosGeneralesxMes = new List<PagosDia>();
            var todosPagos = GetTiposDePago();

            for (int mes = 1; mes <= 12; mes++)
            {

                var pagosMontoxDia = new List<ValoresGrafica>();
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var pagosMesDia =
                       todosPagos.Where(v => v.Venta != null && v.Venta.FechaVenta.Month == mes && v.Venta.FechaVenta.Day == dia)
                         .Select(v => v.Monto);

                    pagosMontoxDia
                     .Add(new ValoresGrafica
                     {
                         Name = dia.ToString(),
                         Y = Convert.ToDecimal(pagosMesDia.Sum()),
                         drilldown = dia.ToString() + mes.ToString()

                     }); ; ;

                }

                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

                var pagosxDia = new PagosDia
                {
                    //Name = nombreMes,
                    Id = nombreMes,
                    Data = pagosMontoxDia,

                };

                pagosGeneralesxMes.Add(pagosxDia);
            }

            for (int mes = 1; mes <= 12; mes++)
            {

                var pagosxAreaDia = new List<ValoresGrafica>();
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);

                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var pagosMontoTipoxDia = new List<ValoresGrafica>();


                    var montoEfectivoDia = todosPagos
                        .Where(p => p.Venta != null && p.Venta.FechaVenta.Month == mes && p.Venta.FechaVenta.Day == dia)
                        .Where(p => p.FormaPagoId == 1)
                         .Select(v => v.Monto)
                         .Sum();

                    pagosMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "Efectivo",
                        Y = montoEfectivoDia
                    });

                    var montoTarjetaVisaDia = todosPagos
                       .Where(p => p.Venta != null && p.Venta.FechaVenta.Month == mes && p.Venta.FechaVenta.Day == dia)
                       .Where(p => p.FormaPagoId == 2)
                        .Select(v => v.Monto)
                        .Sum();

                    pagosMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "Tarjeta Visa",
                        Y = montoTarjetaVisaDia
                    });

                    var montoTarjetaMasterCard = todosPagos
                     .Where(p => p.Venta != null && p.Venta.FechaVenta.Month == mes && p.Venta.FechaVenta.Day == dia)
                     .Where(p => p.FormaPagoId == 3)
                      .Select(v => v.Monto)
                      .Sum();

                    pagosMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "Tarjeta MasterCard",
                        Y = montoTarjetaMasterCard
                    });

                    var motoChequesDia = todosPagos
                   .Where(p => p.Venta != null && p.Venta.FechaVenta.Month == mes && p.Venta.FechaVenta.Day == dia)
                   .Where(p => p.FormaPagoId == 4)
                    .Select(v => v.Monto)
                    .Sum();

                    pagosMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "Cheques",
                        Y = motoChequesDia
                    });

                    var montoTransferenciaDia = todosPagos
                   .Where(p => p.Venta != null && p.Venta.FechaVenta.Month == mes && p.Venta.FechaVenta.Day == dia)
                   .Where(p => p.FormaPagoId == 5)
                    .Select(v => v.Monto)
                    .Sum();

                    pagosMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "Transferencia",
                        Y = montoTransferenciaDia
                    });

                    var montoLinkDia = todosPagos
                   .Where(p => p.Venta != null && p.Venta.FechaVenta.Month == mes && p.Venta.FechaVenta.Day == dia)
                   .Where(p => p.FormaPagoId == 6)
                    .Select(v => v.Monto)
                    .Sum();

                    pagosMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "VisaLink",
                        Y = montoLinkDia
                    });

                    var montoNetDia = todosPagos
                   .Where(p => p.Venta != null && p.Venta.FechaVenta.Month == mes && p.Venta.FechaVenta.Day == dia)
                   .Where(p => p.FormaPagoId == 7)
                    .Select(v => v.Monto)
                    .Sum();

                    pagosMontoTipoxDia.Add(new ValoresGrafica
                    {
                        Name = "VisaNet",
                        Y = montoNetDia
                    });

                    pagosGeneralesxMes.Add(new PagosDia
                    {
                        Id = dia.ToString() + mes.ToString(),
                        Data = pagosMontoTipoxDia // Clonar la lista para evitar referencias compartidas
                    });



                }


            }








            return pagosGeneralesxMes;


        }


        public List<DetalleCaja> GetDetalleCajas() => _context.DetalleCajas
            .ToList();
        public List<Venta> GetVentas() => _context.Ventas
           .Where(a => a.Eliminado).ToList();

        public List<Compra> GetCompras() => _context.Compras
           .Where(a => a.Eliminado).ToList();


        public List<DetalleCaja> GetDetalleCajasCuentaContable() => _context.DetalleCajas
           .Include(dc => dc.CuentaContable)
          .ToList();

        public List<CategoriasCuentaContable> GetCategoriasCuentasContables() => _context.CategoriasCuentaContables
          .ToList();

        public List<CuentaContable> GetCuentasContables() => _context.CuentaContable
          .ToList();

        public List<decimal> GetIngresosGenerales()
        {

            var ingresosGenerales = new List<decimal>();

            var todosIgresos = GetDetalleCajas();

            for (int mes = 1; mes <= 12; mes++)
            {
                var ingresosMensuales = todosIgresos
                   .Where(I => I.Fecha.Month == mes)
                     .Select(I => I.Ingreso)
                     .ToList();
                ingresosGenerales.Add(ingresosMensuales.Sum());

            }

            return ingresosGenerales;


        }

        public List<IngresosDia> GetIngresosGeneralesxDia()
        {
            var IngresosGeneralesxMes = new List<IngresosDia>();
            var todosIngresos = GetDetalleCajas();

            for (int mes = 1; mes <= 12; mes++)
            {

                var IngresosMontoxDia = new List<ValoresGrafica>();
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var IngresosMesDia =
                       todosIngresos.Where(I => I.Fecha.Month == mes && I.Fecha.Day == dia)
                         .Select(I => I.Ingreso);

                    IngresosMontoxDia
                     .Add(new ValoresGrafica
                     {
                         Name = dia.ToString(),
                         Y = Convert.ToDecimal(IngresosMesDia.Sum()),
                         drilldown = dia.ToString() + mes.ToString()

                     }); ; ;

                }

                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

                var IngresosxDia = new IngresosDia
                {
                    //Name = nombreMes,
                    Id = nombreMes,
                    Data = IngresosMontoxDia,

                };

                IngresosGeneralesxMes.Add(IngresosxDia);
            }
            var cuentasdecategoria = new List<CuentaContable>();
            var detalleCaja = new List<DetalleCaja>();

            var categoriasCuentasContables = GetCategoriasCuentasContables();

            var cuentasContables = GetDetalleCajasCuentaContable();

            var prueba = cuentasdecategoria;
            for (int mes = 1; mes <= 12; mes++)
            {

                //var TipoxAreaDia = new List<ValoresGrafica>();
                int diasEnElMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);

                for (int dia = 1; dia <= diasEnElMes; dia++)
                {
                    var ingresosMontoCategoriasCuentasxDia = new List<ValoresGrafica>();


                    foreach (var categoria in categoriasCuentasContables)
                    {
                        var montoCategoria =
                              cuentasContables.
                              Where(cc => cc.CuentaContable != null && cc.CuentaContable.CategoriaCuentaId == categoria.Id)
                              .Where(dc => dc.Fecha.Month == mes && dc.Fecha.Day == dia)
                              .Select(dc => dc.Ingreso)
                              .Sum();



                        ingresosMontoCategoriasCuentasxDia.Add(new ValoresGrafica
                        {
                            Name = categoria.Nombre,
                            Y = montoCategoria
                        });



                    };

                    IngresosGeneralesxMes.Add(new IngresosDia
                    {
                        Id = dia.ToString() + mes.ToString(),
                        Data = ingresosMontoCategoriasCuentasxDia // Clonar la lista para evitar referencias compartidas
                    });

                }

              
            }

            for (int mes = 1; mes <= 12; mes++)
            {

                //var TipoxAreaDia = new List<ValoresGrafica>();
                int diasEnElMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);

                for (int dia = 1; dia <= diasEnElMes; dia++)
                {
                    var ingresoCategoriasCuentasxDia = new List<ValoresGrafica>();


                    foreach (var categoria in categoriasCuentasContables)
                    {
                        var cuentas = cuentasContables.
                               Where(cc => cc.CuentaContable != null && cc.CuentaContable.CategoriaCuentaId == categoria.Id)
                               .Where(dc => dc.Fecha.Month == mes && dc.Fecha.Day == dia)
                               //.Select(dc => dc.Gasto)
                               .ToList();
                        foreach (var cuenta in cuentas)
                        {


                            ingresoCategoriasCuentasxDia.Add(new ValoresGrafica
                            {
                                Name = cuenta.CuentaContable.NombreCuenta.ToString(),
                                Y = cuenta.Gasto

                            });

                            IngresosGeneralesxMes.Add(new IngresosDia
                            {
                                Id = categoria.Nombre,
                                Data = ingresoCategoriasCuentasxDia

                            });
                        }









                    }


                }




            }

            return IngresosGeneralesxMes;


        }

        public List<decimal> GetIngresosVentasGenerales()
        {

            var ingresosVentasGenerales = new List<decimal>();

            var detalleCajas = GetDetalleCajas();

            var ventas = GetVentas(); 

            for (int mes = 1; mes <= 12; mes++)
            {
                var IngresosVentasMensuales = 
                    detalleCajas
                   .Where(I => I.Fecha.Month == mes)
                   .Select(I => I.Ingreso)
                   .Concat(ventas.Where(v => v.FechaVenta.Month == mes)
                   .Select(v => v.MontoPago))
                   .ToList();

                ingresosVentasGenerales.Add(IngresosVentasMensuales.Sum());

            }

            return ingresosVentasGenerales;


        }

        public List<IngresosVentasDia> GetIngresosVentasGeneralesxDia()
        {
            var IngresosVentasGeneralesxMes = new List<IngresosVentasDia>();
            
            var detalleCajas = GetDetalleCajas();
            var ventas = GetVentas();

            for (int mes = 1; mes <= 12; mes++)
            {

                var ingresosVentasMontoxDia = new List<ValoresGrafica>();
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var ingresosVentasMesDia =
                    detalleCajas
                   .Where(I => I.Fecha.Month == mes && I.Fecha.Day == dia)
                   .Select(I => I.Ingreso)
                   .Concat(ventas.Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                   .Select(v => v.MontoPago))
                   .ToList();

                    ingresosVentasMontoxDia
                     .Add(new ValoresGrafica
                     {
                         Name = dia.ToString(),
                         Y = Convert.ToDecimal(ingresosVentasMesDia.Sum()),
                         drilldown = dia.ToString() + mes.ToString()

                     }); ; ;
                }

                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

                var IngresosVentasxDia = new IngresosVentasDia
                {
                    //Name = nombreMes,
                    Id = nombreMes,
                    Data = ingresosVentasMontoxDia,

                };

                IngresosVentasGeneralesxMes.Add(IngresosVentasxDia);
            }



            for (int mes = 1; mes <= 12; mes++)
            {

               
              
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                   var IngresoOventa = new List<ValoresGrafica>();

                    var valorIngreso = detalleCajas
                   .Where(I => I.Fecha.Month == mes && I.Fecha.Day == dia)
                   .Select(I => I.Ingreso)
                   .Sum();
                   

                    IngresoOventa
                   .Add(new ValoresGrafica
                   {
                       Name ="Ingresos",
                       Y = Convert.ToDecimal(valorIngreso),


                   });

                    var valorVenta =ventas.Where(v => v.FechaVenta.Month == mes && v.FechaVenta.Day == dia)
                   .Select(v => v.MontoPago)
                   .Sum();

                    IngresoOventa
                    .Add(new ValoresGrafica
                    {
                        Name = "Ventas",
                        Y = Convert.ToDecimal(valorVenta),


                    });

                   var  IngresosVentasxDia = new IngresosVentasDia
                    {
                        //Name = nombreMes,
                        Id = dia.ToString() + mes.ToString(),
                        Data = IngresoOventa,

                    };

                    IngresosVentasGeneralesxMes.Add(IngresosVentasxDia);

                }


                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

               

                
            }


            return IngresosVentasGeneralesxMes;


        }


        public List<decimal> GetGastosGenerales()
        {

            var gastosGenerales = new List<decimal>();

            var todosgastos = GetDetalleCajas();

            for (int mes = 1; mes <= 12; mes++)
            {
                var gastosMensuales = todosgastos
                   .Where(I => I.Fecha.Month == mes)
                     .Select(I => I.Gasto)
                     .ToList();
                gastosGenerales.Add(gastosMensuales.Sum());

            }

            return gastosGenerales;


        }

        public List<GastosDia> GetgastosGeneralesxDia()
        {
            var gastosGeneralesxMes = new List<GastosDia>();
            var todosgastos = GetDetalleCajas();

            for (int mes = 1; mes <= 12; mes++)
            {

                var gastosMontoxDia = new List<ValoresGrafica>();
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var gastosMesDia =
                       todosgastos.Where(I => I.Fecha.Month == mes && I.Fecha.Day == dia)
                         .Select(I => I.Gasto);

                    gastosMontoxDia
                     .Add(new ValoresGrafica
                     {
                         Name = dia.ToString(),
                         Y = Convert.ToDecimal(gastosMesDia.Sum()),
                         drilldown = dia.ToString() + mes.ToString()

                     });

                }

                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

                var gastosxDia = new GastosDia
                {
                    //Name = nombreMes,
                    Id = nombreMes,
                    Data = gastosMontoxDia,

                };

                gastosGeneralesxMes.Add(gastosxDia);
            }
            var cuentasdecategoria = new List<CuentaContable>();
            var detalleCaja = new List<DetalleCaja>();

            var categoriasCuentasContables = GetCategoriasCuentasContables();

            var cuentasContables = GetDetalleCajasCuentaContable();

            var prueba = cuentasdecategoria;
            for (int mes = 1; mes <= 12; mes++)
            {

                //var TipoxAreaDia = new List<ValoresGrafica>();
                int diasEnElMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);

                for (int dia = 1; dia <= diasEnElMes; dia++)
                {

					var gastosMontoCategoriasCuentasxDia = new List<ValoresGrafica>();

					foreach (var categoria in categoriasCuentasContables)
                    {
						

						var montoCategoria =
                              cuentasContables.
                              Where(cc => cc.CuentaContable != null && cc.CuentaContable.CategoriaCuentaId == categoria.Id)
                              .Where(dc => dc.Fecha.Month == mes && dc.Fecha.Day == dia)
                              .Select(dc => dc.Gasto)
                              .Sum();



                        gastosMontoCategoriasCuentasxDia.Add(new ValoresGrafica
                        {
                            Name = categoria.Nombre,
                            Y = montoCategoria,
                            drilldown = categoria.Nombre + dia.ToString() + mes.ToString()
                        }); 

						

					};

					gastosGeneralesxMes.Add(new GastosDia
					{
						Id = dia.ToString() + mes.ToString(),
						Data = gastosMontoCategoriasCuentasxDia // Clonar la lista para evitar referencias compartidas
					});




				}


            }

            for (int mes = 1; mes <= 12; mes++)
            {

                //var TipoxAreaDia = new List<ValoresGrafica>();
                int diasEnElMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);

                for (int dia = 1; dia <= diasEnElMes; dia++)
                {

					

					foreach (var categoria in categoriasCuentasContables)
                    {
                        var cuentas = cuentasContables.
                               Where(cc => cc.CuentaContable != null && cc.CuentaContable.CategoriaCuentaId == categoria.Id)
                               .Where(dc => dc.Fecha.Month == mes && dc.Fecha.Day == dia)
                               //.Select(dc => dc.Gasto)
                               .ToList();

						var gastosCategoriasCuentasxDia = new List<ValoresGrafica>();

						foreach (var cuenta in cuentas)
                        {


							
							gastosCategoriasCuentasxDia.Add(new ValoresGrafica
                            {
                                Name = cuenta.CuentaContable.NombreCuenta.ToString(),
                                Y = cuenta.Gasto

                            });

                           
                        }


						gastosGeneralesxMes.Add(new GastosDia
						{
							Id = categoria.Nombre + dia.ToString() + mes.ToString(),
							Data = gastosCategoriasCuentasxDia

						});






					}


                }

               


            }
            return gastosGeneralesxMes;

        }

        public List<decimal> GetGastosComprasGenerales()
        {

            var gastosComprasGenerales = new List<decimal>();

            var detalleCajas = GetDetalleCajas();

            var compras = GetCompras();

            for (int mes = 1; mes <= 12; mes++)
            {
                var GastosComprasMensuales =
                    detalleCajas
                   .Where(I => I.Fecha.Month == mes)
                   .Select(I => I.Gasto)
                   .Concat(compras.Where(v => v.FechaCompra.Month == mes)
                   .Select(v => v.ValorTotal))
                   .ToList();

                gastosComprasGenerales.Add(GastosComprasMensuales.Sum());

            }

            return gastosComprasGenerales;


        }

        public List<GastosComprasDia> GetGastosComprasGeneralesxDia()
        {
            var GastosComprasGeneralesxMes = new List<GastosComprasDia>();

            var detalleCajas = GetDetalleCajas();
            var compras = GetCompras();

            for (int mes = 1; mes <= 12; mes++)
            {

                var gastosComprasMontoxDia = new List<ValoresGrafica>();
                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var gastosCompraMesDia =
                    detalleCajas
                   .Where(I => I.Fecha.Month == mes && I.Fecha.Day == dia)
                   .Select(I => I.Ingreso)
                   .Concat(compras.Where(v => v.FechaCompra.Month == mes && v.FechaCompra.Day == dia)
                   .Select(v => v.ValorTotal))
                   .ToList();

                    gastosComprasMontoxDia
                     .Add(new ValoresGrafica
                     {
                         Name = dia.ToString(),
                         Y = Convert.ToDecimal(gastosCompraMesDia.Sum()),
                         drilldown = dia.ToString() + mes.ToString()

                     }); ; ;
                }

                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula

                var GastosComprasxDia = new GastosComprasDia
                {
                    //Name = nombreMes,
                    Id = nombreMes,
                    Data = gastosComprasMontoxDia,

                };

                GastosComprasGeneralesxMes.Add(GastosComprasxDia);
            }



            for (int mes = 1; mes <= 12; mes++)
            {



                int cantidadDiasMes = DateTime.DaysInMonth(DateTime.Now.Year, mes);
                for (int dia = 1; dia <= cantidadDiasMes; dia++)
                {
                    var GastosoCompra = new List<ValoresGrafica>();

                    var valorGasto = detalleCajas
                   .Where(I => I.Fecha.Month == mes && I.Fecha.Day == dia)
                   .Select(I => I.Gasto)
                   .Sum();


                    GastosoCompra
                   .Add(new ValoresGrafica
                   {
                       Name = "Gastos",
                       Y = Convert.ToDecimal(valorGasto),


                   });

                    var valorCompra = compras.Where(v => v.FechaCompra.Month == mes && v.FechaCompra.Day == dia)
                   .Select(v => v.ValorTotal)
                   .Sum();

                    GastosoCompra
                    .Add(new ValoresGrafica
                    {
                        Name = "Compra",
                        Y = Convert.ToDecimal(valorCompra),


                    });

                    var GastosComprasxDia = new GastosComprasDia
                    {
                        //Name = nombreMes,
                        Id = dia.ToString() + mes.ToString(),
                        Data = GastosoCompra,

                    };

                    GastosComprasGeneralesxMes.Add(GastosComprasxDia);

                }


                var culturaEspañola = new CultureInfo("es-ES");
                var nombreMes = culturaEspañola.DateTimeFormat.GetMonthName(mes);
                nombreMes = char.ToUpper(nombreMes[0]) + nombreMes.Substring(1); // Convertir la primera letra a mayúscula




            }


            return GastosComprasGeneralesxMes;


        }


    }
}


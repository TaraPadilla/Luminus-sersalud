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
    public class CajaService : ICajaService
    {
        private readonly UserManager<User> _userManager = null;
        private readonly IUser _userRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly ICaja _cajaRepository = null;

        public CajaService(
            ISucursal sucursalRepository,
            ICaja cajaRepository,
            IUser userRepository,
            UserManager<User> userManager)
        {
            _sucursalRepository = sucursalRepository;
            _cajaRepository = cajaRepository;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        public CajaDetallesViewModel GetDetallesCaja(int id)
        {
            var caja = _cajaRepository.GetCaja(id);

            #region SUCURSALES

            var listaSucursales = _sucursalRepository.GetList();

            #endregion


            //Variable Booleana que determina si la caja
            //de la que se consultan detalles es GLOBAL
            var esGlobal = caja.AmbienteId == (int)AmbienteEnum.Global;

            #region SUBCAJAS

            var listaSubcajas = new List<CajaDetallesSubcajaViewModel>();
            //Si es GLOBAL deben consultarse las SUBCAJAS
            //Se crea una variable subcajas para ser utilizada en varias partes del metodo
            //ya que se debe consultar ingresos, ventas, compras y egresos de cada una
            var subcajas = new List<Caja>();
            //Variable Booleana que determina si la caja GLOBAL contiene subcajas
            var existenSubcajas = false;
            if (esGlobal)
            {
                var fechaApertura = caja.FechaApertura;
                var fechaCierre = caja.FechaCierre;
                subcajas = _cajaRepository.GetSubcajas(fechaApertura, fechaCierre, caja.SucursalId, caja.Id);
                existenSubcajas = subcajas != null && subcajas.Count > 0;
            }

            if (existenSubcajas)
            {
                foreach (var subcaja in subcajas)
                {
                    var sucursalNombre = "-";
                    if (subcaja.AmbienteId == (int)AmbienteEnum.Global)
                    {
                        //Cuando el ambiente de una caja es global
                        //y el dato SucursalId viene NULL significa que la caja es para TODAS las sucursales
                        sucursalNombre = subcaja.Sucursal != null ? subcaja.Sucursal.NombreSucursal : "TODAS";
                    }
                    else
                    {
                        //Cuando el ambiente de una caja NO es global
                        //y el dato SucursalId viene NULL significa que la caja no tiene seleccionada una sucursal
                        //El signo "-" se coloca para capturar la excepcion de que el dato
                        //viene vacio, ya que NO deberia venir NULO
                        sucursalNombre = subcaja.Sucursal != null ? subcaja.Sucursal.NombreSucursal : "-";
                    }
                    listaSubcajas.Add(new CajaDetallesSubcajaViewModel
                    {
                        Id = subcaja.Id,
                        EstadoCaja = subcaja.EstadoCaja,
                        SucursalId = subcaja.SucursalId,
                        SucursalNombre = sucursalNombre,
                        AmbienteId = subcaja.AmbienteId,
                        AmbienteNombre = subcaja.Ambiente != null ? subcaja.Ambiente.NombreAmbiente : "-",
                        FechaApertura = subcaja.FechaApertura,
                        ResponsableApertura = subcaja.ResponsableApertura != null ? subcaja.ResponsableAperturaText : "-",
                        MontoApertura = subcaja.MontoApertura,
                        Ingresos = subcaja.DetalleCajas.Sum(a => a.Ingreso),
                        Gastos = subcaja.DetalleCajas.Sum(a => a.Gasto),
                        FechaCierre = subcaja.FechaCierre,
                        ResponsableCierre = subcaja.ResponsableCierre != null ? subcaja.ResponsableCierreText : "-",
                        TotalCierre = subcaja.MontoApertura
                        + subcaja.DetalleCajas.Sum(a => a.Ingreso)
                        - subcaja.DetalleCajas.Sum(a => a.Gasto)
                    });
                }
            }

            #endregion

            #region VENTAS

            var listaVentas = new List<CajaDetallesVentaViewModel>();
            var ventasBd = new List<DetalleCaja>();

            ventasBd = caja.DetalleCajas
                .Where(a => a.VentaId != null).OrderByDescending(a => a.Venta.FechaVenta).ToList();

            if (esGlobal)
            {
                //Se ejecuta cuando la caja es de tipo global

                #region Ventas Subcajas


                //Ahora se agregan las ventas de cada una de ellas
                //a las ventasBd que se obtienen de la caja global
                if (existenSubcajas)
                {
                    foreach (var subcaja in subcajas)
                    {
                        var ventasSubcaja = subcaja.DetalleCajas
                            .Where(a => a.VentaId != null)
                            .OrderByDescending(a => a.Venta.FechaVenta).ToList();
                        ventasBd.AddRange(ventasSubcaja);
                    }
                }

                #endregion


                //Finalmente las ventas de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                ventasBd = ventasBd.OrderByDescending(a => a.Venta.FechaVenta).ToList();
                foreach (var ventaBd in ventasBd)
                {
                    //Formas de pago
                    var formasPago = new List<CajaDetallesVentaFormaPagoViewModel>();
                    foreach (var pago in ventaBd.Venta.Pagos)
                    {
                        formasPago.Add(new CajaDetallesVentaFormaPagoViewModel
                        {
                            FormaPagoId = pago.FormaPagoId,
                            NombreFormaPago = pago.FormaPago.NombreFormaPago,
                            Monto = pago.Monto
                        });
                    }

                    // DETERMINAR SI ES VENTA DE EMERGENCIA
                    var esEmergencia = ventaBd.Venta.Origen == "EMERGENCIA";

                    listaVentas.Add(new CajaDetallesVentaViewModel
                    {
                        VentaId = ventaBd.Venta.Id,
                        AmbienteId = Convert.ToInt32(ventaBd.Caja.AmbienteId),
                        AmbienteNombre = ventaBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = ventaBd.Caja.SucursalId,
                        SucursalNombre = ventaBd.Caja.Sucursal != null ? ventaBd.Caja.Sucursal.NombreSucursal : "TODAS",
                        Descripcion = ventaBd.Descripcion,
                        Cliente = ventaBd.Venta.Clientes != null ? ventaBd.Venta.Clientes.Nombre : "-",
                        Vendedor = ventaBd.Venta.Empleado != null ? ventaBd.Venta.Empleado.NombreYApellidos : "-",
                        FechaVenta = ventaBd.Venta.FechaVenta,
                        FormasPago = formasPago,
                        Comprobante = ventaBd.Venta.NoComprobante != null ? ventaBd.Venta.NoComprobante : "CF",
                        Total = ventaBd.Ingreso,
                        EsEmergencia = esEmergencia, // NUEVA PROPIEDAD
                        OrigenVenta = ventaBd.Venta.Origen // NUEVA PROPIEDAD
                    });
                }
            }
            else
            {
                //Finalmente las ventas de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                ventasBd = ventasBd.OrderByDescending(a => a.Venta.FechaVenta).ToList();
                foreach (var ventaBd in ventasBd)
                {
                    //Formas de pago
                    var formasPago = new List<CajaDetallesVentaFormaPagoViewModel>();
                    foreach (var pago in ventaBd.Venta.Pagos)
                    {
                        formasPago.Add(new CajaDetallesVentaFormaPagoViewModel
                        {
                            FormaPagoId = pago.FormaPagoId,
                            NombreFormaPago = pago.FormaPago.NombreFormaPago,
                            Monto = pago.Monto
                        });
                    }

                    // DETERMINAR SI ES VENTA DE EMERGENCIA
                    var esEmergencia = ventaBd.Venta.Origen == "EMERGENCIA";

                    listaVentas.Add(new CajaDetallesVentaViewModel
                    {
                        VentaId = ventaBd.Venta.Id,
                        AmbienteId = Convert.ToInt32(ventaBd.Caja.AmbienteId),
                        AmbienteNombre = ventaBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = ventaBd.Caja.SucursalId,
                        SucursalNombre = ventaBd.Caja.Sucursal != null ? ventaBd.Caja.Sucursal.NombreSucursal : "-",
                        Descripcion = ventaBd.Descripcion,
                        Cliente = ventaBd.Venta.Clientes != null ? ventaBd.Venta.Clientes.Nombre : "-",
                        Vendedor = ventaBd.Venta.Empleado != null ? ventaBd.Venta.Empleado.NombreYApellidos : "-",
                        FechaVenta = ventaBd.Venta.FechaVenta,
                        FormasPago = formasPago,
                        Comprobante = ventaBd.Venta.NoComprobante != null ? ventaBd.Venta.NoComprobante : "CF",
                        Total = ventaBd.Ingreso,
                        EsEmergencia = esEmergencia, // NUEVA PROPIEDAD
                        OrigenVenta = ventaBd.Venta.Origen // NUEVA PROPIEDAD
                    });
                }
            }

            #endregion

            #region MONTOS

            var listaMontos = new List<CajaDetallesMontoViewModel>();

            // Agregar TODAS las sucursales con TODOS los ambientes
            listaMontos.Add(new CajaDetallesMontoViewModel
            {
                SucursalNombre = "TODAS",
                AmbienteId = (int)AmbienteEnum.Global,
                AmbienteNombre = "Global"
            });

            if (listaSucursales != null)
            {
                foreach (var sucursal in listaSucursales)
                {
                    foreach (AmbienteEnum ambiente in Enum.GetValues(typeof(AmbienteEnum)))
                    {
                        listaMontos.Add(new CajaDetallesMontoViewModel
                        {
                            SucursalId = sucursal.Id,
                            SucursalNombre = sucursal.NombreSucursal,
                            AmbienteId = (int)ambiente,
                            AmbienteNombre = ambiente.ToString()
                        });
                    }
                }
            }

            // PRIMER RECORRIDO: TODAS LAS VENTAS (montos normales)
            if (listaVentas != null)
            {
                foreach (var venta in listaVentas)
                {
                    if (venta.FormasPago != null)
                    {
                        var regListaMontos = listaMontos
                            .FirstOrDefault(a => a.AmbienteId == venta.AmbienteId
                                              && a.SucursalId == venta.SucursalId);

                        if (regListaMontos != null)
                        {
                            foreach (var formaPago in venta.FormasPago)
                            {
                                regListaMontos.Total += formaPago.Monto;

                                switch (formaPago.FormaPagoId)
                                {
                                    case (int)FormaPagoEnum.Efectivo: regListaMontos.Efectivo += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.TarjetaVisa: regListaMontos.Visa += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.TarjetaMastercard: regListaMontos.MasterCard += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.Cheques: regListaMontos.Cheques += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.Transferencia: regListaMontos.Transferencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.VisaLink: regListaMontos.Visalink += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.VisaNet: regListaMontos.Visanet += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.Seguro: regListaMontos.Seguro += formaPago.Monto; break;
                                }
                            }
                        }
                    }
                }
            }

            // SEGUNDO RECORRIDO: SOLO EMERGENCIAS (montos de emergencia)
            if (listaVentas != null)
            {
                foreach (var venta in listaVentas.Where(v => v.OrigenVenta == "EMERGENCIA"))
                {
                    if (venta.FormasPago != null)
                    {
                        var regListaMontos = listaMontos
                            .FirstOrDefault(a => a.AmbienteId == venta.AmbienteId
                                              && a.SucursalId == venta.SucursalId);

                        if (regListaMontos != null)
                        {
                            foreach (var formaPago in venta.FormasPago)
                            {
                                regListaMontos.TotalEmergencia += formaPago.Monto;

                                switch (formaPago.FormaPagoId)
                                {
                                    case (int)FormaPagoEnum.Efectivo: regListaMontos.EfectivoEmergencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.TarjetaVisa: regListaMontos.VisaEmergencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.TarjetaMastercard: regListaMontos.MasterCardEmergencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.Cheques: regListaMontos.ChequesEmergencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.Transferencia: regListaMontos.TransferenciaEmergencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.VisaLink: regListaMontos.VisalinkEmergencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.VisaNet: regListaMontos.VisanetEmergencia += formaPago.Monto; break;
                                    case (int)FormaPagoEnum.Seguro: regListaMontos.SeguroEmergencia += formaPago.Monto; break;
                                }
                            }
                        }
                    }
                }
            }

            // Eliminar registros en cero
            listaMontos = listaMontos.Where(a => a.Total != 0).ToList();

            #endregion

            #region COMPRAS

            var listaCompras = new List<CajaDetallesCompraViewModel>();
            var comprasBd = new List<DetalleCaja>();

            comprasBd = caja.DetalleCajas
                .Where(a => a.CompraId != null).OrderByDescending(a => a.Compra.FechaCompra).ToList();

            if (esGlobal)
            {
                //Se ejecuta cuando la caja es de tipo global

                #region Compras Subcajas


                //Ahora se agregan las compras de cada una de ellas
                //a las comprasBD que se obtienen de la caja global
                if (existenSubcajas)
                {
                    foreach (var subcaja in subcajas)
                    {
                        var comprasSubcaja = subcaja.DetalleCajas
                            .Where(a => a.CompraId != null)
                            .OrderByDescending(a => a.Compra.FechaCompra).ToList();
                        comprasBd.AddRange(comprasSubcaja);
                    }
                }

                #endregion


                //Finalmente las compras de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                comprasBd = comprasBd.OrderByDescending(a => a.Compra.FechaCompra).ToList();
                foreach (var compraBd in comprasBd)
                {
                    listaCompras.Add(new CajaDetallesCompraViewModel
                    {
                        Id = compraBd.Id,
                        AmbienteId = Convert.ToInt32(compraBd.Caja.AmbienteId),
                        AmbienteNombre = compraBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = compraBd.Caja.SucursalId,
                        SucursalNombre = compraBd.Caja.Sucursal != null ? compraBd.Caja.Sucursal.NombreSucursal : "TODAS",
                        Descripcion = compraBd.Descripcion,
                        Proveedor = compraBd.Compra.Proveedor != null ? compraBd.Compra.Proveedor.Nombre : "-",
                        Empleado = compraBd.Compra.Empleado != null ? compraBd.Compra.Empleado.NombreYApellidos : "-",
                        FechaCompra = compraBd.Compra.FechaCompra,
                        Comprobante = compraBd.Compra.NoComprobante != null ? compraBd.Compra.NoComprobante : "CF",
                        Total = compraBd.Gasto
                    });
                }
            }
            else
            {
                //Finalmente las compras de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                comprasBd = comprasBd.OrderByDescending(a => a.Compra.FechaCompra).ToList();
                foreach (var compraBd in comprasBd)
                {
                    listaCompras.Add(new CajaDetallesCompraViewModel
                    {
                        Id = compraBd.Id,
                        AmbienteId = Convert.ToInt32(compraBd.Caja.AmbienteId),
                        AmbienteNombre = compraBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = compraBd.Caja.SucursalId,
                        SucursalNombre = compraBd.Caja.Sucursal != null ? compraBd.Caja.Sucursal.NombreSucursal : "-",
                        Descripcion = compraBd.Descripcion,
                        Proveedor = compraBd.Compra.Proveedor != null ? compraBd.Compra.Proveedor.Nombre : "-",
                        Empleado = compraBd.Compra.Empleado != null ? compraBd.Compra.Empleado.NombreYApellidos : "-",
                        FechaCompra = compraBd.Compra.FechaCompra,
                        Comprobante = compraBd.Compra.NoComprobante != null ? compraBd.Compra.NoComprobante : "CF",
                        Total = compraBd.Gasto
                    });
                }
            }

            #endregion

            #region INGRESOS

            var listaIngresos = new List<CajaDetallesIngresoViewModel>();
            var ingresosBd = new List<DetalleCaja>();

            ingresosBd = caja.DetalleCajas
                .Where(a => a.Ingreso != 0.00m
                && a.VentaId == null
            /*&& a.VentaServicioId == null*/).OrderByDescending(a => a.Fecha).ToList();

            if (esGlobal)
            {
                //Se ejecuta cuando la caja es de tipo global

                #region Ingresos Subcajas


                //Ahora se agregan los ingresos de cada una de ellas
                //a los ingresosBD que se obtienen de la caja global
                if (existenSubcajas)
                {
                    foreach (var subcaja in subcajas)
                    {
                        var ingresosSubcaja = subcaja.DetalleCajas
                            .Where(a => a.Ingreso != 0.00m
                            && a.VentaId == null
                        /*&& a.VentaServicioId == null*/).OrderByDescending(a => a.Fecha).ToList();
                        ingresosBd.AddRange(ingresosSubcaja);
                    }
                }

                #endregion


                //Finalmente los ingresos de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                ingresosBd = ingresosBd.OrderByDescending(a => a.Fecha).ToList();
                foreach (var ingresoBd in ingresosBd)
                {
                    listaIngresos.Add(new CajaDetallesIngresoViewModel
                    {
                        Id = ingresoBd.Id,
                        AmbienteId = Convert.ToInt32(ingresoBd.Caja.AmbienteId),
                        AmbienteNombre = ingresoBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = ingresoBd.Caja.SucursalId,
                        SucursalNombre = ingresoBd.Caja.Sucursal != null ? ingresoBd.Caja.Sucursal.NombreSucursal : "TODAS",
                        Banco = ingresoBd.Banco != null ? ingresoBd.Banco.Nombre : "-",
                        Cuenta = ingresoBd.Cuenta != null ? ingresoBd.Cuenta.NombreCuenta : "-",
                        Descripcion = ingresoBd.Descripcion,
                        FechaIngreso = ingresoBd.Fecha,
                        NumeroComprobante = ingresoBd.NumeroComprabante,
                        Total = ingresoBd.Ingreso
                    });
                }
            }
            else
            {
                //Finalmente los ingresos de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                ingresosBd = ingresosBd.OrderByDescending(a => a.Fecha).ToList();
                foreach (var ingresoBd in ingresosBd)
                {
                    listaIngresos.Add(new CajaDetallesIngresoViewModel
                    {
                        Id = ingresoBd.Id,
                        AmbienteId = Convert.ToInt32(ingresoBd.Caja.AmbienteId),
                        AmbienteNombre = ingresoBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = ingresoBd.Caja.SucursalId,
                        SucursalNombre = ingresoBd.Caja.Sucursal != null ? ingresoBd.Caja.Sucursal.NombreSucursal : "-",
                        Banco = ingresoBd.Banco != null ? ingresoBd.Banco.Nombre : "-",
                        Cuenta = ingresoBd.Cuenta != null ? ingresoBd.Cuenta.NombreCuenta : "-",
                        Descripcion = ingresoBd.Descripcion,
                        FechaIngreso = ingresoBd.Fecha,
                        NumeroComprobante = ingresoBd.NumeroComprabante,
                        Total = ingresoBd.Ingreso
                    });
                }
            }

            #endregion

            #region EGRESOS

            var listaEgresos = new List<CajaDetallesEgresoViewModel>();
            var egresosBd = new List<DetalleCaja>();

            egresosBd = caja.DetalleCajas
                .Where(a => a.Gasto != 0.00m
                && a.CompraId == null).OrderByDescending(a => a.Fecha).ToList();

            if (esGlobal)
            {
                //Se ejecuta cuando la caja es de tipo global

                #region Egresos Subcajas


                //Ahora se agregan los egresos de cada una de ellas
                //a los egresosBD que se obtienen de la caja global
                if (existenSubcajas)
                {
                    foreach (var subcaja in subcajas)
                    {
                        var egresosSubcaja = subcaja.DetalleCajas
                            .Where(a => a.Gasto != 0.00m
                            && a.CompraId == null).OrderByDescending(a => a.Fecha).ToList();
                        egresosBd.AddRange(egresosSubcaja);
                    }
                }

                #endregion


                //Finalmente los egresos de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                egresosBd = egresosBd.OrderByDescending(a => a.Fecha).ToList();
                foreach (var egresoBd in egresosBd)
                {
                    listaEgresos.Add(new CajaDetallesEgresoViewModel
                    {
                        Id = egresoBd.Id,
                        AmbienteId = Convert.ToInt32(egresoBd.Caja.AmbienteId),
                        AmbienteNombre = egresoBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = egresoBd.Caja.SucursalId,
                        SucursalNombre = egresoBd.Caja.Sucursal != null ? egresoBd.Caja.Sucursal.NombreSucursal : "TODAS",
                        CuentaContable = egresoBd.CuentaContable != null ? egresoBd.CuentaContable.NombreCuenta : "-",
                        Descripcion = egresoBd.Descripcion,
                        FechaEgreso = egresoBd.Fecha,
                        NumeroComprobante = egresoBd.NumeroComprabante,
                        Total = egresoBd.Gasto
                    });
                }
            }
            else
            {
                //Finalmente los egresos de BD (Base de Datos) se agregan a la lista que se enviara
                //al VIEWMODEL
                egresosBd = egresosBd.OrderByDescending(a => a.Fecha).ToList();
                foreach (var egresoBd in egresosBd)
                {
                    listaEgresos.Add(new CajaDetallesEgresoViewModel
                    {
                        Id = egresoBd.Id,
                        AmbienteId = Convert.ToInt32(egresoBd.Caja.AmbienteId),
                        AmbienteNombre = egresoBd.Caja.Ambiente.NombreAmbiente,
                        SucursalId = egresoBd.Caja.SucursalId,
                        SucursalNombre = egresoBd.Caja.Sucursal != null ? egresoBd.Caja.Sucursal.NombreSucursal : "-",
                        CuentaContable = egresoBd.CuentaContable != null ? egresoBd.CuentaContable.NombreCuenta : "-",
                        Descripcion = egresoBd.Descripcion,
                        FechaEgreso = egresoBd.Fecha,
                        NumeroComprobante = egresoBd.NumeroComprabante,
                        Total = egresoBd.Gasto
                    });
                }
            }

            #endregion

            #region TotalesCaja
            var totalVentas = listaVentas.Sum(a => a.Total);
            var totalIngresos = listaIngresos.Sum(a => a.Total);
            //El totalIngresosCaja incluye INGRESOS y VENTAS
            var totalIngresosCaja = totalVentas + totalIngresos;

            var totalCompras = listaCompras.Sum(a => a.Total);
            var totalEgresos = listaEgresos.Sum(a => a.Total);
            //El totalEgresosCaja incluye EGRESOS y COMPRAS
            var totalEgresosCaja = totalCompras + totalEgresos;

            var totalComprasCredito = (comprasBd.Select(x => x.Compra)).Where(x => x.TipoCompraId == (int)TipoCompraEnum.Credito).ToList().Sum(x => x.ValorTotal);


            #endregion

            //Se crea el ViewModel que se enviara a la Vista
            var cajaDetalles = new CajaDetallesViewModel()
            {
                Id = caja.Id,
                ListaSucursales = listaSucursales,
                FechaApertura = caja.FechaApertura,
                ResponsableApertura = caja.ResponsableApertura != null ? caja.ResponsableAperturaText : "-",
                FechaCierre = caja.FechaCierre,
                ResponsableCierre = caja.ResponsableCierre != null ? caja.ResponsableCierreText : "-",
                MontoApertura = caja.MontoApertura,
                EstadoCaja = caja.EstadoCaja,
                SucursalId = caja.SucursalId,
                SucursalNombre = caja.Sucursal != null ? caja.Sucursal.NombreSucursal : "TODAS",
                AmbienteId = caja.AmbienteId ?? 0,
                AmbienteNombre = caja.Ambiente.NombreAmbiente,
                Ventas = listaVentas,
                Montos = listaMontos,
                Ingresos = listaIngresos,
                Egresos = listaEgresos,
                Compras = listaCompras,
                Subcajas = listaSubcajas,
                TotalIngresosCaja = totalIngresosCaja,
                TotalEgresosCaja = totalEgresosCaja,
                TotalComprasCredito = totalComprasCredito
            };

            return cajaDetalles;
        }

        public void ReabrirCaja(int cajaId)
        {
            _cajaRepository.ReabrirCaja(cajaId);
        }
        public bool VerificarEmpleado(string guidUsuario, int empleadoId)
        {
            var usuario = _userRepository.GetbyId(guidUsuario);
            if (usuario != null)
            {
                return usuario.EmpleadoId == empleadoId;
            }
            else
            {
                return false;
            }
        }

        public void CerrarCaja(User usuario, int cajaId)
        {
            var caja = _cajaRepository.GetCaja(cajaId);
            caja.EstadoCaja = false;
            caja.FechaCierre = DateTime.Now;
            caja.ResponsableCierre = usuario;

            _cajaRepository.Update(caja);
        }
    }
}

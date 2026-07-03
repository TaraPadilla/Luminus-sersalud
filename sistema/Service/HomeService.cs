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
    public class HomeService : IHomeService
    {
        private readonly UserManager<User> _userManager = null;
        private readonly IUser _userRepository = null;
        private readonly ISucursal _sucursalRepository = null;
        private readonly ICaja _cajaRepository = null;
        private readonly IProducto _productoRepository = null;
        private readonly ILaboratorioClinico _labClinicoRepository = null;
        private readonly ICompra _compraRepository = null;

        public HomeService(
            ISucursal sucursalRepository,
            ICaja cajaRepository,
            IUser userRepository,
            IProducto productoRepository,
            ILaboratorioClinico labClinicoRepository,
            ICompra compraRepository,
            UserManager<User> userManager)
        {
            _sucursalRepository = sucursalRepository;
            _cajaRepository = cajaRepository;
            _userManager = userManager;
            _userRepository = userRepository;
            _productoRepository = productoRepository;
            _labClinicoRepository = labClinicoRepository;
            _compraRepository = compraRepository;
        }

        public List<HomeProductoStockMinimoViewModel> GetComprasStockMinimo()
        {
            var listaStockMinimo = new List<HomeProductoStockMinimoViewModel>();
            var productos = _productoRepository.GetProductos(null, null,false);
            if (productos != null)
            {
                foreach (var producto in productos)
                {
                    var registrosInventario = producto.ProductosInventario.ToList();
                    if (registrosInventario != null)
                    {
                        foreach (var regInventario in registrosInventario)
                        {
                            if (regInventario.Stock <= regInventario.StockMinimo)
                            {
                                var bodega = regInventario.Bodega ?? new Bodega();
                                var tipoBodega = producto.TipoBodega ?? new TipoBodega();
                                listaStockMinimo.Add(new HomeProductoStockMinimoViewModel
                                {
                                    ProductoNombre = producto.NombreProducto,
                                    AmbienteNombre = tipoBodega.DescripcionBodega,
                                    ProductoCodigo = producto.CodigoReferencia,
                                    ProductoStock = regInventario.Stock,
                                    ProductoUbicacion = bodega.BodegaSucursalText
                                });
                            }
                        }
                    }
                }
            }
            return listaStockMinimo;
        }
        public List<HomeProductoStockMinimoViewModel> GetProductosStockMinimo(int tipoBodegaId)
        {
            var listaStockMinimo = new List<HomeProductoStockMinimoViewModel>();
            var productos = _productoRepository.GetProductos(tipoBodegaId, null,false);
            if (productos != null)
            {
                foreach (var producto in productos)
                {
                    var registrosInventario = producto.ProductosInventario.ToList();
                    if (registrosInventario != null)
                    {
                        foreach (var regInventario in registrosInventario)
                        {
                            if (regInventario.Stock <= regInventario.StockMinimo)
                            {

                                var bodega = regInventario.Bodega ?? new Bodega();
                                listaStockMinimo.Add(new HomeProductoStockMinimoViewModel
                                {
                                    ProductoNombre = producto.NombreProducto,
                                    ProductoCodigo = producto.CodigoReferencia,
                                    ProductoStock = regInventario.Stock,
                                    ProductoUbicacion = bodega.BodegaSucursalText
                                });
                            }
                        }
                    }
                }
            }
            return listaStockMinimo;
        }
        public List<HomeProductoProximoVencerViewModel> GetProductosProximosVencer(int tipoBodegaId)
        {
            var listaProximosVencer = new List<HomeProductoProximoVencerViewModel>();
            var productos = _productoRepository.GetProductos(tipoBodegaId, null, false);
            if (productos != null)
            {
                foreach (var producto in productos)
                {
                    var registrosInventario = producto.ProductosInventario.ToList();
                    if (registrosInventario != null)
                    {
                        foreach (var regInventario in registrosInventario)
                        {
                            if (regInventario.FechaVencimientoArticuloCompra != null
                                && regInventario.FechaVencimientoArticuloCompra > DateTime.Today
                                && ((TimeSpan)(regInventario.FechaVencimientoArticuloCompra
                                - DateTime.Today)).TotalDays <= 120)
                            {
                                var bodega = regInventario.Bodega ?? new Bodega();
                                var fechaVencimiento = regInventario.FechaVencimientoArticuloCompra != null ?
                                    ((DateTime)regInventario.FechaVencimientoArticuloCompra).ToString("dd-MM-yyyy")
                                    : "-";
                                listaProximosVencer.Add(new HomeProductoProximoVencerViewModel
                                {
                                    ProductoNombre = producto.NombreProducto,
                                    ProductoCodigo = producto.CodigoReferencia,
                                    ProductoStock = regInventario.Stock,
                                    ProductoUbicacion = bodega.BodegaSucursalText,
                                    FechaVencimiento = fechaVencimiento
                                });
                            }
                        }
                    }
                }
            }
            return listaProximosVencer;
        }
        public List<HomeProductoVencidoViewModel> GetProductosVencidos(int tipoBodegaId)
        {
            var listaVencidos = new List<HomeProductoVencidoViewModel>();
            var productos = _productoRepository.GetProductos(tipoBodegaId, null, false);
            if (productos != null)
            {
                foreach (var producto in productos)
                {
                    var registrosInventario = producto.ProductosInventario.ToList();
                    if (registrosInventario != null)
                    {
                        foreach (var regInventario in registrosInventario)
                        {
                            if (regInventario.FechaVencimientoArticuloCompra != null
                                && regInventario.FechaVencimientoArticuloCompra <= DateTime.Today)
                            {
                                var bodega = regInventario.Bodega ?? new Bodega();
                                var fechaVencimiento = regInventario.FechaVencimientoArticuloCompra != null ?
                                    ((DateTime)regInventario.FechaVencimientoArticuloCompra).ToString("dd-MM-yyyy")
                                    : "-";
                                listaVencidos.Add(new HomeProductoVencidoViewModel
                                {
                                    ProductoNombre = producto.NombreProducto,
                                    ProductoCodigo = producto.CodigoReferencia,
                                    ProductoStock = regInventario.Stock,
                                    ProductoUbicacion = bodega.BodegaSucursalText,
                                    FechaVencimiento = fechaVencimiento
                                });
                            }
                        }
                    }
                }
            }
            return listaVencidos;
        }
        public List<HomeExamenSolicitadoViewModel> GetExamenesSolicitados()
        {
            var listaExamenesSolicitados = new List<HomeExamenSolicitadoViewModel>();
            var examenesSolicitados = _labClinicoRepository.PaginacionExamenesRealizados(
                null, // sortOrder
                null, // searchString
                null, // pageNumber
                int.MaxValue, // pageSize (traer todos)
                (int)EstadoExamenEnum.Solictiado, // estado solicitado
                true // solicitado
            );
            if (examenesSolicitados != null)
            {
                foreach (var examen in examenesSolicitados)
                {
                    var medico = examen.Medicos ?? new Medicos();
                    var paciente = examen.Paciente ?? new Paciente();
                    var clinica = examen.Clinicas ?? new Clinica();
                    listaExamenesSolicitados.Add(new HomeExamenSolicitadoViewModel
                    {
                        ExamenId = examen.Id,
                        FechaSolicitud = examen.FechaRealizacion.ToString(),
                        ClinicaNombre = clinica.NombreClinica,
                        MedicoNombre = medico.Nombres,
                        PacienteNombre = paciente.Nombre
                    });
                }
            }
            return listaExamenesSolicitados;
        }
        public List<HomeExamenFinalizadoViewModel> GetExamenesFinalizados()
        {
            var listaExamenesFinalizados = new List<HomeExamenFinalizadoViewModel>();
            var examenesFinalizados = _labClinicoRepository.GetListExamenesRealizado();
            if (examenesFinalizados != null)
            {
                foreach (var examen in examenesFinalizados)
                {
                    var medico = examen.Medicos ?? new Medicos();
                    var paciente = examen.Paciente ?? new Paciente();
                    var clinica = examen.Clinicas ?? new Clinica();
                    listaExamenesFinalizados.Add(new HomeExamenFinalizadoViewModel
                    {
                        ExamenId = examen.Id,
                        FechaRealizacion = examen.FechaRealizacion.ToString(),
                        ClinicaNombre = clinica.NombreClinica,
                        MedicoNombre = medico.Nombres,
                        PacienteNombre = paciente.Nombre
                    });
                }
            }
            return listaExamenesFinalizados;
        }
        public List<HomeCuentaPagarViewModel> GetCuentasPorPagar()
        {
            var listaCuentasPagar = new List<HomeCuentaPagarViewModel>();
          var resultadoBd = _compraRepository.GetListaTodas().Where(a => a.TipoCompraId == (int)TipoCompraEnum.Credito).ToList();

            if (resultadoBd != null)
            {
                foreach (var compra in resultadoBd)
                {
                    var nombreAmbiente = "";
                    switch (compra.TipoBodegaId)
                    {
                        case (int)TipoBodegaEnum.Bodega:
                            nombreAmbiente = "Bodega";
                            break;
                        case (int)TipoBodegaEnum.Clinica:
                            nombreAmbiente = "Clinica";
                            break;
                        case (int)TipoBodegaEnum.Hospital:
                            nombreAmbiente = "Hospital";
                            break;
                        case (int)TipoBodegaEnum.Farmacia:
                            nombreAmbiente = "Farmacia";
                            break;
                        case (int)TipoBodegaEnum.Laboratorio:
                            nombreAmbiente = "Laboratorio";
                            break;
                        default:
                            nombreAmbiente = "-";
                            break;
                    }
                    var proveedor = compra.Proveedor ?? new Proveedor();
                    listaCuentasPagar.Add(new HomeCuentaPagarViewModel
                    {
                        CompraId = compra.Id,
                        AmbienteNombre = nombreAmbiente,
                        FechaCompra = compra.FechaCompra.ToString(),
                        Proveedor = proveedor.Nombre,
                        Valor = compra.ValorTotal
                    });
                }
            }
            return listaCuentasPagar;
        }
    }
}

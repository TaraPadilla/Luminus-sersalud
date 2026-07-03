using Database.Shared.Dto;
using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using sistema.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace farmamest.Service
{
    public class ServiciosService : IServiciosService
    {
        private readonly IProducto _productoRepository;
        private readonly IServicio _servicioRepository;
        private readonly ICompra _compraRepository;
        private readonly IProductoEquivalencia _productoEquivalencia;

        public ServiciosService(IProductoEquivalencia productoEquivalencia, IProducto productoRepository,
            ICompra compraRepository, IServicio servicioRepository)
        {
            _productoRepository = productoRepository;
            _compraRepository = compraRepository;
            _productoEquivalencia = productoEquivalencia;
            _servicioRepository = servicioRepository;
        }

        public List<DtoSpGetServicios> GetServicios()
        {
            var data = _servicioRepository.GetServiciosSp();
            return data;
        }
    }
}

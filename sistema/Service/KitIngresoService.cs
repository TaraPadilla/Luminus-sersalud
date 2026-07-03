using Database.Shared.IRepository;
using Database.Shared.Models;
using farmamest.Models;
using farmamest.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace farmamest.Service
{
    public class KitIngresoService : IKitIngresoService
    {
        private readonly IKitIngreso _repository;

        public KitIngresoService(IKitIngreso repository)
        {
            _repository = repository;
        }

        public void Add(KitIngreso kitIngreso)
        {
            _repository.Add(kitIngreso);
        }

        public KitIngreso GetById(int id)
        {
            return _repository.GetById(id);
        }

        public IEnumerable<KitIngreso> GetGlobalKits()
        {
            return _repository.GetGlobalKits();
        }

        public IEnumerable<KitIngreso> GetByHospitalizacionId(int hospitalizacionId)
        {
            return _repository.GetByHospitalizacionId(hospitalizacionId);
        }

        public void UpdateKit(KitIngreso kit)
        {
            _repository.UpdateKit(kit);
        }

        public void AgregarProducto(KitIngresoDetalleInputVM model)
        {
            var detalle = new KitIngresoDetalle
            {
                KitIngresoId = model.KitIngresoId,
                ProductoId = model.ProductoId,
                ProductoCodigo = model.ProductoCodigo,
                ProductoNombre = model.ProductoNombre,
                UnidadMedidaVentaId = model.UnidadMedidaVentaId,
                UnidadMedidaVentaNombre = model.UnidadMedidaVentaNombre,
                PrecioId = model.PrecioId,
                PrecioNombre = model.PrecioNombre,
                Cantidad = model.Cantidad,
                ValorUnitario = model.ValorUnitario,
                ValorSubtotal = model.ValorSubtotal,
                DescuentoPorcentaje = model.DescuentoPorcentaje,
                DescuentoValor = model.DescuentoValor,
                ValorTotal = model.ValorTotal,
                Eliminado = false,
            };
            _repository.AddDetalle(detalle);
        }

        public void EliminarDetalle(int detalleId)
        {
            _repository.EliminarDetalle(detalleId);
        }

        public KitIngresoDetalle GetDetalleById(int detalleId)
        {
            return _repository.GetDetalleById(detalleId);
        }

        public void ActualizarDetalle(KitIngresoDetalleInputVM model)
        {
            var detalle = _repository.GetDetalleById(model.Id);
            if (detalle == null)
                throw new Exception("Detalle no encontrado");

            detalle.Cantidad = model.Cantidad;
            detalle.ValorUnitario = model.ValorUnitario;
            detalle.ValorSubtotal = model.ValorSubtotal;
            detalle.DescuentoPorcentaje = model.DescuentoPorcentaje;
            detalle.DescuentoValor = model.DescuentoValor;
            detalle.ValorTotal = model.ValorTotal;

            if (!string.IsNullOrEmpty(model.UnidadMedidaVentaNombre))
                detalle.UnidadMedidaVentaNombre = model.UnidadMedidaVentaNombre;
            if (!string.IsNullOrEmpty(model.PrecioNombre))
                detalle.PrecioNombre = model.PrecioNombre;

            _repository.UpdateDetalle(detalle);
        }

        public KitIngreso ClonarKit(int kitOrigenId, int nuevaHospitalizacionId, string userId)
        {
            var kitOriginal = _repository.GetById(kitOrigenId);
            if (kitOriginal == null)
                throw new Exception("Kit origen no encontrado");

            var nuevoKit = new KitIngreso
            {
                HospitalizacionId = nuevaHospitalizacionId,
                FechaRegistro = DateTime.Now,
                UserId = userId,
                NombrePaciente = kitOriginal.NombrePaciente,
                Medico = kitOriginal.Medico,
                Procedimiento = kitOriginal.Procedimiento,
                Responsable = kitOriginal.Responsable,
                FechaKit = DateTime.Now,
                NombreKit = kitOriginal.NombreKit + " (copia)"
            };
            _repository.Add(nuevoKit);

            foreach (var detOriginal in kitOriginal.Detalles.Where(d => !d.Eliminado))
            {
                var nuevoDetalle = new KitIngresoDetalle
                {
                    KitIngresoId = nuevoKit.Id,
                    ProductoId = detOriginal.ProductoId,
                    ProductoCodigo = detOriginal.ProductoCodigo,
                    ProductoNombre = detOriginal.ProductoNombre,
                    UnidadMedidaVentaId = detOriginal.UnidadMedidaVentaId,
                    UnidadMedidaVentaNombre = detOriginal.UnidadMedidaVentaNombre,
                    PrecioId = detOriginal.PrecioId,
                    PrecioNombre = detOriginal.PrecioNombre,
                    Cantidad = detOriginal.Cantidad,
                    ValorUnitario = detOriginal.ValorUnitario,
                    ValorSubtotal = detOriginal.ValorSubtotal,
                    DescuentoPorcentaje = detOriginal.DescuentoPorcentaje,
                    DescuentoValor = detOriginal.DescuentoValor,
                    ValorTotal = detOriginal.ValorTotal,
                    Eliminado = false
                };
                _repository.AddDetalle(nuevoDetalle);
            }
            return nuevoKit;
        }
        public async Task<decimal> ObtenerUtilizadoAsync(int detalleId, int hospitalizacionId)
        {
            return await _repository.ObtenerUtilizadoPorDetalleYHospitalizacionAsync(detalleId, hospitalizacionId);
        }

        public async Task GuardarUtilizadoAsync(int detalleId, int hospitalizacionId, decimal utilizado)
        {
            await _repository.GuardarConsumoAsync(detalleId, hospitalizacionId, utilizado);
        }

        public async Task<IEnumerable<HospitalizacionKitConsumo>> ObtenerConsumosPorHospitalizacionAsync(int hospitalizacionId)
        {
            return await _repository.ObtenerConsumosPorHospitalizacionAsync(hospitalizacionId);
        }

        public IEnumerable<KitIngreso> GetGlobalesYPorHospitalizacion(int hospitalizacionId)
        {
            return _repository.GetGlobalesYPorHospitalizacion(hospitalizacionId);
        }


        public KitIngreso ClonarKitConDatos(
    int kitOrigenId,
    int nuevaHospitalizacionId,
    string userId,
    string nombrePaciente,
    string medico,
    string procedimiento,
    string responsable)
        {
            var kitOriginal = _repository.GetById(kitOrigenId);
            if (kitOriginal == null)
                throw new Exception("Kit origen no encontrado");

            var nuevoKit = new KitIngreso
            {
                HospitalizacionId = nuevaHospitalizacionId,
                FechaRegistro = DateTime.Now,
                UserId = userId,
                NombrePaciente = nombrePaciente,   
                Medico = medico,                 
                Procedimiento = procedimiento,   
                Responsable = responsable,       
                FechaKit = DateTime.Now,
                NombreKit = kitOriginal.NombreKit + " (copia)"  
            };
            _repository.Add(nuevoKit);

            foreach (var detOriginal in kitOriginal.Detalles.Where(d => !d.Eliminado))
            {
                var nuevoDetalle = new KitIngresoDetalle
                {
                    KitIngresoId = nuevoKit.Id,
                    ProductoId = detOriginal.ProductoId,
                    ProductoCodigo = detOriginal.ProductoCodigo,
                    ProductoNombre = detOriginal.ProductoNombre,
                    UnidadMedidaVentaId = detOriginal.UnidadMedidaVentaId,
                    UnidadMedidaVentaNombre = detOriginal.UnidadMedidaVentaNombre,
                    PrecioId = detOriginal.PrecioId,
                    PrecioNombre = detOriginal.PrecioNombre,
                    Cantidad = detOriginal.Cantidad,
                    ValorUnitario = detOriginal.ValorUnitario,
                    ValorSubtotal = detOriginal.ValorSubtotal,
                    DescuentoPorcentaje = detOriginal.DescuentoPorcentaje,
                    DescuentoValor = detOriginal.DescuentoValor,
                    ValorTotal = detOriginal.ValorTotal,
                    Eliminado = false
                };
                _repository.AddDetalle(nuevoDetalle);
            }
            return nuevoKit;
        }
    }
}
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class InventarioProductoViewModel
    {
        public int? AmbienteId { get; set; }
        public int? TipoBodegaId { get; set; }
        public string TipoBodegaNombre { get; set; }
        public int? TipoProductoId { get; set; }
        public SelectList TipoProductoSelectListItems { get; set; }
        public string TipoProductoNombre { get; set; }
        public int? Id { get; set; }
        public string CodigoReferencia { get; set; }
        public string Nombre { get; set; }
        public List<ProductoEquivalenciaViewModel> Equivalencias { get; set; }
        public int? CategoriaId { get; set; }
        public SelectList CategoriasSelectListItems { get; set; }
        public int? MarcaId { get; set; }
        public SelectList MarcaSelectListItems { get; set; }
        public int? GrupoId { get; set; }
        public SelectList GruposSelectListItems { get; set; }
        public int? PresentacionId { get; set; }
        public int? PresentacionId2 { get; set; }
        public int? PresentacionId3 { get; set; }
        public int? PresentacionId4 { get; set; }
        public int? PresentacionId5 { get; set; }

        public SelectList PresentacionSelectListItems { get; set; }
        public int? ViadminId { get; set; }
        public int? LaboratorioId { get; set; }
        public string Ubicacion { get; set; }
        public string ActivoConcentracion { get; set; }
        public string UrlImagen { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        public decimal Stock { get; set; }

        public void Init(IDespegablesProducto categoriaRepository)
        {
            CategoriasSelectListItems = new SelectList(categoriaRepository.ListaCategorias(), "Id", "NombreCategoria");
            MarcaSelectListItems = new SelectList(categoriaRepository.ListaMarcas(), "Id", "NombreMarca");
            GruposSelectListItems = new SelectList(categoriaRepository.ListaGrupos(), "Id", "NombreGrupo");
            PresentacionSelectListItems = new SelectList(categoriaRepository.ListarPresentacion(), "Id", "PresentProducto");
            TipoProductoSelectListItems = new SelectList(categoriaRepository.ListarTipoProductos(), "Id", "NombreTipoProducto");
        }
    }
}

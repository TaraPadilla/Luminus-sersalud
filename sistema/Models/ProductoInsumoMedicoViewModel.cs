using Database.Shared.IRepository;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace sistema.Models
{
    public class ProductoInsumoMedicoViewModel
    {
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
        public SelectList PresentacionSelectListItems { get; set; }
        public string UrlImagen { get; set; }
        public string Descripcion { get; set; }

        public void Init(IDespegablesProducto categoriaRepository)
        {
            CategoriasSelectListItems = new SelectList(categoriaRepository.ListaCategorias(), "Id", "NombreCategoria");
            MarcaSelectListItems = new SelectList(categoriaRepository.ListaMarcas(), "Id", "NombreMarca");
            GruposSelectListItems = new SelectList(categoriaRepository.ListaGrupos(), "Id", "NombreGrupo");
            PresentacionSelectListItems = new SelectList(categoriaRepository.ListarPresentacion(), "Id", "PresentProducto");
        }
    }
    
}

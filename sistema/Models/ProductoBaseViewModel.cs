using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;

namespace sistema.Models
{
    public class ProductoBaseViewModel
    {

        public ProductoBaseViewModel()
        {
            Producto.FechaVencimiento = DateTime.Now;
        }

        public Producto Producto {get;set;} = new Producto();

        public List<Viadmin> ListaCategorias = new List<Viadmin>();

        public string imagen {get; set;}
        
        public SelectList ListCategorias {get;set;}
        public SelectList ListaTipoProductos {get;set;}
        public SelectList ListaPresentacionProductos {get;set;}
        public SelectList ListaGrupoTProducto {get;set;}
        public SelectList ListaLaboratorioProductos {get;set;}


        public bool Modificar {get;set;}

        public void Init(IDespegablesProducto categoriaRepository)
        {
            ListCategorias = new SelectList(categoriaRepository.ListarCategorias(), "Id", "NombreViadmin");
            ListaTipoProductos = new SelectList(categoriaRepository.ListarTipoProductos(), "Id", "NombreTipoProducto");
            ListaPresentacionProductos = new SelectList(categoriaRepository.ListarPresentacion(), "Id", "PresentProducto");
            ListaGrupoTProducto = new SelectList(categoriaRepository.ListarGrupoT(), "Id", "NombreGrupoT");
            ListaLaboratorioProductos = new SelectList(categoriaRepository.ListaLaboratorioProducto(), "Id", "NombreLaboratorioProducto");
        }
        
        public int Id
        {
            get { return Producto.Id; }
            set { Producto.Id = value; }
        }
    }
} 
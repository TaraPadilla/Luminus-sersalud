using Database.Shared.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Database.Shared.IRepository;
using Microsoft.AspNetCore.Http;
using System;

namespace sistema.Models
{
    public class ProductoInsumosBaseViewModel
    {

        public ProductoInsumosBaseViewModel()
        {
            Producto.FechaVencimiento = DateTime.Now;
        }

        public Producto Producto {get;set;} = new Producto();
        public SelectList CategoriasSelectListItems {get;set;}
        public SelectList MarcaSelectListItems {get;set;}
        public SelectList GruposSelectListItems {get;set;}
        public SelectList PresentacionSelectListItems {get;set;}
      
        public bool Modificar {get;set;}

        public void Init(IDespegablesProducto categoriaRepository)
        {
            CategoriasSelectListItems = new SelectList(categoriaRepository.ListaCategorias(), "Id", "NombreCategoria");
            MarcaSelectListItems = new SelectList(categoriaRepository.ListaMarcas(), "Id", "NombreMarca");
            GruposSelectListItems = new SelectList(categoriaRepository.ListaGrupos(), "Id", "NombreGrupo");
            PresentacionSelectListItems = new SelectList(categoriaRepository.ListarPresentacion(), "Id", "PresentProducto");
        }
        
        public int Id
        {
            get { return Producto.Id; }
            set { Producto.Id = value; }
        }
    }
} 
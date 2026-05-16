using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Interfaces;
using STOCKPAP.DataAccess;

namespace STOCKPAP.Presenters
{
    public class ProductoPresenter
    {
        private IProductoView view;
        private BindingSource bindingSource;
        private ProductoDAO dao;

        public ProductoPresenter(IProductoView view)
        {
            this.view = view;
            this.bindingSource = new BindingSource();
            this.dao = new ProductoDAO();

            // Asociar eventos
            this.view.SearchEvent += SearchProducto;
            this.view.AddNewEvent += AddNewProducto;
            this.view.EditEvent += LoadSelectedProductoToEdit;
            this.view.DeleteEvent += DeleteSelectedProducto;
            this.view.SaveEvent += SaveProducto;
            this.view.CancelEvent += CancelAction;

            // Set binding source
            this.view.SetProductoListBindingSource(bindingSource);
            LoadAllProductoList();
        }

        private void LoadAllProductoList()
        {
            bindingSource.DataSource = dao.GetAll();
        }

        private void SearchProducto(object sender, EventArgs e)
        {
            bool emptyValue = string.IsNullOrWhiteSpace(this.view.SearchValue);
            var list = dao.GetAll();
            if (!emptyValue)
                bindingSource.DataSource = list.Where(p => p.Nombre.ToLower().Contains(this.view.SearchValue.ToLower())).ToList();
            else 
                bindingSource.DataSource = list;
        }

        private void AddNewProducto(object sender, EventArgs e)
        {
            view.IsEdit = false;
            CleanviewFields();
        }

        private void LoadSelectedProductoToEdit(object sender, EventArgs e)
        {
            var producto = (Producto)bindingSource.Current;
            if (producto != null)
            {
                view.ProdId = producto.Id.ToString();
                view.Nombre = producto.Nombre;
                view.Categoria = producto.Categoria;
                view.PrecioCompra = producto.PrecioCompra.ToString();
                view.PrecioVenta = producto.PrecioVenta.ToString();
                view.Stock = producto.Stock.ToString();
                view.IsEdit = true;
            }
        }

        private void SaveProducto(object sender, EventArgs e)
        {
            try
            {
                var model = new Producto();
                model.Id = string.IsNullOrEmpty(view.ProdId) ? 0 : Convert.ToInt32(view.ProdId);
                model.Nombre = view.Nombre;
                model.Categoria = view.Categoria;
                model.PrecioCompra = Convert.ToDecimal(view.PrecioCompra);
                model.PrecioVenta = Convert.ToDecimal(view.PrecioVenta);
                model.Stock = Convert.ToInt32(view.Stock);

                if (view.IsEdit)
                {
                    dao.Update(model);
                    view.Message = "Producto editado correctamente";
                }
                else
                {
                    dao.Add(model);
                    view.Message = "Producto agregado correctamente";
                }
                view.IsSuccessful = true;
                LoadAllProductoList();
                CleanviewFields();
            }
            catch (Exception ex)
            {
                view.IsSuccessful = false;
                view.Message = ex.Message;
            }
        }

        private void DeleteSelectedProducto(object sender, EventArgs e)
        {
            try
            {
                var producto = (Producto)bindingSource.Current;
                if (producto != null)
                {
                    dao.Delete(producto.Id);
                    view.IsSuccessful = true;
                    view.Message = "Producto eliminado correctamente";
                    LoadAllProductoList();
                }
            }
            catch (Exception ex)
            {
                view.IsSuccessful = false;
                view.Message = "Ocurrió un error: " + ex.Message;
            }
        }

        private void CancelAction(object sender, EventArgs e)
        {
            CleanviewFields();
        }

        private void CleanviewFields()
        {
            view.ProdId = "0";
            view.Nombre = "";
            view.Categoria = "";
            view.PrecioCompra = "0";
            view.PrecioVenta = "0";
            view.Stock = "0";
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Interfaces;

namespace STOCKPAP.Presenters
{
    public class ProductoPresenter
    {
        private IProductoView view;
        private List<Producto> productoList;
        private BindingSource productoBindingSource;

        public ProductoPresenter(IProductoView view)
        {
            this.view = view;
            this.productoBindingSource = new BindingSource();
            this.productoList = new List<Producto>
            {
                new Producto { Id = 1, Nombre = "Cuaderno Profesional", Categoria = "Escolar", PrecioCompra = 15.00m, PrecioVenta = 25.00m, Stock = 150 },
                new Producto { Id = 2, Nombre = "Lápiz Mirado No. 2", Categoria = "Escolar", PrecioCompra = 2.00m, PrecioVenta = 5.00m, Stock = 500 }
            };

            // Associate events
            this.view.SearchEvent += SearchProducto;
            this.view.AddNewEvent += AddNewProducto;
            this.view.EditEvent += LoadSelectedProductoToEdit;
            this.view.DeleteEvent += DeleteSelectedProducto;
            this.view.SaveEvent += SaveProducto;
            this.view.CancelEvent += CancelAction;

            // Set binding source
            this.view.SetProductoListBindingSource(productoBindingSource);
            LoadAllProductoList();
        }

        private void LoadAllProductoList()
        {
            productoBindingSource.DataSource = productoList;
        }

        private void SearchProducto(object sender, EventArgs e)
        {
            bool emptyValue = string.IsNullOrWhiteSpace(this.view.SearchValue);
            if (emptyValue == false)
                productoBindingSource.DataSource = productoList.Where(p => p.Nombre.ToLower().Contains(this.view.SearchValue.ToLower())).ToList();
            else productoBindingSource.DataSource = productoList;
        }

        private void AddNewProducto(object sender, EventArgs e)
        {
            view.IsEdit = false;
            CleanviewFields();
        }

        private void LoadSelectedProductoToEdit(object sender, EventArgs e)
        {
            var producto = (Producto)productoBindingSource.Current;
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
                    var existing = productoList.FirstOrDefault(p => p.Id == model.Id);
                    if (existing != null)
                    {
                        existing.Nombre = model.Nombre;
                        existing.Categoria = model.Categoria;
                        existing.PrecioCompra = model.PrecioCompra;
                        existing.PrecioVenta = model.PrecioVenta;
                        existing.Stock = model.Stock;
                    }
                    view.Message = "Producto editado correctamente";
                }
                else
                {
                    model.Id = productoList.Count > 0 ? productoList.Max(p => p.Id) + 1 : 1;
                    productoList.Add(model);
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
                var producto = (Producto)productoBindingSource.Current;
                if (producto != null)
                {
                    productoList.Remove(producto);
                    view.IsSuccessful = true;
                    view.Message = "Producto eliminado correctamente";
                    LoadAllProductoList();
                }
            }
            catch (Exception)
            {
                view.IsSuccessful = false;
                view.Message = "An error ocurred, could not delete producto";
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

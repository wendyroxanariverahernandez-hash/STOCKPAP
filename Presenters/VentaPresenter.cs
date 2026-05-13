using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Interfaces;

namespace STOCKPAP.Presenters
{
    public class VentaPresenter
    {
        private IVentaView view;
        private List<Venta> ventaList;
        private BindingSource ventaBindingSource;

        public VentaPresenter(IVentaView view)
        {
            this.view = view;
            this.ventaBindingSource = new BindingSource();
            this.ventaList = new List<Venta>
            {
                new Venta { Id = 1001, Fecha = DateTime.Now, Cliente = "Ana López", Total = 450.00m, MetodoPago = "Efectivo" }
            };

            this.view.AddNewEvent += (s, e) => { /* Reset fields if needed */ };
            this.view.SaveEvent += SaveVenta;
            
            this.view.SetVentaListBindingSource(ventaBindingSource);
            LoadAllVentaList();
        }

        private void LoadAllVentaList()
        {
            ventaBindingSource.DataSource = ventaList;
        }

        private void SaveVenta(object sender, EventArgs e)
        {
            try
            {
                var model = new Venta();
                model.Id = ventaList.Count > 0 ? ventaList.Max(v => v.Id) + 1 : 1001;
                model.Fecha = DateTime.Now;
                model.Cliente = view.Cliente;
                model.MetodoPago = view.MetodoPago;
                
                // For simplicity, we'll just use a mock price calculation
                decimal pricePerUnit = 25.00m; 
                model.Total = Convert.ToInt32(view.Cantidad) * pricePerUnit;

                ventaList.Add(model);
                view.IsSuccessful = true;
                view.Message = "Venta registrada con éxito";
                LoadAllVentaList();
            }
            catch (Exception ex)
            {
                view.IsSuccessful = false;
                view.Message = ex.Message;
            }
        }
    }
}

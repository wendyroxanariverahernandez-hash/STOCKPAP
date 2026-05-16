using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Interfaces;
using STOCKPAP.DataAccess;

namespace STOCKPAP.Presenters
{
    public class VentaPresenter
    {
        private IVentaView view;
        private BindingSource bindingSource;
        private VentaDAO dao;

        public VentaPresenter(IVentaView view)
        {
            this.view = view;
            this.bindingSource = new BindingSource();
            this.dao = new VentaDAO();

            this.view.AddNewEvent += (s, e) => { CleanViewFields(); };
            this.view.SaveEvent += SaveVenta;
            
            this.view.SetVentaListBindingSource(bindingSource);
            LoadAllVentaList();
        }

        private void LoadAllVentaList()
        {
            bindingSource.DataSource = dao.GetAll();
        }

        private void SaveVenta(object sender, EventArgs e)
        {
            try
            {
                var model = new Venta();
                model.Fecha = DateTime.Now;
                // Cliente is now hardcoded in the DAO or ignored, but let's just not set it from view
                model.MetodoPago = view.MetodoPago;
                
                if (decimal.TryParse(view.CantidadRecibida, out decimal cr)) model.CantidadRecibida = cr;
                if (decimal.TryParse(view.Cambio, out decimal cb)) model.Cambio = cb;
                model.TipoTarjeta = view.TipoTarjeta;
                model.Banco = view.Banco;
                model.Ultimos4 = view.Ultimos4;
                model.Referencia = view.Referencia;
                model.Confirmacion = view.Confirmacion;
                
                // Simulación cálculo de precio temporal si no viene el Total real
                decimal pricePerUnit = 25.00m; 
                if (int.TryParse(view.Cantidad, out int cantidad))
                {
                    model.Total = cantidad * pricePerUnit;
                }
                else if (decimal.TryParse(view.Total, out decimal total))
                {
                    model.Total = total;
                }

                dao.Add(model);

                view.IsSuccessful = true;
                view.Message = "Venta registrada con éxito";
                LoadAllVentaList();
                CleanViewFields();
            }
            catch (Exception ex)
            {
                view.IsSuccessful = false;
                view.Message = ex.Message;
            }
        }
        
        private void CleanViewFields()
        {
            view.VentaId = "0";
            view.Producto = "";
            view.Cantidad = "0";
            view.Total = "0";
            view.MetodoPago = "";
            view.CantidadRecibida = "";
            view.Cambio = "";
            view.TipoTarjeta = "";
            view.Banco = "";
            view.Ultimos4 = "";
            view.Referencia = "";
            view.Confirmacion = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Interfaces;
using STOCKPAP.DataAccess;

namespace STOCKPAP.Presenters
{
    public class ClientePresenter
    {
        private IClienteView view;
        private BindingSource bindingSource;
        private ClienteDAO dao;

        public ClientePresenter(IClienteView view)
        {
            this.view = view;
            this.bindingSource = new BindingSource();
            this.dao = new ClienteDAO();

            // Asociar eventos
            this.view.SearchEvent += Search;
            this.view.AddNewEvent += AddNew;
            this.view.SaveEvent += Save;

            this.view.SetClienteListBindingSource(bindingSource);
            LoadAll();
        }

        private void LoadAll()
        {
            bindingSource.DataSource = dao.GetAll();
        }

        private void Search(object sender, EventArgs e)
        {
            bool emptyValue = string.IsNullOrWhiteSpace(this.view.SearchValue);
            var list = dao.GetAll();
            if (!emptyValue)
                bindingSource.DataSource = list.Where(p => p.Nombre.ToLower().Contains(this.view.SearchValue.ToLower())).ToList();
            else 
                bindingSource.DataSource = list;
        }

        private void AddNew(object sender, EventArgs e)
        {
            CleanViewFields();
        }

        private void Save(object sender, EventArgs e)
        {
            try
            {
                var model = new Cliente();
                model.Nombre = view.Nombre;
                model.Direccion = view.Direccion;
                model.Telefono = view.Telefono;

                if (string.IsNullOrEmpty(view.ClienteId) || view.ClienteId == "0")
                {
                    dao.Add(model);
                    view.Message = "Cliente agregado correctamente";
                }
                else
                {
                    model.Id = Convert.ToInt32(view.ClienteId);
                    dao.Update(model);
                    view.Message = "Cliente editado correctamente";
                }
                
                view.IsSuccessful = true;
                LoadAll();
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
            view.ClienteId = "0";
            view.Nombre = "";
            view.Direccion = "";
            view.Telefono = "";
        }
    }
}

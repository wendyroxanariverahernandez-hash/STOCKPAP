using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Interfaces;
using STOCKPAP.DataAccess;

namespace STOCKPAP.Presenters
{
    public class ProveedorPresenter
    {
        private IProveedorView view;
        private BindingSource bindingSource;
        private ProveedorDAO dao;

        public ProveedorPresenter(IProveedorView view)
        {
            this.view = view;
            this.bindingSource = new BindingSource();
            this.dao = new ProveedorDAO();

            // Asociar eventos
            this.view.SearchEvent += Search;
            this.view.AddNewEvent += AddNew;
            this.view.EditEvent += LoadSelectedToEdit;
            this.view.DeleteEvent += DeleteSelected;
            this.view.SaveEvent += Save;
            this.view.CancelEvent += CancelAction;

            this.view.SetProveedorListBindingSource(bindingSource);
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
                bindingSource.DataSource = list.Where(p => p.Empresa.ToLower().Contains(this.view.SearchValue.ToLower())).ToList();
            else 
                bindingSource.DataSource = list;
        }

        private void AddNew(object sender, EventArgs e)
        {
            view.IsEdit = false;
            CleanViewFields();
        }

        private void LoadSelectedToEdit(object sender, EventArgs e)
        {
            var model = (Proveedor)bindingSource.Current;
            if (model != null)
            {
                view.ProvId = model.Id.ToString();
                view.Empresa = model.Empresa;
                view.Contacto = model.Contacto;
                view.Telefono = model.Telefono;
                view.Email = model.Email;
                view.Estado = model.Estado;
                view.IsEdit = true;
            }
        }

        private void Save(object sender, EventArgs e)
        {
            try
            {
                var model = new Proveedor();
                model.Id = string.IsNullOrEmpty(view.ProvId) ? 0 : Convert.ToInt32(view.ProvId);
                model.Empresa = view.Empresa;
                model.Contacto = view.Contacto;
                model.Telefono = view.Telefono;
                model.Email = view.Email;
                model.Estado = view.Estado;

                if (view.IsEdit)
                {
                    dao.Update(model);
                    view.Message = "Proveedor editado correctamente";
                }
                else
                {
                    dao.Add(model);
                    view.Message = "Proveedor agregado correctamente";
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

        private void DeleteSelected(object sender, EventArgs e)
        {
            try
            {
                var model = (Proveedor)bindingSource.Current;
                if (model != null)
                {
                    dao.Delete(model.Id);
                    view.IsSuccessful = true;
                    view.Message = "Proveedor eliminado correctamente";
                    LoadAll();
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
            CleanViewFields();
        }

        private void CleanViewFields()
        {
            view.ProvId = "0";
            view.Empresa = "";
            view.Contacto = "";
            view.Telefono = "";
            view.Email = "";
            view.Estado = "";
        }
    }
}

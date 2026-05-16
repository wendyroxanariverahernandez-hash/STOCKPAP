using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using STOCKPAP.Models;
using STOCKPAP.Interfaces;
using STOCKPAP.DataAccess;

namespace STOCKPAP.Presenters
{
    public class UsuarioPresenter
    {
        private IUsuarioView view;
        private BindingSource bindingSource;
        private UsuarioDAO dao;

        public UsuarioPresenter(IUsuarioView view)
        {
            this.view = view;
            this.bindingSource = new BindingSource();
            this.dao = new UsuarioDAO();

            this.view.SearchEvent += SearchUsuario;
            this.view.AddNewEvent += AddNewUsuario;
            this.view.EditEvent += LoadSelectedUsuarioToEdit;
            this.view.DeleteEvent += DeleteSelectedUsuario;
            this.view.SaveEvent += SaveUsuario;
            this.view.CancelEvent += CancelAction;

            this.view.SetUsuarioListBindingSource(bindingSource);
            LoadAllUsuarioList();
        }

        private void LoadAllUsuarioList()
        {
            bindingSource.DataSource = dao.GetAll();
        }

        private void SearchUsuario(object sender, EventArgs e)
        {
            bool emptyValue = string.IsNullOrWhiteSpace(this.view.SearchValue);
            var list = dao.GetAll();
            if (!emptyValue)
                bindingSource.DataSource = list.Where(u => u.Username.ToLower().Contains(this.view.SearchValue.ToLower())).ToList();
            else 
                bindingSource.DataSource = list;
        }

        private void AddNewUsuario(object sender, EventArgs e)
        {
            view.IsEdit = false;
            CleanviewFields();
        }

        private void LoadSelectedUsuarioToEdit(object sender, EventArgs e)
        {
            var usuario = (Usuario)bindingSource.Current;
            if (usuario != null)
            {
                view.UsuarioId = usuario.Id.ToString();
                view.Username = usuario.Username;
                view.Password = usuario.Password;
                view.Rol = usuario.Rol;
                view.IsEdit = true;
            }
        }

        private void SaveUsuario(object sender, EventArgs e)
        {
            try
            {
                var model = new Usuario();
                model.Id = string.IsNullOrEmpty(view.UsuarioId) ? 0 : Convert.ToInt32(view.UsuarioId);
                model.Username = view.Username;
                model.Password = view.Password;
                model.Rol = view.Rol;

                if (view.IsEdit)
                {
                    dao.Update(model);
                    view.Message = "Usuario editado correctamente";
                }
                else
                {
                    dao.Add(model);
                    view.Message = "Usuario agregado correctamente";
                }
                view.IsSuccessful = true;
                LoadAllUsuarioList();
                CleanviewFields();
            }
            catch (Exception ex)
            {
                view.IsSuccessful = false;
                view.Message = ex.Message;
            }
        }

        private void DeleteSelectedUsuario(object sender, EventArgs e)
        {
            try
            {
                var usuario = (Usuario)bindingSource.Current;
                if (usuario != null)
                {
                    dao.Delete(usuario.Id);
                    view.IsSuccessful = true;
                    view.Message = "Usuario eliminado correctamente";
                    LoadAllUsuarioList();
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
            view.UsuarioId = "0";
            view.Username = "";
            view.Password = "";
            view.Rol = "";
        }
    }
}

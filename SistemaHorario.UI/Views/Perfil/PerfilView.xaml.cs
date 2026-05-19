using Microsoft.Win32;
using SistemaHorario.UI.Dialogs.Perfil;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.ViewModels.Perfil;
using SistemaHorario.UI.Views.Auth;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SistemaHorario.UI.Views.Perfil
{
    public partial class PerfilView : UserControl
    {
        private readonly PerfilViewModel _viewModel;

        public PerfilView()
        {
            InitializeComponent();
            _viewModel = new PerfilViewModel();
            Loaded += PerfilView_Loaded;
        }

        private async void PerfilView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.CargarPerfilAsync();
            CargarDatos();
        }

        private void CargarDatos()
        {
            TxtNombre.Text = _viewModel.Perfil.NombreCompleto;
            TxtCorreo.Text = _viewModel.Perfil.CorreoInstitucional;
            TxtRol.Text = _viewModel.Perfil.Rol;
            TxtTelefono.Text = _viewModel.Perfil.Telefono;
            TxtFacultad.Text = _viewModel.Perfil.FacultadPrograma;
            try
            {
                ImgPerfil.Source = new BitmapImage(new Uri(_viewModel.Perfil.RutaImagen, UriKind.RelativeOrAbsolute));
            }
            catch { }
        }

        private void BtnCambiarFoto_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new() { Filter = "Imagenes|*.png;*.jpg;*.jpeg" };
            if (dialog.ShowDialog() != true) return;
            _viewModel.ActualizarFoto(dialog.FileName);
            ImgPerfil.Source = new BitmapImage(new Uri(dialog.FileName));
        }

        private async void BtnEditarPerfil_Click(object sender, RoutedEventArgs e)
        {
            EditarPerfilDialog dialog = new(_viewModel.Perfil) { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;
            bool ok = await _viewModel.ActualizarPerfilAsync(
                dialog.PerfilResultado.NombreCompleto,
                dialog.PerfilResultado.Telefono,
                dialog.PerfilResultado.Rol,
                dialog.PerfilResultado.FacultadPrograma);
            if (!ok)
                MessageBox.Show("Error: " + _viewModel.MensajeEstado, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            CargarDatos();
        }

        private async void BtnCambiarContrasena_Click(object sender, RoutedEventArgs e)
        {
            CambiarContrasenaDialog dialog = new() { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;

            bool ok = await _viewModel.CambiarContrasenaAsync(
                dialog.Request.ContrasenaActual,
                dialog.Request.NuevaContrasena);

            if (!ok)
            {
                MessageBox.Show(
                    "No se pudo cambiar la contraseña: " + _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            MensajeExitoDialog exito = new("Contraseña actualizada correctamente.")
            {
                Owner = Window.GetWindow(this)
            };
            exito.ShowDialog();
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            CerrarSesionDialog dialog = new() { Owner = Window.GetWindow(this) };
            if (dialog.ShowDialog() != true) return;
            Window? ventanaPrincipal = Window.GetWindow(this);
            if (ventanaPrincipal == null) return;
            ventanaPrincipal.Content = new LoginView();
        }
    }
}

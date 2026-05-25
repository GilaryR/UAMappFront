using Microsoft.Win32;
using SistemaHorario.UI.Dialogs.Perfil;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Perfil;
using SistemaHorario.UI.Views.Auth;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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

        private async void PerfilView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await _viewModel.CargarPerfilAsync();

            await CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            TxtNombre.Text = _viewModel.Perfil.NombreCompleto;
            TxtCorreo.Text = _viewModel.Perfil.CorreoInstitucional;
            TxtRol.Text = _viewModel.Perfil.Rol;
            TxtTelefono.Text = _viewModel.Perfil.Telefono;
            TxtFacultad.Text = _viewModel.Perfil.FacultadPrograma;

            await CargarImagenPerfilAsync();
        }

        private async Task CargarImagenPerfilAsync()
        {
            try
            {
                string rutaImagen = _viewModel.Perfil.RutaImagen;

                if (string.IsNullOrWhiteSpace(rutaImagen))
                {
                    rutaImagen = "/Assets/Images/ImgUsuario.png";
                }

                if (rutaImagen.StartsWith(
                    "http",
                    StringComparison.OrdinalIgnoreCase))
                {
                    await CargarImagenDesdeUrlAsync(rutaImagen);
                    return;
                }

                CargarImagenLocal(rutaImagen);
            }
            catch
            {
                CargarImagenLocal("/Assets/Images/ImgUsuario.png");
            }
        }

        private async Task CargarImagenDesdeUrlAsync(
            string rutaImagen)
        {
            string rutaSinCache =
                rutaImagen.Contains("?")
                    ? rutaImagen + "&v=" + DateTime.Now.Ticks
                    : rutaImagen + "?v=" + DateTime.Now.Ticks;

            using HttpClient cliente = new();

            byte[] imagenBytes =
                await cliente.GetByteArrayAsync(rutaSinCache);

            using MemoryStream memoria = new(imagenBytes);

            BitmapImage imagen = new();

            imagen.BeginInit();
            imagen.CacheOption = BitmapCacheOption.OnLoad;
            imagen.StreamSource = memoria;
            imagen.EndInit();
            imagen.Freeze();

            ImgPerfil.Source = imagen;
        }

        private void CargarImagenLocal(
            string rutaImagen)
        {
            BitmapImage imagen = new();

            imagen.BeginInit();
            imagen.CacheOption = BitmapCacheOption.OnLoad;
            imagen.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            imagen.UriSource = new Uri(
                rutaImagen,
                UriKind.RelativeOrAbsolute);
            imagen.EndInit();
            imagen.Freeze();

            ImgPerfil.Source = imagen;
        }

        private async void BtnCambiarFoto_Click(
    object sender,
    RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Imágenes|*.png;*.jpg;*.jpeg;*.bmp"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            ConfirmacionDialog confirmacion =
                new(
                    "Modificar foto",
                    "¿Desea modificar la foto de perfil?"
                )
                {
                    Owner = Window.GetWindow(this)
                };

            if (confirmacion.ShowDialog() != true)
            {
                return;
            }

            bool guardada =
                await _viewModel.ActualizarFotoAsync(dialog.FileName);

            if (!guardada)
            {
                MessageBox.Show(
                    "No se pudo guardar la foto:\n" +
                    _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            await _viewModel.CargarPerfilAsync();

            await CargarDatosAsync();

            MensajeExitoDialog exito =
                new("Foto de perfil actualizada correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private async void BtnEditarPerfil_Click(
            object sender,
            RoutedEventArgs e)
        {
            EditarPerfilDialog dialog = new(_viewModel.Perfil)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            bool actualizado =
                await _viewModel.ActualizarPerfilAsync(
                    dialog.PerfilResultado.NombreCompleto,
                    dialog.PerfilResultado.Telefono);

            if (!actualizado)
            {
                MessageBox.Show(
                    "Error al guardar: " + _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            await _viewModel.CargarPerfilAsync();

            await CargarDatosAsync();

            MensajeExitoDialog exito =
                new("Perfil actualizado correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private async void BtnCambiarContrasena_Click(
    object sender,
    RoutedEventArgs e)
        {
            ConfirmacionDialog confirmacion =
                new(
                    "Cambiar contraseña",
                    "¿Está seguro de cambiar su contraseña?"
                )
                {
                    Owner = Window.GetWindow(this)
                };

            if (confirmacion.ShowDialog() != true)
            {
                return;
            }

            CambiarContrasenaDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            bool actualizada =
                await _viewModel.CambiarContrasenaAsync(
                    dialog.Request.ContrasenaActual,
                    dialog.Request.NuevaContrasena);

            if (!actualizada)
            {
                MessageBox.Show(
                    "No se pudo cambiar la contraseña: " +
                    _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MensajeExitoDialog exito =
                new("Contraseña actualizada correctamente. Debes iniciar sesión nuevamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();

            CerrarSesionYVolverAlLogin();
        }

        private void BtnCerrarSesion_Click(
            object sender,
            RoutedEventArgs e)
        {
            CerrarSesionDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            CerrarSesionYVolverAlLogin();
        }

        private void CerrarSesionYVolverAlLogin()
        {
            ApiClient.Token = null;

            Window? ventanaPrincipal =
                Window.GetWindow(this);

            if (ventanaPrincipal == null)
            {
                return;
            }

            ventanaPrincipal.Content = new LoginView();
        }
    }
}
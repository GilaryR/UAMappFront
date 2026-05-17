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
    /// <summary>
    /// Vista principal del perfil del usuario autenticado.
    ///
    /// Esta vista muestra la información básica del usuario y permite:
    /// - Cambiar foto de perfil.
    /// - Editar datos básicos.
    /// - Cambiar contraseña.
    /// - Cerrar sesión.
    ///
    /// Actualmente trabaja con datos temporales desde PerfilViewModel.
    ///
    /// Más adelante deberá integrarse con:
    /// - GET /api/usuarios/perfil
    /// - PUT /api/usuarios/perfil
    /// - PUT /api/usuarios/cambiar-contrasena
    /// </summary>
    public partial class PerfilView : UserControl
    {
        /// <summary>
        /// ViewModel encargado de manejar la información
        /// visual y temporal del perfil.
        /// </summary>
        private readonly PerfilViewModel _viewModel;

        /// <summary>
        /// Constructor principal de PerfilView.
        ///
        /// Inicializa los componentes visuales,
        /// crea el ViewModel y carga los datos en pantalla.
        /// </summary>
        public PerfilView()
        {
            InitializeComponent();

            _viewModel = new PerfilViewModel();

            CargarDatos();
        }

        /// <summary>
        /// Carga en pantalla la información actual del perfil.
        ///
        /// Actualmente toma los datos desde el mock del ViewModel.
        ///
        /// Más adelante, cuando se conecte la API, este método deberá
        /// refrescar la información obtenida desde GET /api/usuarios/perfil.
        /// </summary>
        private void CargarDatos()
        {
            TxtNombre.Text = _viewModel.Perfil.NombreCompleto;
            TxtCorreo.Text = _viewModel.Perfil.CorreoInstitucional;
            TxtRol.Text = _viewModel.Perfil.Rol;
            TxtTelefono.Text = _viewModel.Perfil.Telefono;
            TxtFacultad.Text = _viewModel.Perfil.FacultadPrograma;

            ImgPerfil.Source = new BitmapImage(
                new Uri(
                    _viewModel.Perfil.RutaImagen,
                    UriKind.RelativeOrAbsolute
                )
            );
        }

        /// <summary>
        /// Abre el explorador de archivos para seleccionar
        /// una nueva imagen de perfil.
        ///
        /// Actualmente solo actualiza la imagen de forma local.
        ///
        /// Más adelante, este flujo puede conectarse con un endpoint
        /// para subir la imagen al backend y guardar la URL resultante.
        /// </summary>
        private void BtnCambiarFoto_Click(
            object sender,
            RoutedEventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Imágenes|*.png;*.jpg;*.jpeg"
            };

            if (dialog.ShowDialog() != true)
                return;

            _viewModel.ActualizarFoto(dialog.FileName);

            ImgPerfil.Source = new BitmapImage(
                new Uri(dialog.FileName)
            );
        }

        /// <summary>
        /// Abre el dialog para editar la información básica del perfil.
        ///
        /// El correo institucional no se edita desde este flujo.
        ///
        /// Actualmente actualiza datos temporales en memoria.
        ///
        /// Más adelante deberá enviar la información mediante:
        /// PUT /api/usuarios/perfil.
        /// </summary>
        private void BtnEditarPerfil_Click(
            object sender,
            RoutedEventArgs e)
        {
            EditarPerfilDialog dialog = new(_viewModel.Perfil)
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            _viewModel.ActualizarPerfil(
                dialog.PerfilResultado.NombreCompleto,
                dialog.PerfilResultado.Telefono,
                dialog.PerfilResultado.Rol,
                dialog.PerfilResultado.FacultadPrograma
            );

            CargarDatos();
        }

        /// <summary>
        /// Abre el dialog para cambiar la contraseña del usuario.
        ///
        /// Actualmente el dialog solo valida campos visualmente.
        ///
        /// Más adelante deberá usar el request generado por el dialog
        /// para consumir:
        /// PUT /api/usuarios/cambiar-contrasena.
        /// </summary>
        private void BtnCambiarContrasena_Click(
            object sender,
            RoutedEventArgs e)
        {
            CambiarContrasenaDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }

        /// <summary>
        /// Abre el dialog de confirmación para cerrar sesión.
        ///
        /// Si el usuario confirma, se retorna al LoginView.
        ///
        /// Más adelante este flujo deberá limpiar:
        /// - Token JWT.
        /// - UsuarioSesion.
        /// - Datos persistidos de autenticación.
        /// </summary>
        private void BtnCerrarSesion_Click(
            object sender,
            RoutedEventArgs e)
        {
            CerrarSesionDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            Window? ventanaPrincipal = Window.GetWindow(this);

            if (ventanaPrincipal == null)
                return;

            ventanaPrincipal.Content = new LoginView();
        }
    }
}
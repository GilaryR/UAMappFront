using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.State;
using SistemaHorario.UI.Views.Auth;
using SistemaHorario.UI.Views.Coordinadores;
using SistemaHorario.UI.Views.Dashboard;
using SistemaHorario.UI.Views.Docentes;
using SistemaHorario.UI.Views.GruposAcademicos;
using SistemaHorario.UI.Views.HistorialCambios;
using SistemaHorario.UI.Views.Horarios;
using SistemaHorario.UI.Views.Materias;
using SistemaHorario.UI.Views.Perfil;
using SistemaHorario.UI.Views.PlanAcademico;
using SistemaHorario.UI.Views.Reportes;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Shell
{
    /// <summary>
    /// Vista principal del sistema después del inicio de sesión.
    /// </summary>
    public partial class MainShellView : UserControl
    {
        public MainShellView()
        {
            InitializeComponent();

            ConfigurarDatosUsuario();
            ConectarEventos();
            CargarVistaInicial();
        }

        private void ConfigurarDatosUsuario()
        {
            topBarView.ConfigurarUsuario(
                UsuarioSesion.NombreCompleto,
                UsuarioSesion.CorreoInstitucional
            );

            userMenuView.ConfigurarUsuario(
                UsuarioSesion.NombreCompleto,
                UsuarioSesion.CorreoInstitucional,
                UsuarioSesion.Rol
            );

            sidebarView.ConfigurarMenu(UsuarioSesion.Rol);
        }

        private void ConectarEventos()
        {
            sidebarView.NavegacionSolicitada += SidebarView_NavegacionSolicitada;
            topBarView.MenuUsuarioSolicitado += TopBarView_MenuUsuarioSolicitado;
            userMenuView.PerfilSolicitado += UserMenuView_PerfilSolicitado;
            userMenuView.CerrarSesionSolicitado += UserMenuView_CerrarSesionSolicitado;
        }

        private void CargarVistaInicial()
        {
            ContentArea.Content = new DashboardView();
        }

        private void SidebarView_NavegacionSolicitada(
            object? sender,
            string vistaDestino)
        {
            userMenuView.Ocultar();

            if (vistaDestino == "Inicio")
            {
                ContentArea.Content = new DashboardView();
                return;
            }

            if (vistaDestino == "Materias")
            {
                ContentArea.Content = new MateriasView();
                return;
            }

            if (vistaDestino == "Horarios")
            {
                ContentArea.Content = new HorariosView();
                return;
            }

            if (vistaDestino == "Docentes")
            {
                ContentArea.Content = new DocentesView();
                return;
            }

            if (vistaDestino == "Coordinadores")
            {
                if (!EsAdministrador())
                {
                    MostrarAccesoDenegado();
                    return;
                }

                ContentArea.Content = new CoordinadoresView();
                return;
            }

            if (vistaDestino == "PlanAcademico")
            {
                ContentArea.Content = new PlanAcademicoView();
                return;
            }

            if (vistaDestino == "HistorialCambios")
            {
                if (!EsAdministrador())
                {
                    MostrarAccesoDenegado();
                    return;
                }

                ContentArea.Content = new HistorialCambiosView();
                return;
            }

            if (vistaDestino == "GruposAcademicos")
            {
                ContentArea.Content = new GruposAcademicosView();
                return;
            }

            if (vistaDestino == "Reportes")
            {
                ContentArea.Content = new ReportesAcademicosView();
                return;
            }

            MostrarMensajeTemporal(vistaDestino);
        }

        private void TopBarView_MenuUsuarioSolicitado(
            object? sender,
            System.EventArgs e)
        {
            userMenuView.Alternar();
        }

        private void UserMenuView_PerfilSolicitado(
            object? sender,
            System.EventArgs e)
        {
            userMenuView.Ocultar();
            ContentArea.Content = new PerfilView();
        }

        private void UserMenuView_CerrarSesionSolicitado(
            object? sender,
            System.EventArgs e)
        {
            userMenuView.Ocultar();

            CerrarSesionDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            ApiClient.Token = null;
            UsuarioSesion.CerrarSesion();

            Window? ventanaPrincipal = Window.GetWindow(this);

            if (ventanaPrincipal == null)
            {
                return;
            }

            ventanaPrincipal.Content = new LoginView();
        }

        private bool EsAdministrador()
        {
            return UsuarioSesion.Rol.Equals(
                "Administrador",
                System.StringComparison.OrdinalIgnoreCase
            );
        }

        private void MostrarAccesoDenegado()
        {
            MessageBox.Show(
                "No tienes permisos para acceder a esta sección.",
                "Acceso denegado",
                MessageBoxButton.OK,
                MessageBoxImage.Warning
            );
        }

        private void MostrarMensajeTemporal(string titulo)
        {
            Border contenedor = new()
            {
                Background = System.Windows.Media.Brushes.White,
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(30)
            };

            TextBlock texto = new()
            {
                Text = $"Vista: {titulo}",
                FontSize = 32,
                FontWeight = FontWeights.Bold,
                Foreground = System.Windows.Media.Brushes.Black,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            contenedor.Child = texto;
            ContentArea.Content = contenedor;
        }
    }
}
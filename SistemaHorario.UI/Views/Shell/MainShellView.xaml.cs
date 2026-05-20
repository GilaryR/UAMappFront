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
using SistemaHorario.UI.Services;
using SistemaHorario.UI.State;
using SistemaHorario.UI.Views.Auth;
using System.Windows;
using System.Windows.Controls;


namespace SistemaHorario.UI.Views.Shell
{
    /// <summary>
    /// Vista principal del sistema después del inicio de sesión.
    ///
    /// Esta vista funciona como contenedor general de la aplicación.
    ///
    /// Contiene:
    /// - SidebarView: menú lateral de navegación.
    /// - TopBarView: barra superior con información del usuario.
    /// - UserMenuView: menú desplegable del usuario.
    /// - ContentArea: zona donde se cargan las vistas principales.
    ///
    /// Actualmente se usan datos temporales mientras se conecta
    /// la autenticación real y la navegación completa.
    /// </summary>
    public partial class MainShellView : UserControl
    {
        /// <summary>
        /// Constructor de MainShellView.
        ///
        /// Inicializa los componentes visuales y conecta los eventos
        /// de navegación entre Sidebar, TopBar y UserMenu.
        /// </summary>
        public MainShellView()
        {
            InitializeComponent();
            
            ConectarEventos();
            CargarVistaInicial();

        }

        /// <summary>
        /// Conecta los eventos de las vistas internas.
        ///
        /// Permite que:
        /// - Sidebar solicite navegación.
        /// - TopBar abra/cierre el menú de usuario.
        /// - UserMenu solicite perfil o cierre de sesión.
        /// </summary>
        private void ConectarEventos()
        {
            sidebarView.NavegacionSolicitada += SidebarView_NavegacionSolicitada;
            topBarView.MenuUsuarioSolicitado += TopBarView_MenuUsuarioSolicitado;
            userMenuView.PerfilSolicitado += UserMenuView_PerfilSolicitado;
            userMenuView.CerrarSesionSolicitado += UserMenuView_CerrarSesionSolicitado;
        }

		/// <summary>
		/// Carga DashboardView como pantalla inicial.
		/// </summary>
		private void CargarVistaInicial()
		{
			ContentArea.Content = new DashboardView();
		}

		/// <summary>
		/// Maneja navegación desde Sidebar.
		/// </summary>
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
            if(vistaDestino == "Horarios")
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

		/// <summary>
		/// Evento ejecutado cuando el usuario hace clic
		/// sobre su nombre o imagen en la TopBar.
		///
		/// Muestra u oculta el menú de usuario.
		/// </summary>
		private void TopBarView_MenuUsuarioSolicitado(
            object? sender,
            System.EventArgs e)
        {
            userMenuView.Alternar();
        }

        /// <summary>
        /// Evento ejecutado cuando el usuario selecciona
        /// la opción "Mi perfil" desde el menú desplegable.
        ///
        /// Más adelante deberá navegar hacia PerfilView.
        /// </summary>
        private void UserMenuView_PerfilSolicitado(object? sender,System.EventArgs e)
        {
            userMenuView.Ocultar();

            ContentArea.Content = new PerfilView();
        }

        /// <summary>
        /// Evento ejecutado cuando el usuario selecciona
        /// la opción "Cerrar sesión".
        ///
        /// Limpia la sesión y vuelve al LoginView.
        /// </summary>
        private void UserMenuView_CerrarSesionSolicitado(
            object? sender,
            System.EventArgs e)
        {
            userMenuView.Ocultar();
            UsuarioSesion.CerrarSesion();
            ApiClient.Token = null;

            Window? ventanaPrincipal = Window.GetWindow(this);

            if (ventanaPrincipal != null)
                ventanaPrincipal.Content = new LoginView();
        }

        /// <summary>
        /// Muestra contenido temporal en el área principal.
        ///
        /// Este método se usa únicamente mientras se crean
        /// las vistas reales de cada módulo.
        /// </summary>
        /// <param name="titulo">
        /// Nombre de la sección seleccionada.
        /// </param>
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

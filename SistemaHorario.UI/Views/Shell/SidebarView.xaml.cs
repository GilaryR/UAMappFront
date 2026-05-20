using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Diagnostics;
using System.IO;

namespace SistemaHorario.UI.Views.Shell
{
    /// <summary>
    /// Vista del menú lateral principal del sistema.
    ///
    /// Esta vista se encarga de mostrar las opciones de navegación
    /// disponibles para el usuario según su rol.
    ///
    /// Reglas actuales:
    /// - Administrador: puede ver todas las opciones.
    /// - Coordinador: no puede ver Coordinadores ni Historial de cambios.
    ///
    /// Esta vista no consume la API directamente.
    /// Más adelante, el rol del usuario vendrá desde la sesión
    /// después del login real.
    /// </summary>
    public partial class SidebarView : UserControl
    {
        /// <summary>
        /// Color de fondo normal de la barra lateral.
        /// </summary>
        private readonly Brush _colorNormal = new SolidColorBrush(Color.FromRgb(83, 102, 121));

        /// <summary>
        /// Color usado para marcar la opción seleccionada.
        /// </summary>
        private readonly Brush _colorSeleccionado = new SolidColorBrush(Color.FromRgb(0, 106, 166));

        /// <summary>
        /// Lista interna de opciones del menú.
        /// </summary>
        private readonly List<MenuItemModel> _opcionesMenu = new();

        /// <summary>
        /// Evento que se ejecuta cuando el usuario selecciona
        /// una opción del menú lateral.
        /// </summary>
        public event EventHandler<string>? NavegacionSolicitada;
        
        /// <summary>
        /// Constructor de SidebarView.
        ///
        /// Inicializa los componentes visuales del menú lateral.
        ///
        /// Actualmente se carga un rol temporal de prueba
        /// mientras se implementa el sistema real de autenticación
        /// y manejo de sesión.
        ///
        /// Más adelante:
        /// - El rol vendrá desde Login.
        /// - El menú se construirá dinámicamente según permisos.
        /// </summary>
        public SidebarView()
        {
            InitializeComponent();

            // Reemplazar el rol temporal por el rol
            // real del usuario autenticado.
            ConfigurarMenu("Administrador");
        }

        /// <summary>
        /// Configura las opciones del menú según el rol recibido.
        ///
        /// Este método puede ser llamado después del login,
        /// cuando ya se conozca el rol real del usuario.
        /// </summary>
        /// <param name="rolUsuario">
        /// Rol del usuario autenticado.
        /// Ejemplo: Administrador o Coordinador.
        /// </param>
        public void ConfigurarMenu(string rolUsuario)
        {
            _opcionesMenu.Clear();

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Inicio",
                Icono = "/Assets/Icons/IcInicio.png",
                VistaDestino = "Inicio",
                Seleccionado = true
            });

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Plan Académico",
                Icono = "/Assets/Icons/IcPlanAca.png",
                VistaDestino = "PlanAcademico"
            });

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Materias",
                Icono = "/Assets/Icons/IcMaterias.png",
                VistaDestino = "Materias"
            });

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Docentes",
                Icono = "/Assets/Icons/IcDocentes.png",
                VistaDestino = "Docentes"
            });

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Grupos Académicos",
                Icono = "/Assets/Icons/IcGrupos.png",
                VistaDestino = "GruposAcademicos"
            });

            if (rolUsuario.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
            {
                _opcionesMenu.Add(new MenuItemModel
                {
                    Titulo = "Coordinadores",
                    Icono = "/Assets/Icons/IcCoordinadores.png",
                    VistaDestino = "Coordinadores"
                });
            }

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Horarios",
                Icono = "/Assets/Icons/IcHorarios.png",
                VistaDestino = "Horarios"
            });

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Reportes",
                Icono = "/Assets/Icons/IcReportes.png",
                VistaDestino = "Reportes"
            });

            if (rolUsuario.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
            {
                _opcionesMenu.Add(new MenuItemModel
                {
                    Titulo = "Historial de cambios",
                    Icono = "/Assets/Icons/IcHistorial.png",
                    VistaDestino = "HistorialCambios"
                });
            }

            _opcionesMenu.Add(new MenuItemModel
            {
                Titulo = "Manual",
                Icono = "/Assets/Icons/IcManual.png",
                VistaDestino = "Manual"
            });

            PintarMenu();
        }

        /// <summary>
        /// Construye visualmente los botones del menú lateral.
        /// </summary>
        private void PintarMenu()
        {
            PnlMenu.Children.Clear();

            foreach (MenuItemModel opcion in _opcionesMenu)
            {
                Button boton = CrearBotonMenu(opcion);
                PnlMenu.Children.Add(boton);
            }
        }

        /// <summary>
        /// Crea un botón visual para una opción del menú.
        /// </summary>
        /// <param name="opcion">
        /// Opción del menú que se desea representar.
        /// </param>
        /// <returns>
        /// Botón configurado con icono, texto y evento click.
        /// </returns>
        private Button CrearBotonMenu(MenuItemModel opcion)
        {
            Image icono = new()
            {
                Width = 28,
                Height = 28,
                Margin = new Thickness(24, 0, 14, 0),
                Stretch = Stretch.Uniform,
                Source = new BitmapImage(new Uri(opcion.Icono, UriKind.RelativeOrAbsolute))
            };

            TextBlock texto = new()
            {
                Text = opcion.Titulo,
                Foreground = Brushes.White,
                FontSize = 17,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center
            };

            StackPanel contenido = new()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            contenido.Children.Add(icono);
            contenido.Children.Add(texto);

            Button boton = new()
            {
                Height = 58,
                Content = contenido,
                Tag = opcion,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                Background = opcion.Seleccionado ? _colorSeleccionado : _colorNormal,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            boton.Click += BtnMenu_Click;

            return boton;
        }

		/// <summary>
		/// Evento ejecutado cuando el usuario hace clic
		/// sobre una opción del menú.
		///
		/// Si la opción seleccionada es "Manual",
		/// abre el archivo PDF del manual de usuario.
		///
		/// Para las demás opciones, mantiene la navegación normal
		/// mediante el evento NavegacionSolicitada.
		/// </summary>
		private void BtnMenu_Click(object sender, RoutedEventArgs e)
		{
			if (sender is not Button boton)
				return;

			if (boton.Tag is not MenuItemModel opcionSeleccionada)
				return;

			// Si el usuario selecciona la opción Manual,
			// se abre directamente el PDF y no se navega a otra vista.
			if (opcionSeleccionada.VistaDestino == "Manual")
			{
				AbrirManualUsuario();
				return;
			}

			foreach (MenuItemModel opcion in _opcionesMenu)
			{
				opcion.Seleccionado = false;
			}

			opcionSeleccionada.Seleccionado = true;

			PintarMenu();

			NavegacionSolicitada?.Invoke(this, opcionSeleccionada.VistaDestino);
		}

		/// <summary>
		/// Abre el manual de usuario en formato PDF.
		///
		/// El archivo debe estar ubicado en:
		/// Documents/manual-placeholder.pdf
		///
		/// Cuando se tenga el PDF final, solo se debe cambiar
		/// el nombre del archivo en la variable rutaManual.
		/// </summary>
		private void AbrirManualUsuario()
		{
			try
			{
				string rutaManual = Path.Combine(
					AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
					"Documents",
					"manual.pdf"
				);

				if (!File.Exists(rutaManual))
				{
					MessageBox.Show(
						"El manual de usuario no se encuentra disponible.",
						"Manual no encontrado",
						MessageBoxButton.OK,
						MessageBoxImage.Warning
					);

					return;
				}

				Process.Start(new ProcessStartInfo
				{
					FileName = rutaManual,
					UseShellExecute = true
				});
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"No se pudo abrir el manual de usuario.\n\nDetalle: {ex.Message}",
					"Error al abrir manual",
					MessageBoxButton.OK,
					MessageBoxImage.Error
				);
			}
		}
	}
}
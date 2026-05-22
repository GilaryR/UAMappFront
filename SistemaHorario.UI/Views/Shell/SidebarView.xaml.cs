using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.State;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SistemaHorario.UI.Views.Shell
{
    public partial class SidebarView : UserControl
    {
        private readonly Brush _colorNormal =
            new SolidColorBrush(Color.FromRgb(83, 102, 121));

        private readonly Brush _colorSeleccionado =
            new SolidColorBrush(Color.FromRgb(0, 106, 166));

        private readonly List<MenuItemModel> _opcionesMenu = new();

        public event EventHandler<string>? NavegacionSolicitada;

        public SidebarView()
        {
            InitializeComponent();
            ConfigurarMenu(UsuarioSesion.Rol);
        }

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

            if (EsAdministrador(rolUsuario))
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

            if (EsAdministrador(rolUsuario))
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

        private bool EsAdministrador(string rolUsuario)
        {
            return rolUsuario.Equals(
                "Administrador",
                StringComparison.OrdinalIgnoreCase
            );
        }

        private void PintarMenu()
        {
            PnlMenu.Children.Clear();

            foreach (MenuItemModel opcion in _opcionesMenu)
            {
                Button boton = CrearBotonMenu(opcion);
                PnlMenu.Children.Add(boton);
            }
        }

        private Button CrearBotonMenu(MenuItemModel opcion)
        {
            Image icono = new()
            {
                Width = 28,
                Height = 28,
                Margin = new Thickness(24, 0, 14, 0),
                Stretch = Stretch.Uniform,
                Source = new BitmapImage(
                    new Uri(opcion.Icono, UriKind.RelativeOrAbsolute)
                )
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
                Background = opcion.Seleccionado
                    ? _colorSeleccionado
                    : _colorNormal,
                BorderThickness = new Thickness(0),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            boton.Click += BtnMenu_Click;

            return boton;
        }

        private void BtnMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton)
            {
                return;
            }

            if (boton.Tag is not MenuItemModel opcionSeleccionada)
            {
                return;
            }

            foreach (MenuItemModel opcion in _opcionesMenu)
            {
                opcion.Seleccionado = false;
            }

            opcionSeleccionada.Seleccionado = true;

            PintarMenu();

            NavegacionSolicitada?.Invoke(
                this,
                opcionSeleccionada.VistaDestino
            );
        }
    }
}
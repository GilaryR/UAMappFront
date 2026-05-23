using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SistemaHorario.UI.Dialogs.Docentes
{
    public partial class DisponibilidadDocenteDialog : Window
    {
        private readonly bool _soloLectura;

        private readonly List<DisponibilidadDocenteItem> _disponibilidades;

        public List<DisponibilidadDocenteItem> DisponibilidadResultado { get; private set; }

        private readonly string[] _dias =
        [
            "Lunes",
            "Martes",
            "Miércoles",
            "Jueves",
            "Viernes",
            "Sábado"
        ];

        private readonly FranjaDisponibilidad[] _franjas =
        [
            new("7:00 AM", "07:00:00", "08:00:00", false),
            new("8:00 AM", "08:00:00", "09:00:00", false),
            new("9:00 AM", "09:00:00", "10:00:00", false),
            new("10:00 AM", "10:00:00", "11:00:00", false),
            new("11:00 AM", "11:00:00", "12:00:00", false),
            new("12:00 - 2:00", "12:00:00", "14:00:00", true),
            new("2:00 PM", "14:00:00", "15:00:00", false),
            new("3:00 PM", "15:00:00", "16:00:00", false),
            new("4:00 PM", "16:00:00", "17:00:00", false),
            new("5:00 PM", "17:00:00", "18:00:00", false),
            new("6:00 - 6:30 PM", "18:00:00", "18:30:00", true),
            new("7:00 PM", "19:00:00", "20:00:00", false),
            new("8:00 PM", "20:00:00", "21:00:00", false),
            new("9:00 PM", "21:00:00", "22:00:00", false),
            new("10:00 PM", "22:00:00", "23:00:00", false)
        ];

        public DisponibilidadDocenteDialog(
            List<DisponibilidadDocenteItem> disponibilidades,
            bool soloLectura = false)
        {
            InitializeComponent();

            _soloLectura = soloLectura;

            _disponibilidades = disponibilidades
                .Select(d => new DisponibilidadDocenteItem
                {
                    Dia = d.Dia,
                    HoraInicio = NormalizarHora(d.HoraInicio),
                    HoraFin = NormalizarHora(d.HoraFin),
                    Disponible = d.Disponible
                })
                .ToList();

            DisponibilidadResultado = _disponibilidades
                .Select(d => new DisponibilidadDocenteItem
                {
                    Dia = d.Dia,
                    HoraInicio = d.HoraInicio,
                    HoraFin = d.HoraFin,
                    Disponible = d.Disponible
                })
                .ToList();

            ConfigurarModo();

            ConstruirTablaDisponibilidad();
        }

        private void ConfigurarModo()
        {
            if (!_soloLectura)
            {
                return;
            }

            TxtTitulo.Text = "Disponibilidad docente";
            TxtSubtitulo.Text = "Consulta de franjas disponibles del docente";

            BtnGuardar.Visibility = Visibility.Collapsed;
            BtnCancelar.Content = "Cerrar";
        }

        private void ConstruirTablaDisponibilidad()
        {
            GridDisponibilidad.Children.Clear();
            GridDisponibilidad.RowDefinitions.Clear();
            GridDisponibilidad.ColumnDefinitions.Clear();

            GridDisponibilidad.ColumnDefinitions.Add(
                new ColumnDefinition { Width = new GridLength(110) });

            foreach (string dia in _dias)
            {
                GridDisponibilidad.ColumnDefinitions.Add(
                    new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });
            }

            for (int i = 0; i <= _franjas.Length; i++)
            {
                GridDisponibilidad.RowDefinitions.Add(
                    new RowDefinition { Height = new GridLength(48) });
            }

            AgregarCelda("Hora", 0, 0, true);

            for (int i = 0; i < _dias.Length; i++)
            {
                AgregarCelda(_dias[i], 0, i + 1, true);
            }

            for (int fila = 0; fila < _franjas.Length; fila++)
            {
                FranjaDisponibilidad franja = _franjas[fila];

                AgregarCelda(franja.Etiqueta, fila + 1, 0, true);

                for (int col = 0; col < _dias.Length; col++)
                {
                    AgregarCeldaSeleccionable(
                        _dias[col],
                        franja,
                        fila + 1,
                        col + 1);
                }
            }
        }

        private void AgregarCelda(
            string texto,
            int fila,
            int columna,
            bool encabezado)
        {
            Border border = new()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Background = encabezado
                    ? new SolidColorBrush(Color.FromRgb(242, 242, 242))
                    : Brushes.White,
                Child = new TextBlock
                {
                    Text = texto,
                    FontWeight = encabezado
                        ? FontWeights.Bold
                        : FontWeights.Normal,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                }
            };

            Grid.SetRow(border, fila);
            Grid.SetColumn(border, columna);

            GridDisponibilidad.Children.Add(border);
        }

        private void AgregarCeldaSeleccionable(
            string dia,
            FranjaDisponibilidad franja,
            int fila,
            int columna)
        {
            if (franja.Bloqueada)
            {
                Border bloqueado = new()
                {
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(0.5),
                    Background = new SolidColorBrush(Color.FromRgb(217, 217, 217)),
                    Child = new TextBlock
                    {
                        Text = "Bloqueado",
                        FontSize = 12,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.DimGray,
                        TextAlignment = TextAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    }
                };

                Grid.SetRow(bloqueado, fila);
                Grid.SetColumn(bloqueado, columna);

                GridDisponibilidad.Children.Add(bloqueado);
                return;
            }

            bool estaDisponible =
                ExisteDisponibilidad(dia, franja.HoraInicio);

            Border border = new()
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(0.5),
                Background = estaDisponible
                    ? new SolidColorBrush(Color.FromRgb(185, 239, 194))
                    : Brushes.White,
                Cursor = _soloLectura
                    ? Cursors.Arrow
                    : Cursors.Hand,
                Tag = new DatosCeldaDisponibilidad(dia, franja),
                Child = new TextBlock
                {
                    Text = estaDisponible ? "Disponible" : "",
                    FontSize = 12,
                    FontWeight = FontWeights.Bold,
                    Foreground = new SolidColorBrush(Color.FromRgb(26, 158, 59)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            if (!_soloLectura)
            {
                border.MouseLeftButtonDown += CeldaDisponibilidad_Click;
            }

            Grid.SetRow(border, fila);
            Grid.SetColumn(border, columna);

            GridDisponibilidad.Children.Add(border);
        }

        private bool ExisteDisponibilidad(
            string dia,
            string horaInicio)
        {
            return _disponibilidades.Any(d =>
                d.Dia == dia &&
                NormalizarHora(d.HoraInicio) == horaInicio &&
                d.Disponible);
        }

        private void CeldaDisponibilidad_Click(
            object sender,
            MouseButtonEventArgs e)
        {
            if (sender is not Border border)
            {
                return;
            }

            if (border.Tag is not DatosCeldaDisponibilidad datos)
            {
                return;
            }

            CambiarEstadoFranja(
                datos.Dia,
                datos.Franja);

            ConstruirTablaDisponibilidad();
        }

        private void CambiarEstadoFranja(
            string dia,
            FranjaDisponibilidad franja)
        {
            DisponibilidadDocenteItem? existente =
                _disponibilidades.FirstOrDefault(d =>
                    d.Dia == dia &&
                    NormalizarHora(d.HoraInicio) == franja.HoraInicio);

            if (existente == null)
            {
                _disponibilidades.Add(new DisponibilidadDocenteItem
                {
                    Dia = dia,
                    HoraInicio = franja.HoraInicio,
                    HoraFin = franja.HoraFin,
                    Disponible = true
                });

                return;
            }

            _disponibilidades.Remove(existente);
        }

        private void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DisponibilidadResultado = _disponibilidades
                .Where(d => d.Disponible)
                .Select(d => new DisponibilidadDocenteItem
                {
                    Dia = d.Dia,
                    HoraInicio = NormalizarHora(d.HoraInicio),
                    HoraFin = NormalizarHora(d.HoraFin),
                    Disponible = true
                })
                .ToList();

            DialogResult = true;
            Close();
        }

        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnCerrar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private static string NormalizarHora(
            string hora)
        {
            if (string.IsNullOrWhiteSpace(hora))
            {
                return "00:00:00";
            }

            if (TimeSpan.TryParse(hora, out TimeSpan timeSpan))
            {
                return timeSpan.ToString(@"hh\:mm\:ss");
            }

            if (DateTime.TryParse(hora, out DateTime dateTime))
            {
                return dateTime.TimeOfDay.ToString(@"hh\:mm\:ss");
            }

            return hora;
        }

        private class FranjaDisponibilidad
        {
            public string Etiqueta { get; }
            public string HoraInicio { get; }
            public string HoraFin { get; }
            public bool Bloqueada { get; }

            public FranjaDisponibilidad(
                string etiqueta,
                string horaInicio,
                string horaFin,
                bool bloqueada)
            {
                Etiqueta = etiqueta;
                HoraInicio = horaInicio;
                HoraFin = horaFin;
                Bloqueada = bloqueada;
            }
        }

        private class DatosCeldaDisponibilidad
        {
            public string Dia { get; }
            public FranjaDisponibilidad Franja { get; }

            public DatosCeldaDisponibilidad(
                string dia,
                FranjaDisponibilidad franja)
            {
                Dia = dia;
                Franja = franja;
            }
        }
    }
}
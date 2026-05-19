using SistemaHorario.UI.ViewModels.HistorialCambios;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.HistorialCambios
{
    public partial class HistorialCambiosView : UserControl
    {
        private readonly HistorialCambiosViewModel _viewModel;

        public HistorialCambiosView()
        {
            InitializeComponent();
            _viewModel = new HistorialCambiosViewModel();
            Loaded += HistorialCambiosView_Loaded;
        }

        private async void HistorialCambiosView_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                await _viewModel.CargarDatosAsync();
                ConfigurarFiltros();
                CargarActividades();
            }
            catch
            {
                CargarActividades();
            }
        }

        private void ConfigurarFiltros()
        {
            var usuarios = new List<string> { "Todos" };
            usuarios.AddRange(_viewModel.Usuarios);
            CmbUsuario.ItemsSource = usuarios;
            CmbUsuario.SelectedIndex = 0;

            var modulos = new List<string> { "Todos" };
            modulos.AddRange(_viewModel.Modulos);
            CmbModulo.ItemsSource = modulos;
            CmbModulo.SelectedIndex = 0;
        }

        private void CargarActividades()
        {
            LstActividades.ItemsSource = _viewModel.Actividades;
            TxtSinResultados.Visibility = _viewModel.Actividades.Count == 0
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            DateTime? fechaDesde = DtpFechaDesde.SelectedDate;
            DateTime? fechaHasta = DtpFechaHasta.SelectedDate;

            if (fechaDesde.HasValue && fechaHasta.HasValue && fechaDesde.Value.Date > fechaHasta.Value.Date)
            {
                MessageBox.Show(
                    "La fecha inicial no puede ser mayor que la fecha final.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            _viewModel.Filtrar(ObtenerTextoCombo(CmbUsuario), ObtenerTextoCombo(CmbModulo), fechaDesde, fechaHasta);
            CargarActividades();
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            CmbUsuario.SelectedIndex = 0;
            CmbModulo.SelectedIndex = 0;
            DtpFechaDesde.SelectedDate = null;
            DtpFechaHasta.SelectedDate = null;
            _viewModel.LimpiarFiltros();
            CargarActividades();
        }

        private static string ObtenerTextoCombo(ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem item)
                return item.Content?.ToString() ?? "Todos";
            if (comboBox.SelectedItem is string texto)
                return texto;
            return "Todos";
        }
    }
}
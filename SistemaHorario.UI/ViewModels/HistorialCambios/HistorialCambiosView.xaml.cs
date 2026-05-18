using SistemaHorario.UI.ViewModels.HistorialCambios;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.HistorialCambios
{
	/// <summary>
	/// Vista principal del módulo Historial de cambios.
	///
	/// Permite visualizar las acciones realizadas dentro del sistema
	/// y filtrarlas por:
	/// - usuario,
	/// - módulo,
	/// - fecha única,
	/// - rango de fechas.
	///
	/// Actualmente trabaja con datos temporales.
	///
	/// TODO:
	/// Conectar con endpoint:
	///
	/// GET /api/historial-cambios
	///
	/// Parámetros sugeridos:
	/// - usuario
	/// - modulo
	/// - fechaDesde
	/// - fechaHasta
	/// </summary>
	public partial class HistorialCambiosView : UserControl
	{
		private readonly HistorialCambiosViewModel _viewModel;

		/// <summary>
		/// Constructor principal.
		/// </summary>
		public HistorialCambiosView()
		{
			InitializeComponent();

			_viewModel = new HistorialCambiosViewModel();

			CargarActividades();
		}

		/// <summary>
		/// Carga actividades en la lista visual.
		/// </summary>
		private void CargarActividades()
		{
			LstActividades.ItemsSource = _viewModel.Actividades;

			TxtSinResultados.Visibility =
				_viewModel.Actividades.Count == 0
					? Visibility.Visible
					: Visibility.Collapsed;
		}

		/// <summary>
		/// Aplica filtros seleccionados.
		/// </summary>
		private void BtnFiltrar_Click(
			object sender,
			RoutedEventArgs e)
		{
			DateTime? fechaDesde = DtpFechaDesde.SelectedDate;
			DateTime? fechaHasta = DtpFechaHasta.SelectedDate;

			if (fechaDesde.HasValue &&
				fechaHasta.HasValue &&
				fechaDesde.Value.Date > fechaHasta.Value.Date)
			{
				MessageBox.Show(
					"La fecha inicial no puede ser mayor que la fecha final.",
					"⚠ Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return;
			}

			_viewModel.Filtrar(
				ObtenerTextoCombo(CmbUsuario),
				ObtenerTextoCombo(CmbModulo),
				fechaDesde,
				fechaHasta);

			CargarActividades();
		}

		/// <summary>
		/// Limpia filtros y restaura todas las actividades.
		/// </summary>
		private void BtnLimpiar_Click(
			object sender,
			RoutedEventArgs e)
		{
			CmbUsuario.SelectedIndex = 0;
			CmbModulo.SelectedIndex = 0;
			DtpFechaDesde.SelectedDate = null;
			DtpFechaHasta.SelectedDate = null;

			_viewModel.LimpiarFiltros();

			CargarActividades();
		}

		/// <summary>
		/// Obtiene el texto seleccionado de un ComboBox.
		/// </summary>
		private static string ObtenerTextoCombo(ComboBox comboBox)
		{
			if (comboBox.SelectedItem is ComboBoxItem item)
				return item.Content?.ToString() ?? "Todos";

			return "Todos";
		}
	}
}
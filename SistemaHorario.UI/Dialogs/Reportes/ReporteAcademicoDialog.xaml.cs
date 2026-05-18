using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.Reportes
{
	/// <summary>
	/// Ventana para crear o visualizar reportes académicos.
	/// 
	/// En modo creación permite ingresar la información del reporte.
	/// En modo visualización muestra los datos sin permitir edición.
	/// </summary>
	public partial class ReporteAcademicoDialog : Window
	{
		public ReporteAcademicoItem ReporteResultado { get; private set; }

		public ReporteAcademicoDialog(
			List<string> tiposReporte,
			List<string> periodos)
		{
			InitializeComponent();

			ReporteResultado = new ReporteAcademicoItem();

			CargarCombos(tiposReporte, periodos);

			DpFecha.SelectedDate = DateTime.Today;
			CmbFormato.SelectedIndex = 0;
		}

		public ReporteAcademicoDialog(
			ReporteAcademicoItem reporte,
			List<string> tiposReporte,
			List<string> periodos)
		{
			InitializeComponent();

			ReporteResultado = reporte;

			CargarCombos(tiposReporte, periodos);
			CargarDatosReporte();
			BloquearFormulario();
		}

		private void CargarCombos(
			List<string> tiposReporte,
			List<string> periodos)
		{
			CmbTipoReporte.ItemsSource = tiposReporte;
			CmbPeriodo.ItemsSource = periodos;
		}

		private void CargarDatosReporte()
		{
			TxtSubtitulo.Text = "Visualizar Reporte";

			CmbTipoReporte.SelectedItem = ReporteResultado.TipoReporte;
			CmbPeriodo.SelectedItem = ReporteResultado.Periodo;
			DpFecha.SelectedDate = ReporteResultado.Fecha;
			TxtDescripcion.Text = ReporteResultado.Descripcion;

			foreach (ComboBoxItem item in CmbFormato.Items)
			{
				if (item.Content?.ToString() == ReporteResultado.FormatoInicial)
				{
					CmbFormato.SelectedItem = item;
					break;
				}
			}
		}

		private void BloquearFormulario()
		{
			CmbTipoReporte.IsEnabled = false;
			CmbPeriodo.IsEnabled = false;
			DpFecha.IsEnabled = false;
			CmbFormato.IsEnabled = false;
			TxtDescripcion.IsReadOnly = true;

			BtnGuardar.Visibility = Visibility.Collapsed;
		}

		private void BtnGuardar_Click(object sender, RoutedEventArgs e)
		{
			if (!FormularioEsValido())
				return;

			ReporteResultado.TipoReporte =
				CmbTipoReporte.SelectedItem?.ToString() ?? string.Empty;

			ReporteResultado.Periodo =
				CmbPeriodo.SelectedItem?.ToString() ?? string.Empty;

			ReporteResultado.Fecha =
				DpFecha.SelectedDate ?? DateTime.Today;

			ReporteResultado.FormatoInicial = ObtenerFormatoSeleccionado();

			ReporteResultado.Descripcion = TxtDescripcion.Text.Trim();

			ReporteResultado.Usuario = "admin";
			ReporteResultado.Detalle = "Diurna";

			DialogResult = true;
			Close();
		}

		private bool FormularioEsValido()
		{
			if (CmbTipoReporte.SelectedItem == null)
			{
				MessageBox.Show("⚠ Selecciona el tipo de reporte.");
				return false;
			}

			if (CmbPeriodo.SelectedItem == null)
			{
				MessageBox.Show("⚠ Selecciona el periodo.");
				return false;
			}

			if (DpFecha.SelectedDate == null)
			{
				MessageBox.Show("⚠ Selecciona la fecha.");
				return false;
			}

			if (CmbFormato.SelectedItem == null)
			{
				MessageBox.Show("⚠ Selecciona el formato inicial.");
				return false;
			}

			if (string.IsNullOrWhiteSpace(TxtDescripcion.Text))
			{
				MessageBox.Show("⚠ Ingresa la descripción del reporte.");
				return false;
			}

			return true;
		}

		private string ObtenerFormatoSeleccionado()
		{
			if (CmbFormato.SelectedItem is ComboBoxItem item)
				return item.Content?.ToString() ?? string.Empty;

			return CmbFormato.SelectedItem?.ToString() ?? string.Empty;
		}

		private void BtnCancelar_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}

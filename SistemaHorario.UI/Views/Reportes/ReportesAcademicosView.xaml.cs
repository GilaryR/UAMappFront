using Microsoft.Win32;
using SistemaHorario.UI.Controls;
using SistemaHorario.UI.Dialogs.Reportes;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Reportes;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Reportes
{
	/// <summary>
	/// Vista principal de reportes académicos.
	/// 
	/// Usa TablaPaginada para listar reportes.
	/// PDF y CSV se manejan como acciones de la tabla para no modificar
	/// el control compartido.
	/// </summary>
	public partial class ReportesAcademicosView : UserControl
	{
		private readonly ReportesAcademicosViewModel _viewModel = new();

		public ReportesAcademicosView()
		{
			InitializeComponent();

			ConfigurarFiltros();
			ConfigurarTabla();
			CargarTabla();
		}

		private void ConfigurarFiltros()
		{
			List<string> tipos = new()
			{
				"Todos los tipos"
			};

			tipos.AddRange(_viewModel.TiposReporte);

			CmbTipoReporte.ItemsSource = tipos;
			CmbTipoReporte.SelectedIndex = 0;
		}

		private void ConfigurarTabla()
		{
			TablaReportes.ConfigurarColumnas(new List<TableColumnDefinition>
			{
				new TableColumnDefinition
				{
					Header = "Fecha",
					Binding = "FechaTexto",
					Width = 1
				},
				new TableColumnDefinition
				{
					Header = "Tipo",
					Binding = "TipoReporte",
					Width = 1.5
				},
				new TableColumnDefinition
				{
					Header = "Usuario",
					Binding = "Usuario",
					Width = 1
				},
				new TableColumnDefinition
				{
					Header = "Detalle",
					Binding = "Detalle",
					Width = 1
				}
			});

			TablaReportes.ConfigurarAcciones(new List<TableActionDefinition>
			{
				new TableActionDefinition
				{
					Nombre = "Ver",
					Texto = "👁"
				},
				new TableActionDefinition
				{
					Nombre = "DescargarPdf",
					Texto = "PDF"
				},
				new TableActionDefinition
				{
					Nombre = "DescargarCsv",
					Texto = "CSV"
				}
			});

			TablaReportes.AccionEjecutada += TablaReportes_AccionEjecutada;
		}

		private void CargarTabla()
		{
			TablaReportes.CargarDatos(_viewModel.Reportes);
		}

		private void BtnCrearReporte_Click(object sender, RoutedEventArgs e)
		{
			ReporteAcademicoDialog dialog = new(
				_viewModel.TiposReporte,
				_viewModel.Periodos)
			{
				Owner = Window.GetWindow(this)
			};

			if (dialog.ShowDialog() != true)
				return;

			_viewModel.AgregarReporte(dialog.ReporteResultado);

			CargarTabla();

			MensajeExitoDialog exito = new("Reporte generado")
			{
				Owner = Window.GetWindow(this)
			};

			exito.ShowDialog();
		}

		private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
		{
			_viewModel.Busqueda = TxtBusqueda.Text.Trim();
			_viewModel.TipoSeleccionado =
				CmbTipoReporte.SelectedItem?.ToString() ?? "Todos los tipos";
			_viewModel.FechaSeleccionada = DpFecha.SelectedDate;

			_viewModel.AplicarFiltros();
			CargarTabla();
		}

		private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
		{
			TxtBusqueda.Clear();
			CmbTipoReporte.SelectedIndex = 0;
			DpFecha.SelectedDate = null;

			_viewModel.LimpiarFiltros();
			CargarTabla();
		}

		private void TablaReportes_AccionEjecutada(object? sender, TableActionEventArgs e)
		{
			if (e.Fila is not ReporteAcademicoItem reporte)
				return;

			if (e.Accion == "Ver")
			{
				VisualizarReporte(reporte);
				return;
			}

			if (e.Accion == "DescargarPdf")
			{
				DescargarReporte(reporte, "PDF");
				return;
			}

			if (e.Accion == "DescargarCsv")
			{
				DescargarReporte(reporte, "CSV");
			}
		}

		private void VisualizarReporte(ReporteAcademicoItem reporte)
		{
			ReporteAcademicoDialog dialog = new(
				reporte,
				_viewModel.TiposReporte,
				_viewModel.Periodos)
			{
				Owner = Window.GetWindow(this)
			};

			dialog.ShowDialog();
		}

		private void DescargarReporte(
			ReporteAcademicoItem reporte,
			string formato)
		{
			SaveFileDialog dialog = new()
			{
				Title = $"Descargar reporte {formato}",
				FileName = $"reporte_{reporte.IdReporte}.{formato.ToLower()}",
				Filter = formato == "PDF"
					? "Archivo PDF (*.pdf)|*.pdf"
					: "Archivo CSV (*.csv)|*.csv"
			};

			if (dialog.ShowDialog() != true)
				return;

			if (formato == "CSV")
			{
				File.WriteAllText(
					dialog.FileName,
					CrearContenidoCsv(reporte),
					Encoding.UTF8);
			}
			else
			{
				File.WriteAllText(
					dialog.FileName,
					CrearContenidoPdfTemporal(reporte),
					Encoding.UTF8);
			}

			MensajeExitoDialog exito = new($"Reporte {formato} descargado")
			{
				Owner = Window.GetWindow(this)
			};

			exito.ShowDialog();
		}

		private static string CrearContenidoCsv(ReporteAcademicoItem reporte)
		{
			return
				"Fecha,Tipo,Usuario,Detalle,Periodo,Descripcion\n" +
				$"{reporte.FechaTexto},{reporte.TipoReporte},{reporte.Usuario},{reporte.Detalle},{reporte.Periodo},{reporte.Descripcion}";
		}

		private static string CrearContenidoPdfTemporal(ReporteAcademicoItem reporte)
		{
			return
				"Reporte académico\n\n" +
				$"Fecha: {reporte.FechaTexto}\n" +
				$"Tipo: {reporte.TipoReporte}\n" +
				$"Usuario: {reporte.Usuario}\n" +
				$"Detalle: {reporte.Detalle}\n" +
				$"Periodo: {reporte.Periodo}\n\n" +
				reporte.Descripcion;
		}
	}
}

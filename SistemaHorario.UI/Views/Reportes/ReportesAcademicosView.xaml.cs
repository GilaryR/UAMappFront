using Microsoft.Win32;
using SistemaHorario.UI.Controls;
using SistemaHorario.UI.Dialogs.Reportes;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Reportes;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Reportes
{
    public partial class ReportesAcademicosView : UserControl
    {
        private readonly ReportesAcademicosViewModel _viewModel = new();

        public ReportesAcademicosView()
        {
            InitializeComponent();
            DataContext = _viewModel;
            Loaded += ReportesAcademicosView_Loaded;
        }

        private async void ReportesAcademicosView_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarVistaAsync();
        }

        private async Task CargarVistaAsync()
        {
            await _viewModel.CargarCatalogosAsync();

            ConfigurarFiltros();
            ConfigurarTabla();

            await _viewModel.CargarReportesAsync();
            CargarTabla();
            MostrarMensajeEstado();
        }

        private void ConfigurarFiltros()
        {
            List<string> tipos = new()
            {
                "Todos los tipos"
            };

            tipos.AddRange(_viewModel.TiposReporte);

            CmbTipoReporte.ItemsSource = tipos;

            if (CmbTipoReporte.SelectedIndex < 0)
            {
                CmbTipoReporte.SelectedIndex = 0;
            }
        }

        private void ConfigurarTabla()
        {
            TablaReportes.ConfigurarColumnas(new List<TableColumnDefinition>
            {
                new TableColumnDefinition
                {
                    Header = "Fecha",
                    Binding = "FechaTexto",
                    Width = 0.8
                },
                new TableColumnDefinition
                {
                    Header = "Tipo",
                    Binding = "TipoReporte",
                    Width = 1.3
                },
                new TableColumnDefinition
                {
                    Header = "Detalle",
                    Binding = "Detalle",
                    Width = 1.7
                },
                new TableColumnDefinition
                {
                    Header = "Periodo",
                    Binding = "Periodo",
                    Width = 0.9
                },
                new TableColumnDefinition
                {
                    Header = "Descripción",
                    Binding = "Descripcion",
                    Width = 2.7
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

            TablaReportes.AccionEjecutada -= TablaReportes_AccionEjecutada;
            TablaReportes.AccionEjecutada += TablaReportes_AccionEjecutada;
        }

        private void CargarTabla()
        {
            TablaReportes.CargarDatos(_viewModel.Reportes);
        }

        private void MostrarMensajeEstado()
        {
            TxtMensajeEstado.Text = _viewModel.MensajeEstado;
            TxtMensajeEstado.Visibility = string.IsNullOrWhiteSpace(_viewModel.MensajeEstado)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        private async void BtnActualizar_Click(object sender, RoutedEventArgs e)
        {
            await _viewModel.CargarReportesAsync();
            CargarTabla();
            MostrarMensajeEstado();

            MensajeExitoDialog exito = new("Reportes actualizados correctamente.")
            {
                Owner = Window.GetWindow(this)
            };

            exito.ShowDialog();
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.Busqueda = TxtBusqueda.Text.Trim();
            _viewModel.TipoSeleccionado = CmbTipoReporte.SelectedItem?.ToString() ?? "Todos los tipos";
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

        private async void TablaReportes_AccionEjecutada(object? sender, TableActionEventArgs e)
        {
            if (e.Fila is not ReporteAcademicoItem reporte)
            {
                return;
            }

            if (e.Accion == "Ver")
            {
                await VisualizarReporteAsync(reporte);
                return;
            }

            if (e.Accion == "DescargarPdf")
            {
                await DescargarReporteAsync(reporte, "pdf");
                return;
            }

            if (e.Accion == "DescargarCsv")
            {
                await DescargarReporteAsync(reporte, "csv");
            }
        }

        private async Task VisualizarReporteAsync(ReporteAcademicoItem reporte)
        {
            string contenido = await _viewModel.ObtenerVistaPreviaAsync(reporte);

            ReporteAcademicoDialog dialog = new(reporte, contenido)
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
        }

        private async Task DescargarReporteAsync(ReporteAcademicoItem reporte, string formato)
        {
            SaveFileDialog saveFileDialog = new()
            {
                FileName = ConstruirNombreArchivo(reporte, formato),
                Filter = formato == "pdf"
                    ? "Documento PDF (*.pdf)|*.pdf"
                    : "Archivo CSV (*.csv)|*.csv"
            };

            if (saveFileDialog.ShowDialog() != true)
            {
                return;
            }

            var respuesta = await _viewModel.DescargarReporteAsync(
                reporte,
                formato,
                saveFileDialog.FileName);

            if (!respuesta.Success)
            {
                MessageBox.Show(
                    respuesta.Message,
                    "No se pudo descargar",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            MensajeExitoDialog exito = new($"Reporte guardado en:\n{saveFileDialog.FileName}")
            {
                Owner = Window.GetWindow(this)
            };

            exito.ShowDialog();
        }

        private static string ConstruirNombreArchivo(
            ReporteAcademicoItem reporte,
            string formato)
        {
            string detalle = reporte.Detalle
                .Replace(" ", "_")
                .Replace("-", "_")
                .Replace("/", "_")
                .Replace("\\", "_");

            if (detalle.Length > 35)
            {
                detalle = detalle[..35];
            }

            string extension = formato == "pdf" ? "pdf" : "csv";
            return $"reporte_{reporte.TipoCodigo}_{detalle}.{extension}";
        }
    }
}

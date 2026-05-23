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
    public partial class ReportesAcademicosView : UserControl
    {
        private readonly ReportesAcademicosViewModel _viewModel = new();

        public ReportesAcademicosView()
        {
            InitializeComponent();

            DataContext = _viewModel;
            Loaded += ReportesAcademicosView_Loaded;
        }

        private async void ReportesAcademicosView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await _viewModel.CargarCatalogosAsync();

            ConfigurarFiltros();
            ConfigurarTabla();

            await _viewModel.CargarReportesAsync();

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

        private async void BtnCrearReporte_Click(
            object sender,
            RoutedEventArgs e)
        {
            await _viewModel.CargarReportesAsync();

            CargarTabla();

            MensajeExitoDialog exito = new(
                "Reportes actualizados desde la API.")
            {
                Owner = Window.GetWindow(this)
            };

            exito.ShowDialog();
        }

        private void BtnFiltrar_Click(
            object sender,
            RoutedEventArgs e)
        {
            _viewModel.Busqueda = TxtBusqueda.Text.Trim();

            _viewModel.TipoSeleccionado =
                CmbTipoReporte.SelectedItem?.ToString()
                ?? "Todos los tipos";

            _viewModel.FechaSeleccionada = DpFecha.SelectedDate;

            _viewModel.AplicarFiltros();

            CargarTabla();
        }

        private void BtnLimpiar_Click(
            object sender,
            RoutedEventArgs e)
        {
            TxtBusqueda.Clear();

            CmbTipoReporte.SelectedIndex = 0;

            DpFecha.SelectedDate = null;

            _viewModel.LimpiarFiltros();

            CargarTabla();
        }

        private void TablaReportes_AccionEjecutada(
            object? sender,
            TableActionEventArgs e)
        {
            if (e.Fila is not ReporteAcademicoItem reporte)
            {
                return;
            }

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
            if (formato == "PDF")
            {
                MessageBox.Show(
                    "La descarga en PDF aún no está conectada al backend.\n\n" +
                    "Se requiere implementar un endpoint real para generar el archivo PDF.",
                    "PDF no disponible",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                return;
            }

            SaveFileDialog dialog = new()
            {
                Title = "Descargar reporte CSV",
                FileName = $"reporte_{reporte.IdReporte}.csv",
                Filter = "Archivo CSV (*.csv)|*.csv"
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            File.WriteAllText(
                dialog.FileName,
                CrearContenidoCsv(reporte),
                Encoding.UTF8);

            MensajeExitoDialog exito = new("Reporte CSV descargado")
            {
                Owner = Window.GetWindow(this)
            };

            exito.ShowDialog();
        }

        private static string CrearContenidoCsv(
            ReporteAcademicoItem reporte)
        {
            return
                "Fecha,Tipo,Usuario,Detalle,Periodo,Descripcion\n" +
                $"{EscaparCsv(reporte.FechaTexto)}," +
                $"{EscaparCsv(reporte.TipoReporte)}," +
                $"{EscaparCsv(reporte.Usuario)}," +
                $"{EscaparCsv(reporte.Detalle)}," +
                $"{EscaparCsv(reporte.Periodo)}," +
                $"{EscaparCsv(reporte.Descripcion)}";
        }

        private static string EscaparCsv(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            string limpio = valor.Replace("\"", "\"\"");

            return $"\"{limpio}\"";
        }
    }
}
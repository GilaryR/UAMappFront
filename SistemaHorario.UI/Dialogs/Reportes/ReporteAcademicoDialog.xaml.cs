using SistemaHorario.UI.Models.UI;
using System.Windows;

namespace SistemaHorario.UI.Dialogs.Reportes
{
    public partial class ReporteAcademicoDialog : Window
    {
        public ReporteAcademicoDialog(
            ReporteAcademicoItem reporte,
            string contenido)
        {
            InitializeComponent();

            TxtTitulo.Text = reporte.TipoReporte;
            TxtSubtitulo.Text = $"{reporte.Detalle} · {reporte.Periodo}";
            TxtContenido.Text = contenido;
        }

        private void BtnCerrar_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

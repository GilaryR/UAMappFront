using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.PlanAcademico
{
    public partial class NuevoPlanDialog : Window
    {
        public int CantidadSemestres { get; private set; } = 10;

        public string Jornada { get; private set; } = "Por definir";

        public NuevoPlanDialog()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnContinuar_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCantidadSemestres.SelectedItem is ComboBoxItem semestreItem &&
                int.TryParse(semestreItem.Tag?.ToString(), out int cantidad))
            {
                CantidadSemestres = cantidad;
            }

            if (CmbJornada.SelectedItem is ComboBoxItem jornadaItem)
            {
                Jornada = jornadaItem.Content?.ToString() ?? "Por definir";
            }

            DialogResult = true;
        }
    }
}
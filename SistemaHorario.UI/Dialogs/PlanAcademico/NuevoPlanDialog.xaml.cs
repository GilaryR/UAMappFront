using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.PlanAcademico
{
    public partial class NuevoPlanAcademicoDialog : Window
    {
        public int CantidadSemestres { get; private set; }

        public string Jornada { get; private set; } = "Diurna";

        public NuevoPlanAcademicoDialog()
        {
            InitializeComponent();

            CantidadSemestres = 10;
            Jornada = "Diurna";
        }

        private void BtnCrear_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (CmbJornada.SelectedItem is ComboBoxItem jornadaItem)
            {
                Jornada = jornadaItem.Content?.ToString() ?? "Diurna";
            }

            if (CmbCantidadSemestres.SelectedItem is ComboBoxItem semestreItem)
            {
                string textoSemestres =
                    semestreItem.Content?.ToString() ?? "10";

                if (!int.TryParse(textoSemestres, out int cantidad))
                {
                    MessageBox.Show(
                        "La cantidad de semestres no es válida.",
                        "Validación",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);

                    return;
                }

                CantidadSemestres = cantidad;
            }

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
    }
}
using System.Windows;

namespace SistemaHorario.UI.Dialogs.Horarios
{
    /// <summary>
    /// Solicita el motivo de rechazo de un horario generado.
    /// </summary>
    public partial class RechazarHorarioDialog : Window
    {
        public string MotivoRechazo { get; private set; } = string.Empty;

        public RechazarHorarioDialog()
        {
            InitializeComponent();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnRechazar_Click(object sender, RoutedEventArgs e)
        {
            string motivo = TxtMotivo.Text.Trim();

            if (string.IsNullOrWhiteSpace(motivo))
            {
                MessageBox.Show(
                    "Debe ingresar el motivo del rechazo.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MotivoRechazo = motivo;
            DialogResult = true;
            Close();
        }
    }
}

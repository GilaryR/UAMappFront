using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.PlanAcademico;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.PlanAcademico
{
    public partial class DetalleSemestrePlanDialog : Window
    {
        private readonly DetalleSemestrePlanViewModel viewModel = new();

        public DetalleSemestrePlanDialog(
            int idPlanAcademico,
            int idSemestrePlan,
            int numeroSemestre,
            bool modoEdicion)
        {
            InitializeComponent();

            DataContext = viewModel;

            viewModel.Cargar(
                idPlanAcademico,
                idSemestrePlan,
                numeroSemestre,
                modoEdicion
            );
        }

        private void Editar_Click(object sender, RoutedEventArgs e)
        {
            viewModel.ActivarEdicion();
        }

        private async void AgregarMateria_Click(object sender, RoutedEventArgs e)
        {
            bool agregado = await viewModel.AgregarMateriaSeleccionadaAsync();

            if (!agregado)
            {
                MessageBox.Show(
                    "No se pudo agregar la materia: " + viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private async void QuitarMateria_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.CommandParameter is not MateriaItem materia)
            {
                return;
            }

            bool eliminado = await viewModel.QuitarMateriaAsync(materia.IdMateria);

            if (!eliminado)
            {
                MessageBox.Show(
                    "No se pudo quitar la materia: " + viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        private void GuardarCambios_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = viewModel.Plan?.TieneCambiosPendientes == true;
        }
    }
}

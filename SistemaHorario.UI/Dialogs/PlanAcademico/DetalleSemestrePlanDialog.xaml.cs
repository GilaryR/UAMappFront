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
            int numeroSemestre,
            bool modoEdicion)
        {
            InitializeComponent();

            DataContext = viewModel;

            viewModel.Cargar(
                idPlanAcademico,
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
            await viewModel.AgregarMateriaSeleccionadaAsync();
        }

        private async void QuitarMateria_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.CommandParameter is not MateriaItem materia)
            {
                return;
            }

            await viewModel.QuitarMateriaAsync(materia.IdMateria);
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
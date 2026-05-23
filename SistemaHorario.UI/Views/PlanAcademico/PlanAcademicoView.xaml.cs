using SistemaHorario.UI.Dialogs.PlanAcademico;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.PlanAcademico;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SistemaHorario.UI.Views.PlanAcademico
{
    /// <summary>
    /// Vista principal del módulo Plan Académico.
    /// Carga los planes desde la API y permite abrir la malla curricular.
    /// </summary>
    public partial class PlanAcademicoView : UserControl
    {
        private readonly PlanAcademicoViewModel viewModel = new();

        public PlanAcademicoView()
        {
            InitializeComponent();

            DataContext = viewModel;

            Loaded += PlanAcademicoView_Loaded;
        }

        private async void PlanAcademicoView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await viewModel.CargarPlanesAsync();

            if (!string.IsNullOrWhiteSpace(viewModel.MensajeEstado))
            {
                MessageBox.Show(
                    viewModel.MensajeEstado,
                    "Plan académico",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private async void NuevoPlan_Click(
            object sender,
            RoutedEventArgs e)
        {
            NuevoPlanAcademicoDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            PlanAcademicoItem? nuevoPlan =
                await viewModel.CrearNuevoPlanAsync(
                    dialog.CantidadSemestres,
                    dialog.Jornada
                );

            if (nuevoPlan == null)
            {
                MessageBox.Show(
                    viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return;
            }

            MensajeExitoDialog exito = new(
                "Plan académico creado correctamente.")
            {
                Owner = Window.GetWindow(this)
            };

            exito.ShowDialog();

            Navegar(
                new MallaPlanAcademicoView(
                    nuevoPlan.IdPlanAcademico,
                    true
                )
            );
        }

        private void VerMalla_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button button)
            {
                return;
            }

            if (button.CommandParameter is not PlanAcademicoItem plan)
            {
                return;
            }

            Navegar(
                new MallaPlanAcademicoView(
                    plan.IdPlanAcademico,
                    false
                )
            );
        }

        private void Navegar(UserControl vista)
        {
            ContentControl? contenedor =
                BuscarContentControlPadre(this);

            if (contenedor is not null)
            {
                contenedor.Content = vista;
            }
        }

        private static ContentControl? BuscarContentControlPadre(
            DependencyObject origen)
        {
            DependencyObject? actual = origen;

            while (actual is not null)
            {
                if (actual is ContentControl contentControl &&
                    contentControl.Name == "ContentArea")
                {
                    return contentControl;
                }

                actual = VisualTreeHelper.GetParent(actual);
            }

            return null;
        }
    }
}
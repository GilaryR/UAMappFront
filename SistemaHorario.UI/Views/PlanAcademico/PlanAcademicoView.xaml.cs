using SistemaHorario.UI.Dialogs.PlanAcademico;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.PlanAcademico;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SistemaHorario.UI.Views.PlanAcademico
{
    /// <summary>
    /// Vista principal donde se muestran los planes académicos.
    /// Desde aquí se puede:
    /// - Ver la malla de un plan existente.
    /// - Crear un nuevo plan académico.
    /// </summary>
    public partial class PlanAcademicoView : UserControl
    {
        // ViewModel que contiene la lógica y datos de la vista
        private readonly PlanAcademicoViewModel viewModel = new();

        /// <summary>
        /// Constructor de la vista.
        /// Inicializa componentes y carga los planes académicos.
        /// </summary>
        public PlanAcademicoView()
        {
            InitializeComponent();

            // Se asigna el ViewModel como contexto de datos
            DataContext = viewModel;

            // Carga la lista de planes académicos
            viewModel.CargarPlanes();
        }

        /// <summary>
        /// Evento que se ejecuta al hacer clic en el botón "Ver Malla".
        /// Abre la vista de la malla del plan seleccionado.
        /// </summary>
        private void VerMalla_Click(object sender, RoutedEventArgs e)
        {
            // Verifica que el botón tenga un plan asociado
            if (sender is not Button button ||
                button.CommandParameter is not PlanAcademicoItem plan)
            {
                return;
            }

            // Navega hacia la vista de la malla académica
            Navegar(new MallaPlanAcademicoView(plan.IdPlanAcademico, false));
        }

        /// <summary>
        /// Evento que se ejecuta al hacer clic en "Nuevo Plan".
        /// Abre un diálogo para crear un nuevo plan académico.
        /// </summary>
        private async void NuevoPlan_Click(object sender, RoutedEventArgs e)
        {
            NuevoPlanDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            if (dialog.ShowDialog() != true)
                return;

            PlanAcademicoItem? nuevoPlan =
                await viewModel.CrearNuevoPlanAsync(
                    dialog.CantidadSemestres,
                    dialog.Jornada
                );

            if (nuevoPlan == null)
            {
                MessageBox.Show(
                    "No se pudo crear el plan: " + viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            Navegar(new MallaPlanAcademicoView(nuevoPlan.IdPlanAcademico, true));
        }

        /// <summary>
        /// Cambia el contenido actual por una nueva vista.
        /// </summary>
        private void Navegar(UserControl vista)
        {
            // Busca el contenedor principal
            ContentControl? contenedor = BuscarContentControlPadre(this);

            // Si existe, reemplaza el contenido actual
            if (contenedor is not null)
            {
                contenedor.Content = vista;
            }
        }

        /// <summary>
        /// Busca el ContentControl padre llamado "ContentArea".
        /// Se usa para poder cambiar entre vistas.
        /// </summary>
        private static ContentControl? BuscarContentControlPadre(DependencyObject origen)
        {
            DependencyObject? actual = origen;

            // Recorre los elementos padres hasta encontrar el contenedor
            while (actual is not null)
            {
                if (actual is ContentControl contentControl &&
                    contentControl.Name == "ContentArea")
                {
                    return contentControl;
                }

                actual = VisualTreeHelper.GetParent(actual);
            }

            // Si no encuentra el contenedor, retorna null
            return null;
        }
    }
}
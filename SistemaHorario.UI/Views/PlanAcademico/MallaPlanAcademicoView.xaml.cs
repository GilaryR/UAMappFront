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
    /// Vista que muestra la malla de un plan académico.
    /// Permite ver los semestres, guardar cambios o volver a la lista de planes.
    /// </summary>
    public partial class MallaPlanAcademicoView : UserControl
    {
        // ViewModel que maneja los datos y acciones de esta vista
        private readonly MallaPlanAcademicoViewModel viewModel = new();

        /// <summary>
        /// Constructor de la vista.
        /// Recibe el plan académico que se va a mostrar y si está en modo creación.
        /// </summary>
        public MallaPlanAcademicoView(int idPlanAcademico, bool modoCreacion)
        {
            InitializeComponent();

            // Se asigna el ViewModel como contexto de datos de la vista
            DataContext = viewModel;

            // Carga la información del plan académico
            viewModel.CargarPlan(idPlanAcademico, modoCreacion);
        }

        /// <summary>
        /// Evento que se ejecuta al hacer clic en un semestre.
        /// Abre una ventana con el detalle de ese semestre.
        /// </summary>
        private void VerSemestre_Click(object sender, RoutedEventArgs e)
        {
            // Verifica que el botón tenga un semestre asociado
            // y que exista un plan académico cargado
            if (sender is not Button button ||
                button.CommandParameter is not SemestrePlanItem semestre ||
                viewModel.Plan is null)
            {
                return;
            }

            // Crea la ventana de detalle del semestre seleccionado
            DetalleSemestrePlanDialog dialog = new(
                viewModel.Plan.IdPlanAcademico,
                semestre.NumeroSemestre,
                viewModel.EsModoCreacion)
            {
                Owner = Window.GetWindow(this)
            };

            // Muestra la ventana y espera el resultado
            bool? resultado = dialog.ShowDialog();

            // Si hubo cambios, se actualiza la vista
            if (resultado == true)
            {
                viewModel.MarcarCambiosPendientes();
                viewModel.Refrescar();
            }
        }

        /// <summary>
        /// Evento que se ejecuta al hacer clic en "Guardar".
        /// Guarda los cambios realizados en el plan académico.
        /// </summary>
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            // Guarda los cambios del plan académico
            viewModel.GuardarCambios();

            // Muestra un mensaje indicando que se guardó correctamente
            MensajeExitoDialog dialog = new("Plan académico guardado")
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();

            // Regresa a la vista principal de planes académicos
            Navegar(new PlanAcademicoView());
        }

        /// <summary>
        /// Evento que se ejecuta al hacer clic en "Volver".
        /// Regresa a la vista principal de planes académicos.
        /// </summary>
        private void Volver_Click(object sender, RoutedEventArgs e)
        {
            Navegar(new PlanAcademicoView());
        }

        /// <summary>
        /// Cambia la vista actual por otra vista.
        /// </summary>
        private void Navegar(UserControl vista)
        {
            // Busca el contenedor principal donde se muestran las vistas
            ContentControl? contenedor = BuscarContentControlPadre(this);

            // Si lo encuentra, reemplaza el contenido actual
            if (contenedor is not null)
            {
                contenedor.Content = vista;
            }
        }

        /// <summary>
        /// Busca el contenedor llamado "ContentArea" en los elementos padres.
        /// Este contenedor es donde se cambia la pantalla actual.
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

            // Si no se encuentra el contenedor, retorna null
            return null;
        }
    }
}
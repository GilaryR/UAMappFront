using SistemaHorario.UI.Dialogs.Materias;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Materias;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Materias
{
    /// <summary>
    /// Vista principal del módulo de materias.
    ///
    /// Permite:
    /// - Visualizar materias.
    /// - Buscar por código o nombre.
    /// - Filtrar por semestre y estado.
    /// - Abrir dialogs para crear, editar y ver.
    /// - Eliminar materias usando dialogs reutilizables.
    ///
    /// Actualmente usa datos temporales.
    ///
    /// Más adelante será conectado a:
    /// - GET /api/materias
    /// - POST /api/materias
    /// - PUT /api/materias/{id}
    /// - DELETE /api/materias/{id}
    /// </summary>
    public partial class MateriasView : UserControl
    {
        /// <summary>
        /// ViewModel asociado a la vista.
        /// </summary>
        private readonly MateriasViewModel _viewModel;

        /// <summary>
        /// Constructor de MateriasView.
        /// </summary>
        public MateriasView()
        {
            InitializeComponent();

            _viewModel = new MateriasViewModel();
            DataContext = _viewModel;

            ActualizarResumen();
        }

        /// <summary>
        /// Abre el dialog para crear una materia.
        /// </summary>
        private void BtnAgregarMateria_Click(
            object sender,
            RoutedEventArgs e)
        {
            MateriaDialog dialog = new(
                ModoMateriaDialog.Crear,
                null,
                _viewModel.ObtenerMateriasDisponiblesComoPrerrequisito()
            );

            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true &&
                dialog.MateriaResultado != null)
            {
                _viewModel.AgregarMateria(dialog.MateriaResultado);
                ActualizarResumen();
            }
        }

        /// <summary>
        /// Abre el dialog en modo edición.
        /// </summary>
        private void BtnEditar_Click(
            object sender,
            RoutedEventArgs e)
        {
            MateriaItem? materia = ObtenerMateriaDesdeBoton(sender);

            if (materia == null)
                return;

            MateriaDialog dialog = new(
                ModoMateriaDialog.Editar,
                materia,
                _viewModel.ObtenerMateriasDisponiblesComoPrerrequisito(materia)
            );

            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() == true &&
                dialog.MateriaResultado != null)
            {
                _viewModel.ActualizarMateria(dialog.MateriaResultado);
                ActualizarResumen();
            }
        }

        /// <summary>
        /// Abre el dialog en modo solo lectura.
        /// </summary>
        private void BtnVer_Click(
            object sender,
            RoutedEventArgs e)
        {
            MateriaItem? materia = ObtenerMateriaDesdeBoton(sender);

            if (materia == null)
                return;

            MateriaDialog dialog = new(
                ModoMateriaDialog.Ver,
                materia,
                _viewModel.ObtenerMateriasDisponiblesComoPrerrequisito(materia)
            );

            dialog.Owner = Window.GetWindow(this);
            dialog.ShowDialog();
        }

        /// <summary>
        /// Elimina una materia usando los dialogs reutilizables.
        ///
        /// Flujo:
        /// - Muestra EliminarConfirmacionDialog.
        /// - Si confirma, elimina temporalmente.
        /// - Muestra MensajeExitoDialog.
        ///
        /// Más adelante:
        /// - Reemplazar eliminación temporal por DELETE /api/materias/{id}.
        /// </summary>
        private void BtnEliminar_Click(
            object sender,
            RoutedEventArgs e)
        {
            MateriaItem? materia = ObtenerMateriaDesdeBoton(sender);

            if (materia == null)
                return;

            EliminarConfirmacionDialog dialogEliminar =
                new EliminarConfirmacionDialog();

            dialogEliminar.Owner = Window.GetWindow(this);

            bool? resultado = dialogEliminar.ShowDialog();

            if (resultado != true)
                return;

            _viewModel.EliminarMateria(materia);
            ActualizarResumen();

            MensajeExitoDialog dialogExito =
                new MensajeExitoDialog("Materia eliminada");

            dialogExito.Owner = Window.GetWindow(this);
            dialogExito.ShowDialog();
        }

        /// <summary>
        /// Obtiene la materia asociada al botón presionado.
        /// </summary>
        private static MateriaItem? ObtenerMateriaDesdeBoton(
            object sender)
        {
            if (sender is not Button boton)
                return null;

            return boton.DataContext as MateriaItem;
        }

        /// <summary>
        /// Evento ejecutado cuando cambia el texto del SearchBox.
        ///
        /// Permite buscar por:
        /// - Código.
        /// - Nombre.
        /// </summary>
        private void SearchBoxMaterias_BusquedaCambiada(
            object sender,
            RoutedEventArgs e)
        {
            if (_viewModel == null)
                return;

            _viewModel.Busqueda =
                SearchBoxMaterias.TextoBusqueda;

            ActualizarResumen();
        }

        /// <summary>
        /// Filtra materias por semestre.
        /// </summary>
        private void CmbSemestre_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_viewModel == null)
                return;

            if (CmbSemestre.SelectedItem is ComboBoxItem item)
            {
                _viewModel.SemestreSeleccionado =
                    item.Content?.ToString() ?? "Todos";

                ActualizarResumen();
            }
        }

        /// <summary>
        /// Filtra materias por estado.
        /// </summary>
        private void CmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_viewModel == null)
                return;

            if (CmbEstado.SelectedItem is ComboBoxItem item)
            {
                _viewModel.EstadoSeleccionado =
                    item.Content?.ToString() ?? "Todos";

                ActualizarResumen();
            }
        }

        /// <summary>
        /// Limpia búsqueda, semestre y estado.
        /// </summary>
        private void BtnLimpiarFiltros_Click(
            object sender,
            RoutedEventArgs e)
        {
            SearchBoxMaterias.Limpiar();

            CmbSemestre.SelectedIndex = 0;
            CmbEstado.SelectedIndex = 0;

            _viewModel.LimpiarFiltros();

            ActualizarResumen();
        }

        /// <summary>
        /// Actualiza el texto inferior de resumen.
        /// </summary>
        private void ActualizarResumen()
        {
            if (_viewModel == null)
                return;

            TxtResumen.Text =
                $"Mostrando {_viewModel.MateriasFiltradas.Count} de {_viewModel.Materias.Count} materias";
        }
    }
}
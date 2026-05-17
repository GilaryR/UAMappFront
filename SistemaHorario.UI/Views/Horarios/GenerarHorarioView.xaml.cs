using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Horarios;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Horarios
{
    /// <summary>
    /// Vista encargada de configurar la generación automática de horarios.
    ///
    /// Actualmente trabaja con datos temporales para validar la interfaz.
    ///
    /// En la versión final, esta vista solo debe enviar el grupo seleccionado
    /// al backend. La lógica real de distribución de materias, docentes,
    /// aulas, horas y duración de bloques pertenece al motor de generación
    /// del backend.
    ///
    /// Endpoints relacionados:
    /// - GET /api/grupos/activos
    /// - POST /api/horarios/generar
    /// - GET /api/horarios/{id}/vista-previa
    ///
    /// Nota para integración:
    /// El endpoint POST /api/horarios/generar debería recibir idGrupo.
    /// Si el request actual no lo tiene, se recomienda solicitar ese ajuste.
    /// </summary>
    public partial class GenerarHorarioView : UserControl
    {
        private readonly GenerarHorarioViewModel _viewModel;

        public GenerarHorarioView()
        {
            InitializeComponent();

            _viewModel = new GenerarHorarioViewModel();

            CargarGrupos();
        }

        /// <summary>
        /// Carga los grupos disponibles en el ComboBox.
        ///
        /// TODO:
        /// Reemplazar datos mock por consumo real de:
        /// GET /api/grupos/activos.
        /// </summary>
        private void CargarGrupos()
        {
            CmbGrupo.ItemsSource = _viewModel.Grupos;

            if (_viewModel.Grupos.Count > 0)
                CmbGrupo.SelectedIndex = 0;
        }

        /// <summary>
        /// Genera un horario temporal y navega hacia la vista previa
        /// en modo aprobación.
        ///
        /// Actualmente crea un HorarioItem mock.
        ///
        /// TODO:
        /// Reemplazar esta creación local por consumo de:
        /// POST /api/horarios/generar.
        ///
        /// Después de generar, backend debe retornar el horario creado
        /// o su identificador para consultar:
        /// GET /api/horarios/{id}/vista-previa.
        /// </summary>
        private void BtnGenerar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!FormularioEsValido())
                return;

            GrupoHorarioOption grupoSeleccionado =
                (GrupoHorarioOption)CmbGrupo.SelectedItem;

            HorarioItem horarioGenerado = new()
            {
                IdHorario = 0,
                Nombre =
                    $"Horario_{grupoSeleccionado.NombreGrupo}_{DateTime.Now:yyyyMMddHHmm}",
                Grupo = grupoSeleccionado.NombreGrupo,
                Tipo = grupoSeleccionado.NombreGrupo.Contains("TAPSI")
                    ? "TAPSI"
                    : "Regular",
                Jornada = grupoSeleccionado.Jornada,
                FechaGeneracion = DateTime.Now.ToString("dd/MM/yyyy"),
                Estado = "Pendiente"
            };

            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
                return;

            contentArea.Content =
                new VistaPreviaHorarioView(
                    horarioGenerado,
                    false,
                    true);
        }

        /// <summary>
        /// Valida que el usuario seleccione un grupo antes de generar.
        /// </summary>
        private bool FormularioEsValido()
        {
            if (CmbGrupo.SelectedItem is not GrupoHorarioOption)
            {
                MessageBox.Show(
                    "Debe seleccionar un grupo antes de generar el horario.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Busca el ContentArea principal para navegar entre vistas internas.
        /// </summary>
        private ContentControl? BuscarContentArea()
        {
            DependencyObject? actual = this;

            while (actual != null)
            {
                if (actual is ContentControl content &&
                    content.Name == "ContentArea")
                {
                    return content;
                }

                actual =
                    System.Windows.Media.VisualTreeHelper.GetParent(actual);
            }

            return null;
        }
    }
}
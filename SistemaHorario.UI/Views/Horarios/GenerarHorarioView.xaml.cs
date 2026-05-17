using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Horarios;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Horarios
{
    /// <summary>
    /// Vista encargada de configurar la generación automática de horarios.
    ///
    /// Actualmente trabaja con datos temporales para validar la interfaz.
    ///
    /// Esta vista fue ajustada para respetar el contrato actual del backend:
    ///
    /// POST /api/horarios/generar
    ///
    /// {
    ///   "horaInicio": "07:00",
    ///   "horaFinal": "22:30",
    ///   "duracionBloque": 60,
    ///   "dias": ["Lunes", "Martes", "Miércoles"]
    /// }
    ///
    /// Nota:
    /// El grupo académico se conserva visualmente porque el flujo funcional
    /// del sistema lo necesita, pero el backend actual todavía no recibe IdGrupo.
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
        /// Reemplazar datos mock por GET /api/grupos/activos.
        /// </summary>
        private void CargarGrupos()
        {
            CmbGrupo.ItemsSource = _viewModel.Grupos;

            if (_viewModel.Grupos.Count > 0)
                CmbGrupo.SelectedIndex = 0;
        }

        /// <summary>
        /// Prepara el request según contrato actual del backend
        /// y navega temporalmente hacia la vista previa.
        ///
        /// TODO:
        /// Reemplazar creación mock por consumo real de:
        /// POST /api/horarios/generar.
        /// </summary>
        private void BtnGenerar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!FormularioEsValido())
                return;

            GrupoHorarioOption grupoSeleccionado =
                (GrupoHorarioOption)CmbGrupo.SelectedItem;

            GenerarHorarioRequestUI request =
                ConstruirRequestBackend();

            _viewModel.Request = request;

            // TODO:
            // Enviar request a POST /api/horarios/generar.
            //
            // Importante:
            // Este request NO incluye IdGrupo porque backend
            // actualmente no lo acepta.
            //
            // Si backend agrega IdGrupo, mapear aquí:
            // request.IdGrupo = grupoSeleccionado.IdGrupo;

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
        /// Construye el request exactamente con los campos
        /// esperados actualmente por backend.
        /// </summary>
        private GenerarHorarioRequestUI ConstruirRequestBackend()
        {
            return new GenerarHorarioRequestUI
            {
                HoraInicio = ObtenerContenidoCombo(CmbHoraInicio),
                HoraFinal = ObtenerContenidoCombo(CmbHoraFinal),
                DuracionBloque = ObtenerDuracionBloque(),
                Dias = ObtenerDiasSeleccionados()
            };
        }

        /// <summary>
        /// Valida campos requeridos antes de generar.
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

            if (ObtenerDiasSeleccionados().Count == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar al menos un día.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Obtiene la duración seleccionada en minutos.
        /// </summary>
        private int ObtenerDuracionBloque()
        {
            if (CmbDuracion.SelectedItem is ComboBoxItem item &&
                int.TryParse(item.Tag?.ToString(), out int duracion))
            {
                return duracion;
            }

            return 60;
        }

        /// <summary>
        /// Obtiene el texto seleccionado de un ComboBox.
        /// </summary>
        private static string ObtenerContenidoCombo(ComboBox comboBox)
        {
            if (comboBox.SelectedItem is ComboBoxItem item)
                return item.Content?.ToString() ?? string.Empty;

            return string.Empty;
        }

        /// <summary>
        /// Obtiene los días seleccionados para el request.
        /// </summary>
        private List<string> ObtenerDiasSeleccionados()
        {
            List<string> dias = new();

            if (ChkLunes.IsChecked == true)
                dias.Add("Lunes");

            if (ChkMartes.IsChecked == true)
                dias.Add("Martes");

            if (ChkMiercoles.IsChecked == true)
                dias.Add("Miércoles");

            if (ChkJueves.IsChecked == true)
                dias.Add("Jueves");

            if (ChkViernes.IsChecked == true)
                dias.Add("Viernes");

            if (ChkSabado.IsChecked == true)
                dias.Add("Sábado");

            return dias;
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
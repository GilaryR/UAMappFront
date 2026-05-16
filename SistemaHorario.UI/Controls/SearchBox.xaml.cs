using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Controls
{
    /// <summary>
    /// Control reutilizable de búsqueda utilizado
    /// en los distintos módulos del sistema.
    ///
    /// Permite:
    /// - Escribir texto de búsqueda.
    /// - Mostrar placeholder dinámico.
    /// - Limpiar contenido rápidamente.
    ///
    /// Este control será utilizado en:
    /// - Materias
    /// - Docentes
    /// - Horarios
    /// - Reportes
    /// - Usuarios
    /// - Historial
    /// </summary>
    public partial class SearchBox : UserControl
    {
        /// <summary>
        /// Constructor del control SearchBox.
        /// </summary>
        public SearchBox()
        {
            InitializeComponent();

            ActualizarEstadoVisual();
        }

        /// <summary>
        /// Texto placeholder mostrado cuando
        /// el campo está vacío.
        /// </summary>
        public string Placeholder
        {
            get => TxtPlaceholder.Text;
            set => TxtPlaceholder.Text = value;
        }

        /// <summary>
        /// Texto actual escrito por el usuario.
        /// </summary>
        public string TextoBusqueda
        {
            get => TxtBusqueda.Text;
            set => TxtBusqueda.Text = value;
        }

        /// <summary>
        /// Evento ejecutado cuando cambia el texto.
        /// </summary>
        private void TxtBusqueda_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualizarEstadoVisual();
        }

        /// <summary>
        /// Limpia el contenido del buscador.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtBusqueda.Clear();

            ActualizarEstadoVisual();
        }

        /// <summary>
        /// Actualiza visualmente:
        /// - Placeholder
        /// - Botón limpiar
        /// </summary>
        private void ActualizarEstadoVisual()
        {
            bool tieneTexto =
                !string.IsNullOrWhiteSpace(TxtBusqueda.Text);

            TxtPlaceholder.Visibility =
                tieneTexto
                    ? Visibility.Collapsed
                    : Visibility.Visible;

            BtnLimpiar.Visibility =
                tieneTexto
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }
}
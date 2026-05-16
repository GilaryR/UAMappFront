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
    /// - Notificar cambios de búsqueda.
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
        /// Evento público ejecutado cuando cambia el texto.
        /// </summary>
        public event RoutedEventHandler? BusquedaCambiada;

        /// <summary>
        /// Constructor del SearchBox.
        /// </summary>
        public SearchBox()
        {
            InitializeComponent();

            ActualizarEstadoVisual();
        }

        /// <summary>
        /// Placeholder mostrado cuando el campo está vacío.
        /// </summary>
        public string Placeholder
        {
            get => TxtPlaceholder.Text;
            set => TxtPlaceholder.Text = value;
        }

        /// <summary>
        /// Texto actual del buscador.
        /// </summary>
        public string TextoBusqueda
        {
            get => TxtBusqueda.Text;
            set => TxtBusqueda.Text = value;
        }

        /// <summary>
        /// Limpia el contenido del buscador.
        /// </summary>
        public void Limpiar()
        {
            TxtBusqueda.Clear();

            ActualizarEstadoVisual();

            BusquedaCambiada?.Invoke(
                this,
                new RoutedEventArgs()
            );
        }

        /// <summary>
        /// Evento ejecutado cuando cambia el texto.
        /// </summary>
        private void TxtBusqueda_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            ActualizarEstadoVisual();

            BusquedaCambiada?.Invoke(this, e);
        }

        /// <summary>
        /// Limpia el contenido desde el botón interno.
        /// </summary>
        private void BtnLimpiar_Click(
            object sender,
            RoutedEventArgs e)
        {
            Limpiar();
        }

        /// <summary>
        /// Actualiza visualmente:
        /// - Placeholder.
        /// - Botón limpiar.
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
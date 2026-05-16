using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Controls
{
    /// <summary>
    /// Control reutilizable utilizado para mostrar
    /// un panel de carga sobre una pantalla.
    ///
    /// Se usa cuando el sistema está realizando procesos como:
    /// - Consultar información desde la API.
    /// - Guardar registros.
    /// - Generar horarios.
    /// - Descargar reportes.
    /// - Cargar tablas o catálogos.
    ///
    /// Este control no consume datos ni llama servicios.
    /// Solo muestra u oculta una capa visual de espera.
    /// </summary>
    public partial class LoadingPanel : UserControl
    {
        /// <summary>
        /// Constructor del control LoadingPanel.
        /// Inicializa los componentes visuales definidos en XAML.
        /// </summary>
        public LoadingPanel()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Muestra el panel de carga con un mensaje personalizado.
        /// </summary>
        /// <param name="mensaje">
        /// Texto que se mostrará al usuario mientras se realiza la operación.
        /// </param>
        public void Mostrar(string mensaje = "Cargando...")
        {
            TxtMensaje.Text = mensaje;
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Oculta el panel de carga.
        /// </summary>
        public void Ocultar()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
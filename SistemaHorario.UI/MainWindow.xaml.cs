
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SistemaHorario.UI
{
    /// <summary>
    /// Ventana principal de la aplicación WPF.
    ///
    /// Esta ventana actúa como contenedor principal del sistema.
    ///
    /// Actualmente:
    /// - Carga la vista de LoginView.
    /// - Inicializa la aplicación gráfica.
    ///
    /// Futuramente:
    /// - Contendrá el Shell principal del sistema.
    /// - Permitirá navegación entre módulos.
    /// - Administrará Sidebar, TopBar y contenido dinámico.
    ///
    /// NOTA:
    /// Por ahora MainWindow únicamente sirve como
    /// ventana raíz para mostrar la interfaz inicial.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor de la ventana principal.
        ///
        /// Inicializa todos los componentes visuales
        /// definidos en MainWindow.xaml.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }
    }
}
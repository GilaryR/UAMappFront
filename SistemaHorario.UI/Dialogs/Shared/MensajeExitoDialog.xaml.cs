using System.Windows;

namespace SistemaHorario.UI.Dialogs.Shared
{
    /// <summary>
    /// Diálogo reutilizable para mostrar mensajes de éxito.
    ///
    /// Se utiliza después de completar correctamente una operación.
    ///
    /// Ejemplos:
    /// - Docente creado.
    /// - Materia actualizada.
    /// - Horario aprobado.
    /// - Reporte generado.
    /// </summary>
    public partial class MensajeExitoDialog : Window
    {
        /// <summary>
        /// Inicializa el diálogo con un mensaje personalizado.
        /// </summary>
        /// <param name="titulo">
        /// Mensaje principal de éxito.
        /// </param>
        public MensajeExitoDialog(string titulo)
        {
            InitializeComponent();

            TxtTitulo.Text = titulo;
        }

        /// <summary>
        /// Cierra el diálogo de éxito.
        /// </summary>
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}
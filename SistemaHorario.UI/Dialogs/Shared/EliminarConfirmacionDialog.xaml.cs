using System;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.Shared
{
    /// <summary>
    /// Diálogo reutilizable para confirmar acciones críticas escribiendo una palabra.
    /// Por defecto se usa para eliminar, pero también permite acciones como desactivar.
    /// </summary>
    public partial class EliminarConfirmacionDialog : Window
    {
        private readonly string _palabraConfirmacion;

        public EliminarConfirmacionDialog(
            string titulo = "ELIMINAR",
            string mensaje = "¿Estás seguro que deseas eliminar?\nDebes escribir la palabra eliminar.",
            string palabraConfirmacion = "eliminar")
        {
            InitializeComponent();

            _palabraConfirmacion = palabraConfirmacion;

            Title = titulo;
            TxtTitulo.Text = titulo;
            TxtMensaje.Text = mensaje;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void TxtConfirmacion_TextChanged(
            object sender,
            TextChangedEventArgs e)
        {
            bool textoValido = TxtConfirmacion.Text.Trim()
                .Equals(_palabraConfirmacion, StringComparison.OrdinalIgnoreCase);

            TxtPlaceholder.Visibility =
                string.IsNullOrWhiteSpace(TxtConfirmacion.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            BtnGuardar.IsEnabled = textoValido;
            BtnGuardar.Opacity = textoValido ? 1 : 0.55;
        }
    }
}
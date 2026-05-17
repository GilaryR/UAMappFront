using SistemaHorario.UI.Models.UI;
using System.Windows;

namespace SistemaHorario.UI.Dialogs.Perfil
{
    /// <summary>
    /// Ventana modal utilizada para cambiar
    /// la contraseña del usuario autenticado.
    ///
    /// Este dialog permite:
    /// - Validar contraseña actual.
    /// - Ingresar nueva contraseña.
    /// - Confirmar nueva contraseña.
    ///
    /// Actualmente las validaciones son únicamente visuales
    /// y se realizan en frontend.
    ///
    /// Más adelante este dialog deberá conectarse con:
    ///
    /// PUT /api/usuarios/cambiar-contrasena
    ///
    /// enviando un objeto:
    /// CambiarContrasenaRequest.
    /// </summary>
    public partial class CambiarContrasenaDialog : Window
    {
        /// <summary>
        /// Objeto que contiene la información ingresada
        /// por el usuario para cambiar la contraseña.
        ///
        /// Este request será utilizado posteriormente
        /// para enviar la información al backend.
        /// </summary>
        public CambiarContrasenaRequest Request { get; private set; } = new();

        /// <summary>
        /// Constructor principal del dialog.
        ///
        /// Inicializa todos los componentes visuales
        /// de la ventana.
        /// </summary>
        public CambiarContrasenaDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Evento ejecutado al presionar el botón Cancelar.
        ///
        /// Cierra el dialog sin guardar cambios
        /// ni generar request para backend.
        /// </summary>
        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Evento ejecutado al presionar el botón Guardar.
        ///
        /// Realiza:
        /// - Validación de campos.
        /// - Creación del request temporal.
        /// - Cierre exitoso del dialog.
        ///
        /// Más adelante este flujo deberá:
        /// - Consumir PUT /api/usuarios/cambiar-contrasena.
        /// - Manejar respuestas del backend.
        /// - Mostrar mensajes de éxito o error.
        /// </summary>
        private void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!FormularioEsValido())
                return;

            Request = new CambiarContrasenaRequest
            {
                ContrasenaActual =
                    TxtContrasenaActual.Password.Trim(),

                NuevaContrasena =
                    TxtNuevaContrasena.Password.Trim(),

                ConfirmarContrasena =
                    TxtConfirmarContrasena.Password.Trim()
            };

            DialogResult = true;

            Close();
        }

        /// <summary>
        /// Valida la información ingresada
        /// dentro del formulario.
        ///
        /// Reglas actuales:
        /// - Contraseña actual obligatoria.
        /// - Nueva contraseña obligatoria.
        /// - Longitud mínima de 6 caracteres.
        /// - Confirmación igual a nueva contraseña.
        ///
        /// Más adelante algunas validaciones podrían
        /// ser manejadas también desde backend.
        /// </summary>
        /// <returns>
        /// true si el formulario es válido.
        /// false si existe algún error de validación.
        /// </returns>
        private bool FormularioEsValido()
        {
            // Validar contraseña actual.
            if (string.IsNullOrWhiteSpace(
                TxtContrasenaActual.Password))
            {
                MessageBox.Show(
                    "La contraseña actual es obligatoria.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            // Validar nueva contraseña.
            if (string.IsNullOrWhiteSpace(
                TxtNuevaContrasena.Password))
            {
                MessageBox.Show(
                    "La nueva contraseña es obligatoria.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            // Validar longitud mínima.
            if (TxtNuevaContrasena.Password.Length < 6)
            {
                MessageBox.Show(
                    "La nueva contraseña debe tener mínimo 6 caracteres.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            // Validar coincidencia entre contraseñas.
            if (TxtNuevaContrasena.Password
                != TxtConfirmarContrasena.Password)
            {
                MessageBox.Show(
                    "Las contraseñas no coinciden.",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );

                return false;
            }

            return true;
        }
    }
}
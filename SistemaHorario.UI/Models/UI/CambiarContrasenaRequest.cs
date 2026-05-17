namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo de datos preparado para solicitar
    /// el cambio de contraseña del usuario autenticado.
    ///
    /// Este modelo se llena desde CambiarContrasenaDialog
    /// cuando el usuario presiona Guardar.
    ///
    /// Endpoint futuro:
    /// PUT /api/usuarios/cambiar-contrasena
    ///
    /// Nota para integración:
    /// El backend deberá validar la contraseña actual,
    /// la longitud de la nueva contraseña y la confirmación.
    /// La UI ya realiza validaciones básicas para mejorar
    /// la experiencia del usuario.
    /// </summary>
    public class CambiarContrasenaRequest
    {
        /// <summary>
        /// Contraseña actual ingresada por el usuario.
        /// </summary>
        public string ContrasenaActual { get; set; } = string.Empty;

        /// <summary>
        /// Nueva contraseña ingresada por el usuario.
        /// </summary>
        public string NuevaContrasena { get; set; } = string.Empty;

        /// <summary>
        /// Confirmación de la nueva contraseña.
        ///
        /// Se utiliza para validar que coincida con NuevaContrasena
        /// antes de enviar la solicitud al backend.
        /// </summary>
        public string ConfirmarContrasena { get; set; } = string.Empty;
    }
}
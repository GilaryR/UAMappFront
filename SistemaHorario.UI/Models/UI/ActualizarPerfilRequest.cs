namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo de datos preparado para actualizar
    /// la información básica del perfil del usuario autenticado.
    ///
    /// Este modelo NO representa la pantalla completa,
    /// sino únicamente los datos que se podrían enviar
    /// al backend cuando el usuario guarde cambios desde
    /// EditarPerfilDialog.
    ///
    /// Endpoint futuro:
    /// PUT /api/usuarios/perfil
    ///
    /// </summary>
    public class ActualizarPerfilRequest
    {
        /// <summary>
        /// Nombre completo actualizado del usuario.
        /// </summary>
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Número telefónico actualizado del usuario.
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Rol seleccionado visualmente.
        ///
        /// Se deja preparado para futuros cambios administrativos,
        /// aunque puede no enviarse si el endpoint de perfil
        /// solo permite editar datos personales.
        /// </summary>
        public string Rol { get; set; } = string.Empty;

        /// <summary>
        /// Facultad o programa asociado al usuario.
        ///
        /// Se conserva para que la UI no tenga que modificarse
        /// cuando backend defina este dato.
        /// </summary>
        public string FacultadPrograma { get; set; } = string.Empty;
    }
}
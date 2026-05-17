namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual del perfil del usuario autenticado.
    ///
    /// Esta clase representa los datos que se muestran
    /// en PerfilView.
    ///
    /// No es un DTO directo de backend.
    /// Es un modelo de UI pensado para que la vista trabaje
    /// de forma clara y desacoplada.
    ///
    /// Endpoint futuro para cargar datos:
    /// GET /api/usuarios/perfil
    ///
    /// Nota para integración:
    /// El servicio de perfil deberá mapear la respuesta
    /// del backend hacia este modelo visual.
    /// </summary>
    public class PerfilUsuarioItem
    {
        /// <summary>
        /// Nombre completo del usuario autenticado.
        /// </summary>
        public string NombreCompleto { get; set; } = string.Empty;

        /// <summary>
        /// Correo institucional del usuario.
        ///
        /// En la pantalla de edición se muestra bloqueado
        /// porque no debe modificarse desde Perfil.
        /// </summary>
        public string CorreoInstitucional { get; set; } = string.Empty;

        /// <summary>
        /// Rol actual del usuario.
        /// </summary>
        public string Rol { get; set; } = string.Empty;

        /// <summary>
        /// Número telefónico del usuario.
        /// </summary>
        public string Telefono { get; set; } = string.Empty;

        /// <summary>
        /// Facultad o programa asociado al usuario.
        ///
        /// Se deja como dato visual para que la UI ya esté
        /// preparada cuando backend entregue esta información.
        /// </summary>
        public string FacultadPrograma { get; set; } = string.Empty;

        /// <summary>
        /// Ruta local o remota de la imagen del usuario.
        ///
        /// Por defecto apunta al recurso local ImgUsuario.png.
        /// Si en el futuro backend entrega una URL de imagen,
        /// esta propiedad podrá almacenar esa URL.
        /// </summary>
        public string RutaImagen { get; set; }
            = "/Assets/Images/ImgUsuario.png";
    }
}
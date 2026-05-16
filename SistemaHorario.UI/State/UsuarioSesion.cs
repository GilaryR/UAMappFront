namespace SistemaHorario.UI.State
{
    /// <summary>
    /// Clase estática encargada de almacenar temporalmente
    /// la información del usuario autenticado durante la sesión.
    ///
    /// Esta clase será utilizada después del login real para guardar:
    /// - Identificador del usuario.
    /// - Nombre completo.
    /// - Correo institucional.
    /// - Rol.
    /// - Token JWT.
    ///
    /// Actualmente queda preparada para futura integración con:
    /// POST /api/auth/login
    /// GET /api/auth/perfil
    /// GET /api/usuarios/perfil
    /// </summary>
    public static class UsuarioSesion
    {
        /// <summary>
        /// Identificador interno del usuario autenticado.
        /// </summary>
        public static int IdUsuario { get; private set; }

        /// <summary>
        /// Nombre completo del usuario autenticado.
        /// </summary>
        public static string NombreCompleto { get; private set; } = string.Empty;

        /// <summary>
        /// Correo institucional del usuario autenticado.
        /// </summary>
        public static string CorreoInstitucional { get; private set; } = string.Empty;

        /// <summary>
        /// Rol del usuario autenticado.
        /// Ejemplo: Administrador o Coordinador.
        /// </summary>
        public static string Rol { get; private set; } = string.Empty;

        /// <summary>
        /// Token JWT entregado por la API después del login.
        /// </summary>
        public static string Token { get; private set; } = string.Empty;

        /// <summary>
        /// Indica si actualmente existe una sesión activa.
        /// </summary>
        public static bool EstaAutenticado =>
            !string.IsNullOrWhiteSpace(Token);

        /// <summary>
        /// Guarda la información del usuario autenticado.
        ///
        /// Este método será llamado después de un login exitoso.
        /// </summary>
        public static void IniciarSesion(
            int idUsuario,
            string nombreCompleto,
            string correoInstitucional,
            string rol,
            string token)
        {
            IdUsuario = idUsuario;
            NombreCompleto = nombreCompleto;
            CorreoInstitucional = correoInstitucional;
            Rol = rol;
            Token = token;
        }

        /// <summary>
        /// Limpia toda la información almacenada
        /// del usuario autenticado.
        ///
        /// Este método será utilizado al cerrar sesión.
        /// </summary>
        public static void CerrarSesion()
        {
            IdUsuario = 0;
            NombreCompleto = string.Empty;
            CorreoInstitucional = string.Empty;
            Rol = string.Empty;
            Token = string.Empty;
        }

        /// <summary>
        /// Retorna un nombre corto para mostrar en la interfaz.
        ///
        /// Ejemplo:
        /// "Juan Felipe Pérez Gómez" retorna "Juan Felipe".
        ///
        /// Si más adelante se requiere mostrar primer nombre
        /// y primer apellido, esta lógica se puede ajustar.
        /// </summary>
        public static string ObtenerNombreCorto()
        {
            if (string.IsNullOrWhiteSpace(NombreCompleto))
                return "Usuario";

            string[] partes = NombreCompleto.Split(
                ' ',
                System.StringSplitOptions.RemoveEmptyEntries
            );

            if (partes.Length == 1)
                return partes[0];

            return $"{partes[0]} {partes[1]}";
        }
    }
}
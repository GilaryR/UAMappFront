using SistemaHorario.UI.Models.UI;

namespace SistemaHorario.UI.ViewModels.Perfil
{
    /// <summary>
    /// ViewModel encargado de administrar la información
    /// visual del módulo Perfil.
    ///
    /// Esta clase funciona como intermediario entre:
    /// - La interfaz gráfica (PerfilView).
    /// - Los datos del usuario autenticado.
    ///
    /// Actualmente utiliza información temporal (mockup)
    /// mientras se implementa la conexión real con backend.
    ///
    /// En futuras integraciones este ViewModel deberá:
    /// - Consumir GET /api/usuarios/perfil
    /// - Consumir PUT /api/usuarios/perfil
    /// - Consumir PUT /api/usuarios/cambiar-contrasena
    ///
    /// También será el encargado de:
    /// - Actualizar la foto del usuario.
    /// - Refrescar datos visuales del perfil.
    /// - Mantener sincronizada la sesión del usuario.
    /// </summary>
    public class PerfilViewModel
    {
        /// <summary>
        /// Información visual actual del usuario autenticado.
        ///
        /// Este objeto contiene todos los datos mostrados
        /// dentro de PerfilView:
        ///
        /// - Nombre completo
        /// - Correo institucional
        /// - Rol
        /// - Teléfono
        /// - Facultad o programa
        /// - Ruta de imagen del perfil
        ///
        /// Más adelante esta información será obtenida
        /// desde la API del sistema.
        /// </summary>
        public PerfilUsuarioItem Perfil { get; set; }

        /// <summary>
        /// Constructor principal del ViewModel.
        ///
        /// Inicializa datos temporales para pruebas visuales
        /// mientras se implementa la conexión real con backend.
        ///
        /// TODO:
        /// Reemplazar esta información quemada por
        /// datos obtenidos desde:
        ///
        /// GET /api/usuarios/perfil
        /// </summary>
        public PerfilViewModel()
        {
            Perfil = new PerfilUsuarioItem
            {
                NombreCompleto = "Lina",
                CorreoInstitucional = "administrador@autonoma.edu.co",
                Rol = "Administrador",
                Telefono = "3456789012",
                FacultadPrograma =
                    "Facultad de Ingeniería / Ingeniería de Sistemas",

                RutaImagen =
                    "/Assets/Images/ImgUsuario.png"
            };
        }

        /// <summary>
        /// Actualiza temporalmente la información editable
        /// del perfil del usuario.
        ///
        /// Este método es utilizado actualmente por
        /// EditarPerfilDialog para refrescar la UI.
        ///
        /// Más adelante deberá conectarse con:
        ///
        /// PUT /api/usuarios/perfil
        ///
        /// Después de guardar correctamente en backend,
        /// este método deberá refrescar la información local.
        /// </summary>
        /// <param name="nombre">
        /// Nuevo nombre completo del usuario.
        /// </param>
        /// <param name="telefono">
        /// Nuevo teléfono del usuario.
        /// </param>
        /// <param name="rol">
        /// Nuevo rol asignado al usuario.
        /// </param>
        /// <param name="facultad">
        /// Nueva facultad o programa académico.
        /// </param>
        public void ActualizarPerfil(
            string nombre,
            string telefono,
            string rol,
            string facultad)
        {
            Perfil.NombreCompleto = nombre;
            Perfil.Telefono = telefono;
            Perfil.Rol = rol;
            Perfil.FacultadPrograma = facultad;
        }

        /// <summary>
        /// Actualiza la imagen de perfil mostrada en la UI.
        ///
        /// Actualmente únicamente cambia la ruta local
        /// seleccionada desde el explorador de archivos.
        ///
        /// Más adelante este método deberá:
        /// - Subir la imagen al backend.
        /// - Guardar la URL retornada por la API.
        /// - Actualizar la sesión del usuario.
        ///
        /// Endpoint futuro sugerido:
        ///
        /// PUT /api/usuarios/foto-perfil
        /// </summary>
        /// <param name="rutaImagen">
        /// Ruta local o URL de la nueva imagen.
        /// </param>
        public void ActualizarFoto(string rutaImagen)
        {
            Perfil.RutaImagen = rutaImagen;
        }
    }
}
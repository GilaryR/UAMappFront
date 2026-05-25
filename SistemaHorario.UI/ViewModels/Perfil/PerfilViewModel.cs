using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorarios.Application.Common;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Perfil
{
    public class PerfilViewModel
    {
        private readonly PerfilApiService _api = new();

        public PerfilUsuarioItem Perfil { get; private set; }
        public string MensajeEstado { get; private set; } = string.Empty;

        public PerfilViewModel()
        {
            Perfil = new PerfilUsuarioItem
            {
                NombreCompleto = string.Empty,
                CorreoInstitucional = string.Empty,
                Rol = string.Empty,
                Telefono = string.Empty,
                FacultadPrograma = "Universidad Autónoma de Manizales",
                RutaImagen = "pack://application:,,,/Assets/Images/ImgUsuario.png"
            };
        }

        public async Task CargarPerfilAsync()
        {
            ApiResponse<PerfilUsuarioItem> resp =
                await _api.ObtenerPerfilAsync();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo cargar el perfil."
                    : resp.Message;

                return;
            }

            Perfil = resp.Data;
            MensajeEstado = string.Empty;
        }

        public async Task<bool> ActualizarPerfilAsync(
            string nombre,
            string telefono)
        {
            ApiResponse<string> resp =
                await _api.ActualizarPerfilAsync(
                    nombre,
                    Perfil.CorreoInstitucional,
                    telefono
                );

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo actualizar el perfil."
                    : resp.Message;

                return false;
            }

            Perfil.NombreCompleto = nombre;
            Perfil.Telefono = telefono;

            MensajeEstado = string.Empty;
            return true;
        }

        public async Task<bool> CambiarContrasenaAsync(
            string contrasenaActual,
            string nuevaContrasena)
        {
            ApiResponse<string> resp =
                await _api.CambiarContrasenaAsync(
                    contrasenaActual,
                    nuevaContrasena
                );

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo cambiar la contraseña."
                    : resp.Message;

                return false;
            }

            MensajeEstado = string.Empty;
            return true;
        }

        public async Task<bool> ActualizarFotoAsync(string rutaArchivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rutaArchivo) ||
                    !System.IO.File.Exists(rutaArchivo))
                {
                    MensajeEstado = "La imagen seleccionada no existe.";
                    return false;
                }

                /*
                 * Se copia la imagen seleccionada a una carpeta propia de la aplicación.
                 * Así no dependemos de la ruta original del computador del usuario.
                 */
                string carpetaPerfil = System.IO.Path.Combine(
                    System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.ApplicationData),
                    "SistemaHorarios",
                    "Perfil"
                );

                System.IO.Directory.CreateDirectory(carpetaPerfil);

                string extension = System.IO.Path.GetExtension(rutaArchivo);

                string nombreArchivo = "foto_perfil" + extension;

                string rutaDestino = System.IO.Path.Combine(
                    carpetaPerfil,
                    nombreArchivo
                );

                System.IO.File.Copy(
                    rutaArchivo,
                    rutaDestino,
                    overwrite: true
                );

                /*
                 * Aquí se manda a la API la copia local, no la ruta original
                 * de Descargas, Escritorio o Imágenes.
                 */
                ApiResponse<string> resp =
                    await _api.ActualizarFotoPerfilAsync(rutaDestino);

                if (!resp.Success || string.IsNullOrWhiteSpace(resp.Data))
                {
                    MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                        ? "No se pudo actualizar la foto de perfil."
                        : resp.Message;

                    return false;
                }

                string rutaFoto = resp.Data.StartsWith("http")
                    ? resp.Data
                    : "http://localhost:5023" + resp.Data;

                Perfil.RutaImagen = rutaFoto;

                MensajeEstado = string.Empty;
                return true;
            }
            catch (System.Exception ex)
            {
                MensajeEstado = ex.Message;
                return false;
            }
        }
    }
}
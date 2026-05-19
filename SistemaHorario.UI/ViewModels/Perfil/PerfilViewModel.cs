using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Perfil
{
    public class PerfilViewModel
    {
        private readonly PerfilApiService _api = new();
        public PerfilUsuarioItem Perfil { get; set; }
        public string MensajeEstado { get; private set; } = string.Empty;

        public PerfilViewModel()
        {
            Perfil = new PerfilUsuarioItem
            {
                NombreCompleto = "",
                CorreoInstitucional = "",
                Rol = "",
                Telefono = "",
                FacultadPrograma = "",
                RutaImagen = "/Assets/Images/ImgUsuario.png"
            };
            _ = CargarPerfilAsync();
        }

        public async Task CargarPerfilAsync()
        {
            var resp = await _api.ObtenerPerfilAsync();
            if (resp.Success && resp.Data != null)
            {
                Perfil.NombreCompleto = resp.Data.NombreCompleto;
                Perfil.CorreoInstitucional = resp.Data.CorreoInstitucional;
                Perfil.Rol = resp.Data.Rol;
                Perfil.FacultadPrograma = resp.Data.FacultadPrograma;
            }
            else MensajeEstado = resp.Message;
        }

        public async Task<bool> ActualizarPerfilAsync(string nombre, string telefono, string rol, string facultad)
        {
            var resp = await _api.ActualizarPerfilAsync(nombre, Perfil.CorreoInstitucional);
            if (resp.Success)
            {
                Perfil.NombreCompleto = nombre;
                Perfil.Telefono = telefono;
                Perfil.Rol = rol;
                Perfil.FacultadPrograma = facultad;
                return true;
            }
            MensajeEstado = resp.Message;
            return false;
        }

        public void ActualizarPerfil(string nombre, string telefono, string rol, string facultad)
            => _ = ActualizarPerfilAsync(nombre, telefono, rol, facultad);

        public void ActualizarFoto(string rutaImagen) => Perfil.RutaImagen = rutaImagen;
    }
}

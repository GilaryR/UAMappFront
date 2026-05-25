using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorarios.Application.Common;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Coordinadores
{
    public class AgregarEditarCoordinadorViewModel
    {
        private readonly CoordinadoresApiService _api = new();

        public CoordinadorItem Coordinador { get; private set; }

        public bool EsEdicion { get; private set; }

        public string MensajeEstado { get; private set; } = string.Empty;

        public AgregarEditarCoordinadorViewModel()
        {
            EsEdicion = false;

            Coordinador = new CoordinadorItem
            {
                NombreCompleto = string.Empty,
                Cedula = string.Empty,
                CorreoInstitucional = string.Empty,
                Celular = string.Empty,
                Rol = "Coordinador",
                Estado = "Activo",
                ContrasenaInicial = "Coordinador123!"
            };
        }

        public AgregarEditarCoordinadorViewModel(CoordinadorItem coordinador)
        {
            EsEdicion = true;

            Coordinador = new CoordinadorItem
            {
                IdCoordinador = coordinador.IdCoordinador,
                NombreCompleto = coordinador.NombreCompleto,
                Cedula = coordinador.Cedula,
                CorreoInstitucional = coordinador.CorreoInstitucional,
                Celular = coordinador.Celular,
                Rol = "Coordinador",
                Estado = coordinador.Estado
            };
        }

        public async Task<bool> GuardarAsync()
        {
            Coordinador.Rol = "Coordinador";

            ApiResponse<string> resp = EsEdicion
                ? await _api.ActualizarCoordinadorAsync(Coordinador)
                : await _api.CrearCoordinadorAsync(Coordinador);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo guardar el coordinador."
                    : resp.Message;

                return false;
            }

            MensajeEstado = string.Empty;
            return true;
        }
    }
}

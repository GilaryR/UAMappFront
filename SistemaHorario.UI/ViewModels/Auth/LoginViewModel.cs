using SistemaHorario.Infrastructure.Api;
using SistemaHorario.UI.ViewModels.Base;
using SistemaHorarios.Application.Common;
using SistemaHorarios.Application.Common.Auth;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Auth
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly AuthApiService _authApiService;

        private string _correoInstitucional = string.Empty;
        private string _contrasena = string.Empty;
        private bool _mantenerSesion;

        public string CorreoInstitucional
        {
            get => _correoInstitucional;
            set => SetProperty(ref _correoInstitucional, value);
        }

        public string Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }

        public bool MantenerSesion
        {
            get => _mantenerSesion;
            set => SetProperty(ref _mantenerSesion, value);
        }

        public LoginViewModel()
        {
            _authApiService = new AuthApiService();
        }

        public bool CorreoEsValido()
        {
            return !string.IsNullOrWhiteSpace(CorreoInstitucional);
        }

        public bool CorreoTieneFormatoValido()
        {
            return CorreoInstitucional.Contains("@");
        }

        public bool ContrasenaEsValida()
        {
            return !string.IsNullOrWhiteSpace(Contrasena);
        }

        public bool ContrasenaCumpleLongitudMinima()
        {
            return Contrasena.Length >= 6;
        }

        public bool FormularioEsValido()
        {
            return CorreoEsValido()
                && ContrasenaEsValida()
                && ContrasenaCumpleLongitudMinima();
        }

        public async Task<ApiResponse<LoginData>> IniciarSesionAsync()
        {
            var request = new LoginRequest
            {
                CorreoInstitucional = CorreoInstitucional,
                Contrasena = Contrasena
            };

            return await _authApiService.LoginAsync(request);
        }
    }
}
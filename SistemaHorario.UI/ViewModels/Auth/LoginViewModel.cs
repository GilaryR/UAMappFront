using SistemaHorario.Infrastructure.Api;
using SistemaHorario.UI.ViewModels.Base;
using SistemaHorarios.Application.Common;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Auth
{
    /// <summary>
    /// ViewModel del Login.
    ///
    /// Centraliza el estado y las validaciones básicas
    /// de la pantalla de inicio de sesión.
    ///
    /// Actualmente NO consume la API.
    ///
    /// Más adelante deberá reemplazarse por el consumo real de:
    /// POST /api/auth/login.
    ///
    /// El endpoint espera:
    /// - correoInstitucional
    /// - contrasena
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        private readonly ApiHealthService _apiHealthService;

        private string _correoInstitucional = string.Empty;
        private string _contrasena = string.Empty;
        private bool _mantenerSesion;
        private string _mensajeEstado = string.Empty;

        /// <summary>
        /// Correo institucional ingresado por el usuario.
        /// </summary>
        public string CorreoInstitucional
        {
            get => _correoInstitucional;
            set => SetProperty(ref _correoInstitucional, value);
        }

        /// <summary>
        /// Contraseña ingresada por el usuario.
        /// </summary>
        public string Contrasena
        {
            get => _contrasena;
            set => SetProperty(ref _contrasena, value);
        }

        /// <summary>
        /// Indica si el usuario desea mantener sesión iniciada.
        /// Queda preparado para futura implementación.
        /// </summary>
        public bool MantenerSesion
        {
            get => _mantenerSesion;
            set => SetProperty(ref _mantenerSesion, value);
        }

        /// <summary>
        /// Constructor del ViewModel.
        /// </summary>
        public LoginViewModel()
        {
            _apiHealthService = new ApiHealthService();
        }

        /// <summary>
        /// Valida que el correo institucional no esté vacío.
        /// </summary>
        public bool CorreoEsValido()
        {
            return !string.IsNullOrWhiteSpace(CorreoInstitucional);
        }

        /// <summary>
        /// Valida que la contraseña no esté vacía.
        /// </summary>
        public bool ContrasenaEsValida()
        {
            return !string.IsNullOrWhiteSpace(Contrasena);
        }

        /// <summary>
        /// Valida que la contraseña cumpla mínimo 6 caracteres,
        /// como lo exige el contrato LoginRequestDto.
        /// </summary>
        public bool ContrasenaCumpleLongitudMinima()
        {
            return Contrasena.Length >= 6;
        }

        /// <summary>
        /// Valida el formulario completo.
        /// </summary>
        public bool FormularioEsValido()
        {
            return CorreoEsValido()
                && ContrasenaEsValida()
                && ContrasenaCumpleLongitudMinima();
        }
    }
}
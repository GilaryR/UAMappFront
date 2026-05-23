using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Materias
{
    public class MateriasViewModel : ViewModelBase
    {
        private readonly MateriasApiService _api = new();

        private string _busqueda = string.Empty;
        private string _semestreSeleccionado = "Todos";
        private string _estadoSeleccionado = "Todos";
        private string _mensajeEstado = string.Empty;

        public ObservableCollection<MateriaItem> Materias { get; } = new();

        public ObservableCollection<MateriaItem> MateriasFiltradas { get; } = new();

        public string Busqueda
        {
            get => _busqueda;
            set
            {
                if (SetProperty(ref _busqueda, value))
                {
                    AplicarFiltros();
                }
            }
        }

        public string SemestreSeleccionado
        {
            get => _semestreSeleccionado;
            set
            {
                if (SetProperty(ref _semestreSeleccionado, value))
                {
                    AplicarFiltros();
                }
            }
        }

        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                {
                    AplicarFiltros();
                }
            }
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            private set => SetProperty(ref _mensajeEstado, value);
        }

        public async Task CargarMateriasAsync()
        {
            var resp = await _api.ObtenerMateriasAsync();

            Materias.Clear();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar las materias."
                    : resp.Message;

                AplicarFiltros();
                return;
            }

            foreach (MateriaItem materia in resp.Data)
            {
                Materias.Add(materia);
            }

            MensajeEstado = string.Empty;
            AplicarFiltros();
        }

        public void AplicarFiltros()
        {
            var resultado = Materias.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                string busqueda = Busqueda.ToLower();

                resultado = resultado.Where(m =>
                    m.Codigo.ToLower().Contains(busqueda) ||
                    m.Nombre.ToLower().Contains(busqueda));
            }

            if (SemestreSeleccionado != "Todos" &&
                int.TryParse(SemestreSeleccionado, out int semestre))
            {
                resultado = resultado.Where(m =>
                    m.Semestre == semestre);
            }

            if (EstadoSeleccionado != "Todos")
            {
                bool activa = EstadoSeleccionado == "Activa";

                resultado = resultado.Where(m =>
                    m.Activa == activa);
            }

            MateriasFiltradas.Clear();

            foreach (MateriaItem materia in resultado)
            {
                MateriasFiltradas.Add(materia);
            }
        }

        public void LimpiarFiltros()
        {
            Busqueda = string.Empty;
            SemestreSeleccionado = "Todos";
            EstadoSeleccionado = "Todos";

            AplicarFiltros();
        }

        public async Task<bool> AgregarMateriaAsync(MateriaItem materia)
        {
            var resp = await _api.CrearMateriaAsync(materia);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo crear la materia."
                    : resp.Message;

                return false;
            }

            await CargarMateriasAsync();

            MensajeEstado = string.Empty;
            return true;
        }

        public async Task<bool> ActualizarMateriaAsync(MateriaItem materia)
        {
            var resp = await _api.ActualizarMateriaAsync(materia);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo actualizar la materia."
                    : resp.Message;

                return false;
            }

            await CargarMateriasAsync();

            MensajeEstado = string.Empty;
            return true;
        }

        public async Task<bool> InactivarMateriaAsync(MateriaItem materia)
        {
            var resp = await _api.InactivarMateriaAsync(materia.IdMateria);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo inactivar la materia."
                    : resp.Message;

                return false;
            }

            await CargarMateriasAsync();

            MensajeEstado = string.Empty;
            return true;
        }

        public async Task<bool> ActivarMateriaAsync(MateriaItem materia)
        {
            var resp = await _api.ActivarMateriaAsync(materia.IdMateria);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo activar la materia."
                    : resp.Message;

                return false;
            }

            await CargarMateriasAsync();

            MensajeEstado = string.Empty;
            return true;
        }

        public ObservableCollection<MateriaItem> ObtenerMateriasDisponiblesComoPrerrequisito(
            MateriaItem? materiaActual = null)
        {
            ObservableCollection<MateriaItem> resultado = new();

            foreach (MateriaItem materia in Materias)
            {
                if (materiaActual != null &&
                    materia.IdMateria == materiaActual.IdMateria)
                {
                    continue;
                }

                resultado.Add(materia);
            }

            return resultado;
        }
    }
}
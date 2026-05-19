
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
            set { if (SetProperty(ref _busqueda, value)) AplicarFiltros(); }
        }

        public string SemestreSeleccionado
        {
            get => _semestreSeleccionado;
            set { if (SetProperty(ref _semestreSeleccionado, value)) AplicarFiltros(); }
        }

        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set { if (SetProperty(ref _estadoSeleccionado, value)) AplicarFiltros(); }
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            set => SetProperty(ref _mensajeEstado, value);
        }

        public MateriasViewModel()
        {
            _ = CargarMateriasAsync();
        }

        public async Task CargarMateriasAsync()
        {
            var resp = await _api.ObtenerMateriasAsync();
            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = "Error al cargar materias: " + resp.Message;
                return;
            }
            Materias.Clear();
            foreach (var m in resp.Data)
                Materias.Add(m);
            AplicarFiltros();
            MensajeEstado = Materias.Count + " materias cargadas.";
        }

        public void AplicarFiltros()
        {
            var resultado = Materias.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                string b = Busqueda.ToLower();
                resultado = resultado.Where(m =>
                    m.Codigo.ToLower().Contains(b) ||
                    m.Nombre.ToLower().Contains(b));
            }
            if (SemestreSeleccionado != "Todos" &&
                int.TryParse(SemestreSeleccionado, out int sem))
                resultado = resultado.Where(m => m.Semestre == sem);
            if (EstadoSeleccionado != "Todos")
            {
                bool activa = EstadoSeleccionado == "Activa";
                resultado = resultado.Where(m => m.Activa == activa);
            }
            MateriasFiltradas.Clear();
            foreach (var m in resultado)
                MateriasFiltradas.Add(m);
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
            if (resp.Success) { await CargarMateriasAsync(); return true; }
            MensajeEstado = "Error al crear: " + resp.Message;
            return false;
        }

        public async Task<bool> ActualizarMateriaAsync(MateriaItem materia)
        {
            var resp = await _api.ActualizarMateriaAsync(materia);
            if (resp.Success) { await CargarMateriasAsync(); return true; }
            MensajeEstado = "Error al actualizar: " + resp.Message;
            return false;
        }

        public async Task<bool> EliminarMateriaAsync(MateriaItem materia)
        {
            var resp = await _api.EliminarMateriaAsync(materia.IdMateria);
            if (resp.Success) { Materias.Remove(materia); AplicarFiltros(); return true; }
            MensajeEstado = "Error al eliminar: " + resp.Message;
            return false;
        }

        public void AgregarMateria(MateriaItem materia) => _ = AgregarMateriaAsync(materia);
        public void ActualizarMateria(MateriaItem materia) => _ = ActualizarMateriaAsync(materia);
        public void EliminarMateria(MateriaItem materia) => _ = EliminarMateriaAsync(materia);

        public ObservableCollection<MateriaItem> ObtenerMateriasDisponiblesComoPrerrequisito(
            MateriaItem? materiaActual = null)
        {
            ObservableCollection<MateriaItem> resultado = new();
            foreach (var m in Materias)
            {
                if (materiaActual != null && m.IdMateria == materiaActual.IdMateria)
                    continue;
                resultado.Add(m);
            }
            return resultado;
        }
    }
}

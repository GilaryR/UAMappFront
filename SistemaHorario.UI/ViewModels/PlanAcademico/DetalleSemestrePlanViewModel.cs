using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.PlanAcademico
{
    public class DetalleSemestrePlanViewModel : INotifyPropertyChanged
    {
        private readonly PlanAcademicoApiService _api = new();
        private readonly MateriasApiService _materiasApi = new();
        private PlanAcademicoItem? _plan;
        private SemestrePlanItem? _semestre;
        private MateriaItem? _materiaSeleccionada;
        private bool _estaEditando;
        private int _idPlanAcademico;
        private int _idSemestrePlan;
        private int _numeroSemestre;

        public PlanAcademicoItem? Plan
        {
            get => _plan;
            set
            {
                _plan = value;
                OnPropertyChanged();
                NotificarEncabezado();
            }
        }

        public SemestrePlanItem? Semestre
        {
            get => _semestre;
            set
            {
                _semestre = value;
                OnPropertyChanged();
                NotificarEncabezado();
            }
        }

        public ObservableCollection<MateriaItem> MateriasDisponibles { get; } = new();

        public MateriaItem? MateriaSeleccionada
        {
            get => _materiaSeleccionada;
            set
            {
                _materiaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        public bool EstaEditando
        {
            get => _estaEditando;
            set
            {
                _estaEditando = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TextoBotonEditar));
            }
        }

        public string TextoBotonEditar => EstaEditando ? "Editando" : "Editar";

        public string TituloSemestre =>
            Semestre == null ? "Semestre" : $"Semestre {Semestre.NumeroSemestre}";

        public string ResumenSemestre =>
            Semestre == null
                ? "0 materias · 0 créditos"
                : $"{Semestre.TotalMaterias} materias · {Semestre.TotalCreditos} créditos";

        public string JornadaSemestre =>
            string.IsNullOrWhiteSpace(Semestre?.Jornada)
                ? "Jornada: Por definir"
                : $"Jornada: {Semestre.Jornada}";

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Cargar(
            int idPlanAcademico,
            int idSemestrePlan,
            int numeroSemestre,
            bool modoEdicion)
        {
            _ = CargarAsync(idPlanAcademico, idSemestrePlan, numeroSemestre, modoEdicion);
        }

        public async Task CargarAsync(
            int idPlanAcademico,
            int idSemestrePlan,
            int numeroSemestre,
            bool modoEdicion)
        {
            _idPlanAcademico = idPlanAcademico;
            _idSemestrePlan = idSemestrePlan;
            _numeroSemestre = numeroSemestre;
            EstaEditando = modoEdicion;

            var resp = await _api.ObtenerPlanPorIdAsync(idPlanAcademico);

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo cargar el plan académico."
                    : resp.Message;
                await CargarMateriasDisponiblesAsync();
                NotificarEncabezado();
                return;
            }

            Plan = resp.Data;
            Semestre = Plan.Semestres.FirstOrDefault(s => s.IdSemestrePlan == idSemestrePlan)
                ?? Plan.Semestres.FirstOrDefault(s => s.NumeroSemestre == numeroSemestre);

            if (Semestre == null)
            {
                MensajeEstado = "No se encontró el semestre seleccionado en el plan.";
            }

            await CargarMateriasDisponiblesAsync();
            NotificarEncabezado();
        }

        public void ActivarEdicion()
        {
            EstaEditando = true;
        }

        public string MensajeEstado { get; private set; } = string.Empty;

        public async Task<bool> AgregarMateriaSeleccionadaAsync()
        {
            MensajeEstado = string.Empty;

            if (Semestre == null)
            {
                MensajeEstado = "No se encontró el semestre seleccionado.";
                return false;
            }

            if (MateriaSeleccionada == null)
            {
                MensajeEstado = "Debe seleccionar una materia.";
                return false;
            }

            var resp = await _api.AgregarMateriaAsync(
                Semestre.IdSemestrePlan,
                MateriaSeleccionada.IdMateria
            );

            if (!resp.Success)
            {
                MensajeEstado = resp.Message;
                return false;
            }

            if (resp.Data == null)
            {
                MensajeEstado = "El backend no devolvió la materia agregada.";
                return false;
            }

            MarcarPlanComoModificado();
            MateriaSeleccionada = null;

            await RefrescarSemestreDesdeBackendAsync();
            NotificarEncabezado();

            return true;
        }

        public async Task<bool> QuitarMateriaAsync(int idMateria)
        {
            MensajeEstado = string.Empty;

            if (Semestre == null)
            {
                MensajeEstado = "No se encontró el semestre seleccionado.";
                return false;
            }

            var materia = Semestre.Materias.FirstOrDefault(m => m.IdMateria == idMateria);
            if (materia == null)
            {
                MensajeEstado = "No se encontró la materia seleccionada en el semestre.";
                return false;
            }

            var resp = await _api.EliminarMateriaAsync(materia.IdMateriaPlan);
            if (!resp.Success)
            {
                MensajeEstado = resp.Message;
                return false;
            }

            MarcarPlanComoModificado();
            await RefrescarSemestreDesdeBackendAsync();
            NotificarEncabezado();
            return true;
        }

        public async Task<bool> CargarMateriasDisponiblesAsync()
        {
            MateriasDisponibles.Clear();

            var resp = await _materiasApi.ObtenerMateriasAsync();
            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar las materias disponibles."
                    : resp.Message;
                return false;
            }

            var idsAsignados = Semestre?.Materias
                .Select(m => m.IdMateria)
                .ToHashSet() ?? new System.Collections.Generic.HashSet<int>();

            foreach (var m in resp.Data.Where(m => m.Activa && !idsAsignados.Contains(m.IdMateria)))
                MateriasDisponibles.Add(m);

            OnPropertyChanged(nameof(MateriasDisponibles));
            return true;
        }

        public void RefrescarResumen()
        {
            NotificarEncabezado();
        }

        private void MarcarPlanComoModificado()
        {
            if (Plan == null) return;
            Plan.TieneCambiosPendientes = true;
            Plan.RecalcularTotalesDesdeSemestres();
        }

        private async Task RefrescarSemestreDesdeBackendAsync()
        {
            if (_idPlanAcademico <= 0)
                return;

            await CargarAsync(
                _idPlanAcademico,
                _idSemestrePlan,
                _numeroSemestre,
                EstaEditando);

            MarcarPlanComoModificado();
        }

        private void NotificarEncabezado()
        {
            OnPropertyChanged(nameof(TituloSemestre));
            OnPropertyChanged(nameof(ResumenSemestre));
            OnPropertyChanged(nameof(JornadaSemestre));
            OnPropertyChanged(nameof(Semestre));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

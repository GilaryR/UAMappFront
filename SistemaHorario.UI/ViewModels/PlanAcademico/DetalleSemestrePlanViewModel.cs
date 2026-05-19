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

        public void Cargar(int idPlanAcademico, int numeroSemestre, bool modoEdicion)
        {
            _ = CargarAsync(idPlanAcademico, numeroSemestre, modoEdicion);
        }

        public async Task CargarAsync(int idPlanAcademico, int numeroSemestre, bool modoEdicion)
        {
            EstaEditando = modoEdicion;
            var resp = await _api.ObtenerPlanPorIdAsync(idPlanAcademico);
            if (resp.Success && resp.Data != null)
            {
                Plan = resp.Data;
                Semestre = Plan.Semestres.FirstOrDefault(s => s.NumeroSemestre == numeroSemestre);
            }
            await CargarMateriasDisponiblesAsync();
            NotificarEncabezado();
        }

        public void ActivarEdicion()
        {
            EstaEditando = true;
        }

        public async Task AgregarMateriaSeleccionadaAsync()
        {
            if (Semestre == null || MateriaSeleccionada == null) return;

            var resp = await _api.AgregarMateriaAsync(Semestre.IdSemestre, MateriaSeleccionada.IdMateria);
            if (!resp.Success || resp.Data == null) return;

            var nuevaMateria = new MateriaItem
            {
                IdMateria = resp.Data.IdMateria,
                IdMateriaPlan = resp.Data.IdMateriaPlan,
                Codigo = resp.Data.Codigo,
                Nombre = resp.Data.Nombre,
                Creditos = resp.Data.Creditos,
                IntensidadHorariaSemanal = resp.Data.IntensidadHorariaSemanal,
                Activa = true
            };

            Semestre.AgregarMateria(nuevaMateria);
            MarcarPlanComoModificado();
            MateriaSeleccionada = null;
            await CargarMateriasDisponiblesAsync();
            NotificarEncabezado();
        }

        public async Task QuitarMateriaAsync(int idMateria)
        {
            if (Semestre == null) return;

            var materia = Semestre.Materias.FirstOrDefault(m => m.IdMateria == idMateria);
            if (materia == null) return;

            var resp = await _api.EliminarMateriaAsync(materia.IdMateriaPlan);
            if (!resp.Success) return;

            Semestre.QuitarMateria(idMateria);
            MarcarPlanComoModificado();
            await CargarMateriasDisponiblesAsync();
            NotificarEncabezado();
        }

        public async Task CargarMateriasDisponiblesAsync()
        {
            MateriasDisponibles.Clear();

            var resp = await _materiasApi.ObtenerMateriasAsync();
            if (!resp.Success || resp.Data == null) return;

            var idsAsignados = Semestre?.Materias
                .Select(m => m.IdMateria)
                .ToHashSet() ?? new System.Collections.Generic.HashSet<int>();

            foreach (var m in resp.Data.Where(m => m.Activa && !idsAsignados.Contains(m.IdMateria)))
                MateriasDisponibles.Add(m);

            OnPropertyChanged(nameof(MateriasDisponibles));
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

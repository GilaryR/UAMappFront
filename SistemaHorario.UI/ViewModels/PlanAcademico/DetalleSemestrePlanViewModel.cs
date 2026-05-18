using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.ViewModels.PlanAcademico
{
    /// <summary>
    /// ViewModel del detalle de semestre.
    /// Administra materias asignadas, materias disponibles y modo edición.
    /// </summary>
    public class DetalleSemestrePlanViewModel : INotifyPropertyChanged
    {
        private PlanAcademicoItem? _plan;
        private SemestrePlanItem? _semestre;
        private MateriaItem? _materiaSeleccionada;
        private bool _estaEditando;

        /// <summary>
        /// Plan académico dueño del semestre.
        /// </summary>
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

        /// <summary>
        /// Semestre actualmente consultado o editado.
        /// </summary>
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

        /// <summary>
        /// Materias disponibles para agregar.
        /// Para API real, esta colección debe venir desde endpoint de materias.
        /// </summary>
        public ObservableCollection<MateriaItem> MateriasDisponibles { get; } = new();

        /// <summary>
        /// Materia seleccionada en el ComboBox de agregar.
        /// </summary>
        public MateriaItem? MateriaSeleccionada
        {
            get => _materiaSeleccionada;
            set
            {
                _materiaSeleccionada = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Indica si están activos los controles de edición.
        /// </summary>
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

        /// <summary>
        /// Texto auxiliar del botón de edición.
        /// </summary>
        public string TextoBotonEditar =>
            EstaEditando ? "Editando" : "Editar";

        /// <summary>
        /// Título del semestre mostrado en la cabecera.
        /// </summary>
        public string TituloSemestre =>
            Semestre == null ? "Semestre" : $"Semestre {Semestre.NumeroSemestre}";

        /// <summary>
        /// Resumen de materias y créditos del semestre.
        /// </summary>
        public string ResumenSemestre =>
            Semestre == null
                ? "0 materias · 0 créditos"
                : $"{Semestre.TotalMaterias} materias · {Semestre.TotalCreditos} créditos";

        /// <summary>
        /// Jornada del semestre visible en la cabecera.
        /// </summary>
        public string JornadaSemestre =>
            string.IsNullOrWhiteSpace(Semestre?.Jornada)
                ? "Jornada: Por definir"
                : $"Jornada: {Semestre.Jornada}";

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Carga el semestre seleccionado dentro del plan académico.
        /// Para API real, se podría cargar el detalle por id de plan y número de semestre.
        /// </summary>
        public void Cargar(
            int idPlanAcademico,
            int numeroSemestre,
            bool modoEdicion)
        {
            Plan = PlanAcademicoMockStore.ObtenerPlanPorId(idPlanAcademico);

            Semestre = Plan?.Semestres
                .FirstOrDefault(item => item.NumeroSemestre == numeroSemestre);

            EstaEditando = modoEdicion;

            CargarMateriasDisponibles();
            NotificarEncabezado();
        }

        /// <summary>
        /// Activa el modo edición del semestre.
        /// </summary>
        public void ActivarEdicion()
        {
            EstaEditando = true;
        }

        /// <summary>
        /// Agrega al semestre la materia seleccionada desde la lista.
        /// </summary>
        public void AgregarMateriaSeleccionada()
        {
            if (Semestre == null || MateriaSeleccionada == null)
                return;

            Semestre.AgregarMateria(MateriaSeleccionada);
            MarcarPlanComoModificado();

            MateriaSeleccionada = null;

            CargarMateriasDisponibles();
            NotificarEncabezado();
        }

        /// <summary>
        /// Quita una materia del semestre actual.
        /// </summary>
        public void QuitarMateria(int idMateria)
        {
            if (Semestre == null)
                return;

            Semestre.QuitarMateria(idMateria);
            MarcarPlanComoModificado();

            CargarMateriasDisponibles();
            NotificarEncabezado();
        }

        /// <summary>
        /// Carga las materias que aún no están asignadas al semestre.
        /// </summary>
        public void CargarMateriasDisponibles()
        {
            MateriasDisponibles.Clear();

            List<int> idsAsignados = Semestre?.Materias
                .Select(materia => materia.IdMateria)
                .ToList() ?? new List<int>();

            IEnumerable<MateriaItem> disponibles = PlanAcademicoMockStore
                .ObtenerMateriasDisponibles()
                .Where(materia => !idsAsignados.Contains(materia.IdMateria));

            foreach (MateriaItem materia in disponibles)
            {
                MateriasDisponibles.Add(materia);
            }

            OnPropertyChanged(nameof(MateriasDisponibles));
        }

        /// <summary>
        /// Recalcula el resumen visual del semestre.
        /// </summary>
        public void RefrescarResumen()
        {
            NotificarEncabezado();
        }

        private void MarcarPlanComoModificado()
        {
            if (Plan == null)
                return;

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
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }
    }
}
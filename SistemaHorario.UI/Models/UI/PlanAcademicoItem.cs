using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Representa un plan académico dentro de la interfaz.
    ///
    /// Un plan académico puede ser, por ejemplo:
    /// - Plan Diurno.
    /// - Plan Nocturno.
    /// - Plan Tapsi.
    /// - Un nuevo plan creado por el usuario.
    ///
    /// Este modelo está preparado para recibir datos desde la base de datos
    /// por medio de endpoints.
    /// </summary>
    public class PlanAcademicoItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Nombre interno del plan académico.
        /// </summary>
        private string _nombre = string.Empty;

        /// <summary>
        /// Jornada interna del plan académico.
        /// </summary>
        private string _jornada = string.Empty;

        /// <summary>
        /// Estado interno del plan académico.
        /// </summary>
        private string _estado = "Activo";

        /// <summary>
        /// Texto interno de la carga académica por semestre.
        /// </summary>
        private string _cargaPorSemestre = string.Empty;

        /// <summary>
        /// Indica internamente si el plan tiene cambios pendientes por guardar.
        /// </summary>
        private bool _tieneCambiosPendientes;

        /// <summary>
        /// Identificador del plan académico.
        /// </summary>
        public int IdPlanAcademico { get; set; }

        /// <summary>
        /// Nombre visible del plan académico.
        ///
        /// Ejemplo: Plan Diurno, Plan Nocturno o Plan Tapsi.
        /// </summary>
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Jornada del plan académico.
        ///
        /// Ejemplo: Diurna, Nocturna o Diurna / Nocturna.
        /// </summary>
        public string Jornada
        {
            get => _jornada;
            set
            {
                _jornada = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Estado del plan académico.
        ///
        /// Permite ocultar planes inactivos sin eliminarlos físicamente
        /// de la base de datos.
        /// </summary>
        public string Estado
        {
            get => _estado;
            set
            {
                _estado = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Describe cómo se distribuye la carga académica del plan.
        ///
        /// Ejemplo:
        /// - Mayor por semestre.
        /// - Menor por semestre.
        /// - Por definir.
        /// </summary>
        public string CargaPorSemestre
        {
            get => _cargaPorSemestre;
            set
            {
                _cargaPorSemestre = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Cantidad total de semestres del plan.
        /// </summary>
        public int TotalSemestres { get; set; }

        /// <summary>
        /// Cantidad total de materias del plan.
        /// </summary>
        public int TotalMaterias { get; set; }

        /// <summary>
        /// Cantidad total de créditos del plan.
        /// </summary>
        public int TotalCreditos { get; set; }

        /// <summary>
        /// Indica si el plan fue creado desde la interfaz durante esta sesión.
        ///
        /// Se usa para mostrar una flecha en la pantalla principal y diferenciar
        /// visualmente los planes nuevos de los planes ya existentes.
        /// </summary>
        public bool EsNuevo { get; set; }

        /// <summary>
        /// Indica si el plan tiene cambios sin guardar.
        ///
        /// Se activa cuando el usuario agrega o quita materias de un semestre.
        /// </summary>
        public bool TieneCambiosPendientes
        {
            get => _tieneCambiosPendientes;
            set
            {
                _tieneCambiosPendientes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Lista de semestres que pertenecen al plan académico.
        ///
        /// Cada semestre contiene sus propias materias.
        /// </summary>
        public ObservableCollection<SemestrePlanItem> Semestres { get; set; } = new();

        /// <summary>
        /// Evento usado por WPF para refrescar la interfaz cuando cambia un dato.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Recalcula los totales del plan usando la información de sus semestres.
        ///
        /// Se usa cuando el usuario agrega o elimina materias.
        /// Así la interfaz puede mostrar datos actualizados como:
        /// - Total de semestres.
        /// - Total de materias.
        /// - Total de créditos.
        /// </summary>
        public void RecalcularTotalesDesdeSemestres()
        {
            TotalSemestres = Semestres.Count;
            TotalMaterias = Semestres.Sum(semestre => semestre.TotalMaterias);
            TotalCreditos = Semestres.Sum(semestre => semestre.TotalCreditos);

            OnPropertyChanged(nameof(TotalSemestres));
            OnPropertyChanged(nameof(TotalMaterias));
            OnPropertyChanged(nameof(TotalCreditos));
            OnPropertyChanged(nameof(Semestres));
        }

        /// <summary>
        /// Notifica a WPF que una propiedad cambió para actualizar la pantalla.
        /// </summary>
        protected void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }
    }
}
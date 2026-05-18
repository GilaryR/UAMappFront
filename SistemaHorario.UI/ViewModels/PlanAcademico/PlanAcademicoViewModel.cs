using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Base;
using System.Collections.ObjectModel;

namespace SistemaHorario.UI.ViewModels.PlanAcademico
{
    /// <summary>
    /// ViewModel principal del módulo Plan Académico.
    /// Controla la pantalla donde se listan planes, se muestra la comparación
    /// general y se inicia la creación de un nuevo plan.
    /// </summary>
    public class PlanAcademicoViewModel : ViewModelBase
    {
        private ObservableCollection<PlanAcademicoItem> _planes = new();
        private int _cantidadSemestresNuevoPlan = 10;
        private string _jornadaNuevoPlan = "Por definir";

        /// <summary>
        /// Planes académicos visibles en la pantalla principal.
        /// Luego debe llenarse con el endpoint de consulta de planes.
        /// </summary>
        public ObservableCollection<PlanAcademicoItem> Planes
        {
            get => _planes;
            set
            {
                _planes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Cantidad de semestres elegida para crear un nuevo plan académico.
        /// </summary>
        public int CantidadSemestresNuevoPlan
        {
            get => _cantidadSemestresNuevoPlan;
            set
            {
                _cantidadSemestresNuevoPlan = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Jornada elegida para crear un nuevo plan académico.
        /// </summary>
        public string JornadaNuevoPlan
        {
            get => _jornadaNuevoPlan;
            set
            {
                _jornadaNuevoPlan = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Nota fija solicitada para explicar la diferencia entre plan diurno y nocturno.
        /// </summary>
        public string NotaComparativa =>
            "Ambos planes contienen las mismas materias y créditos. La diferencia principal está en la distribución por semestre: el plan diurno se organiza en 10 semestres y el plan nocturno en 12 semestres.";

        /// <summary>
        /// Carga los planes desde la fuente temporal.
        /// Para conexión real, reemplazar esta llamada por un servicio de infraestructura.
        /// </summary>
        public void CargarPlanes()
        {
            Planes = PlanAcademicoMockStore.ObtenerPlanesPrincipales();
        }

        /// <summary>
        /// Crea un nuevo plan académico temporal con los datos seleccionados.
        /// Para conexión real, aquí se llamaría el endpoint POST de creación.
        /// </summary>
        public PlanAcademicoItem CrearNuevoPlan()
        {
            PlanAcademicoItem nuevoPlan =
                PlanAcademicoMockStore.CrearNuevoPlan(
                    CantidadSemestresNuevoPlan,
                    JornadaNuevoPlan
                );

            OnPropertyChanged(nameof(Planes));

            return nuevoPlan;
        }

        /// <summary>
        /// Crea un nuevo plan académico recibiendo semestres y jornada desde el diálogo.
        /// </summary>
        public PlanAcademicoItem CrearNuevoPlan(
            int cantidadSemestres,
            string jornada)
        {
            CantidadSemestresNuevoPlan = cantidadSemestres;
            JornadaNuevoPlan = jornada;

            return CrearNuevoPlan();
        }
    }
}
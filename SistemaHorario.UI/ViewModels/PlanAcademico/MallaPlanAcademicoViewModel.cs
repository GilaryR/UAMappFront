using SistemaHorario.UI.Models.UI;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.ViewModels.PlanAcademico
{
    /// <summary>
    /// ViewModel de la malla académica.
    /// Controla la visualización de semestres y el guardado general del plan.
    /// </summary>
    public class MallaPlanAcademicoViewModel : INotifyPropertyChanged
    {
        private PlanAcademicoItem? _plan;
        private bool _esModoCreacion;

        /// <summary>
        /// Plan académico actualmente seleccionado.
        /// Luego debe cargarse desde endpoint por id.
        /// </summary>
        public PlanAcademicoItem? Plan
        {
            get => _plan;
            set
            {
                _plan = value;
                OnPropertyChanged();
                RefrescarPropiedadesCalculadas();
            }
        }

        /// <summary>
        /// Indica si la malla corresponde a creación de un nuevo plan.
        /// </summary>
        public bool EsModoCreacion
        {
            get => _esModoCreacion;
            set
            {
                _esModoCreacion = value;
                OnPropertyChanged();
                RefrescarPropiedadesCalculadas();
            }
        }

        /// <summary>
        /// Título principal de la malla.
        /// </summary>
        public string Titulo
        {
            get
            {
                if (Plan == null)
                    return "Plan Académico";

                if (EsModoCreacion)
                    return $"Plan Académico Jornada {Plan.Jornada}";

                return $"{Plan.Nombre} - Jornada {Plan.Jornada}";
            }
        }

        /// <summary>
        /// Determina si se muestra el botón de guardar cambios del plan completo.
        /// </summary>
        public bool MostrarBotonGuardar =>
            Plan?.TieneCambiosPendientes == true || EsModoCreacion;

        /// <summary>
        /// Ícono de acción de cada semestre.
        /// En creación se agrega materia; en consulta se ve detalle.
        /// </summary>
        public string IconoAccionSemestre =>
            EsModoCreacion ? "+" : "👁";

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Carga el plan académico seleccionado.
        /// Para API real, reemplazar por GET /planes-academicos/{id}.
        /// </summary>
        public void CargarPlan(int idPlanAcademico, bool modoCreacion)
        {
            Plan = PlanAcademicoMockStore.ObtenerPlanPorId(idPlanAcademico);
            EsModoCreacion = modoCreacion;

            Refrescar();
        }

        /// <summary>
        /// Marca el plan como modificado para habilitar guardado.
        /// </summary>
        public void MarcarCambiosPendientes()
        {
            if (Plan == null)
                return;

            Plan.TieneCambiosPendientes = true;
            Refrescar();
        }

        /// <summary>
        /// Guarda los cambios temporales del plan completo.
        /// Para API real, reemplazar por POST/PUT según corresponda.
        /// </summary>
        public void GuardarCambios()
        {
            if (Plan == null)
                return;

            PlanAcademicoMockStore.GuardarCambiosPlan(Plan);
            Refrescar();
        }

        /// <summary>
        /// Recalcula totales y actualiza la interfaz.
        /// </summary>
        public void Refrescar()
        {
            Plan?.RecalcularTotalesDesdeSemestres();

            OnPropertyChanged(nameof(Plan));
            RefrescarPropiedadesCalculadas();
        }

        private void RefrescarPropiedadesCalculadas()
        {
            OnPropertyChanged(nameof(Titulo));
            OnPropertyChanged(nameof(MostrarBotonGuardar));
            OnPropertyChanged(nameof(IconoAccionSemestre));
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
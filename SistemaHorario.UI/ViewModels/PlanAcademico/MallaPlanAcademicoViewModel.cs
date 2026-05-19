using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.PlanAcademico
{
    public class MallaPlanAcademicoViewModel : INotifyPropertyChanged
    {
        private readonly PlanAcademicoApiService _api = new();
        private PlanAcademicoItem? _plan;
        private bool _esModoCreacion;

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

        public string Titulo
        {
            get
            {
                if (Plan == null) return "Plan Académico";
                if (EsModoCreacion) return $"Plan Académico Jornada {Plan.Jornada}";
                return $"{Plan.Nombre} - Jornada {Plan.Jornada}";
            }
        }

        public bool MostrarBotonGuardar =>
            Plan?.TieneCambiosPendientes == true || EsModoCreacion;

        public string IconoAccionSemestre =>
            EsModoCreacion ? "+" : "👁";

        public event PropertyChangedEventHandler? PropertyChanged;

        public async Task CargarPlan(int idPlanAcademico, bool modoCreacion)
        {
            EsModoCreacion = modoCreacion;
            var resp = await _api.ObtenerPlanPorIdAsync(idPlanAcademico);
            if (resp.Success && resp.Data != null)
                Plan = resp.Data;
            Refrescar();
        }

        public void MarcarCambiosPendientes()
        {
            if (Plan == null) return;
            Plan.TieneCambiosPendientes = true;
            Refrescar();
        }

        public void GuardarCambios()
        {
            if (Plan == null) return;
            Plan.TieneCambiosPendientes = false;
            Refrescar();
        }

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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

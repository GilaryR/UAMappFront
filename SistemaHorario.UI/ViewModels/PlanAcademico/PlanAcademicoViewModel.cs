using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.PlanAcademico
{
    public class PlanAcademicoViewModel : ViewModelBase
    {
        private readonly PlanAcademicoApiService _api = new();
        private ObservableCollection<PlanAcademicoItem> _planes = new();
        private int _cantidadSemestresNuevoPlan = 10;
        private string _jornadaNuevoPlan = "Por definir";

        public ObservableCollection<PlanAcademicoItem> Planes
        {
            get => _planes;
            set { _planes = value; OnPropertyChanged(); }
        }

        public int CantidadSemestresNuevoPlan
        {
            get => _cantidadSemestresNuevoPlan;
            set { _cantidadSemestresNuevoPlan = value; OnPropertyChanged(); }
        }

        public string JornadaNuevoPlan
        {
            get => _jornadaNuevoPlan;
            set { _jornadaNuevoPlan = value; OnPropertyChanged(); }
        }

        public string MensajeEstado { get; private set; } = string.Empty;

        public string NotaComparativa =>
            "Ambos planes contienen las mismas materias y creditos. La diferencia principal esta en la distribucion por semestre.";

        public PlanAcademicoViewModel() { _ = CargarPlanesAsync(); }

        public async Task CargarPlanesAsync()
        {
            var resp = await _api.ObtenerPlanesAsync();
            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = "Error: " + resp.Message;
                return;
            }
            Planes = new ObservableCollection<PlanAcademicoItem>(resp.Data);
        }

        public void CargarPlanes() => _ = CargarPlanesAsync();

        public async Task<PlanAcademicoItem?> CrearNuevoPlanAsync(int cantidadSemestres, string jornada)
        {
            var nuevo = new PlanAcademicoItem
            {
                Nombre = "Plan " + jornada + " " + System.DateTime.Now.Year,
                Jornada = jornada,
                CargaPorSemestre = "Por definir",
                TotalSemestres = cantidadSemestres,
                EsNuevo = true
            };

            var resp = await _api.CrearPlanAsync(nuevo);
            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = "Error: " + resp.Message;
                return null;
            }

            nuevo.IdPlanAcademico = resp.Data.IdPlanAcademico;

            for (int i = 1; i <= cantidadSemestres; i++)
            {
                var sResp = await _api.AgregarSemestreAsync(nuevo.IdPlanAcademico, i);
                if (!sResp.Success)
                {
                    MensajeEstado = "Error al crear semestre " + i + ": " + sResp.Message;
                    return null;
                }
            }

            await CargarPlanesAsync();
            return nuevo;
        }
    }
}

using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Base;
using SistemaHorarios.Application.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.PlanAcademico
{
    public class PlanAcademicoViewModel : ViewModelBase
    {
        private readonly PlanAcademicoApiService _api = new();

        private ObservableCollection<PlanAcademicoItem> _planes = new();
        private int _cantidadSemestresNuevoPlan = 10;
        private string _jornadaNuevoPlan = "Diurna";
        private string _mensajeEstado = string.Empty;

        public ObservableCollection<PlanAcademicoItem> Planes
        {
            get => _planes;
            private set
            {
                _planes = value;
                OnPropertyChanged();
            }
        }

        public int CantidadSemestresNuevoPlan
        {
            get => _cantidadSemestresNuevoPlan;
            set
            {
                _cantidadSemestresNuevoPlan = value;
                OnPropertyChanged();
            }
        }

        public string JornadaNuevoPlan
        {
            get => _jornadaNuevoPlan;
            set
            {
                _jornadaNuevoPlan = value;
                OnPropertyChanged();
            }
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            private set
            {
                _mensajeEstado = value;
                OnPropertyChanged();
            }
        }

        public string NotaComparativa =>
            "Los planes académicos conservan la misma base curricular. La diferencia principal está en la distribución por semestre y jornada.";

        public async Task CargarPlanesAsync()
        {
            ApiResponse<List<PlanAcademicoItem>> resp =
                await _api.ObtenerPlanesAsync();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar los planes académicos."
                    : resp.Message;

                Planes = new ObservableCollection<PlanAcademicoItem>();
                return;
            }

            Planes = new ObservableCollection<PlanAcademicoItem>(resp.Data);
            MensajeEstado = string.Empty;
        }

        public async Task<PlanAcademicoItem?> CrearNuevoPlanAsync(
            int cantidadSemestres,
            string jornada)
        {
            if (cantidadSemestres <= 0)
            {
                MensajeEstado = "La cantidad de semestres debe ser mayor que cero.";
                return null;
            }

            if (string.IsNullOrWhiteSpace(jornada))
            {
                MensajeEstado = "Debe seleccionar una jornada.";
                return null;
            }

            PlanAcademicoItem nuevo = new()
            {
                Nombre = $"Plan {jornada} {DateTime.Now.Year}",
                Jornada = jornada,
                CargaPorSemestre = "Por definir",
                TotalSemestres = cantidadSemestres,
                EsNuevo = true
            };

            ApiResponse<PlanAcademicoBackendDto> resp =
                await _api.CrearPlanAsync(nuevo);

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo crear el plan académico."
                    : resp.Message;

                return null;
            }

            nuevo.IdPlanAcademico = resp.Data.IdPlanAcademico;

            bool semestresCreados =
                await CrearSemestresDelPlanAsync(
                    nuevo.IdPlanAcademico,
                    cantidadSemestres
                );

            if (!semestresCreados)
            {
                return null;
            }

            await CargarPlanesAsync();

            MensajeEstado = string.Empty;
            return nuevo;
        }

        private async Task<bool> CrearSemestresDelPlanAsync(
            int idPlanAcademico,
            int cantidadSemestres)
        {
            for (int numeroSemestre = 1;
                 numeroSemestre <= cantidadSemestres;
                 numeroSemestre++)
            {
                ApiResponse<string> semestreResp =
                    await _api.AgregarSemestreAsync(
                        idPlanAcademico,
                        numeroSemestre
                    );

                if (!semestreResp.Success)
                {
                    MensajeEstado =
                        $"No se pudo crear el semestre {numeroSemestre}: " +
                        semestreResp.Message;

                    return false;
                }
            }

            return true;
        }

        public async Task<bool> EliminarPlanAsync(
            PlanAcademicoItem plan)
        {
            ApiResponse<string> resp =
                await _api.EliminarPlanAsync(plan.IdPlanAcademico);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo eliminar el plan académico."
                    : resp.Message;

                return false;
            }

            await CargarPlanesAsync();

            MensajeEstado = string.Empty;
            return true;
        }
    }
}
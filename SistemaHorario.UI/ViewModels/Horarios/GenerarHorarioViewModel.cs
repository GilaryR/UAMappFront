using SistemaHorario.UI.Models.UI;
using System.Collections.ObjectModel;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    /// <summary>
    /// ViewModel encargado de preparar la información visual
    /// necesaria para generar horarios.
    ///
    /// Actualmente usa datos mock.
    ///
    /// Endpoints relacionados:
    /// - GET /api/grupos/activos
    /// - POST /api/horarios/generar
    ///
    /// Nota:
    /// El combo de grupo se conserva en UI porque funcionalmente
    /// el sistema debe generar horarios por grupo académico.
    ///
    /// Sin embargo, el contrato actual del backend para
    /// POST /api/horarios/generar todavía no recibe IdGrupo.
    /// </summary>
    public class GenerarHorarioViewModel
    {
        /// <summary>
        /// Lista visual de grupos académicos.
        ///
        /// TODO:
        /// Reemplazar por GET /api/grupos/activos.
        /// </summary>
        public ObservableCollection<GrupoHorarioOption> Grupos { get; set; }

        /// <summary>
        /// Request preparado según contrato actual del backend.
        /// </summary>
        public GenerarHorarioRequestUI Request { get; set; }

        /// <summary>
        /// Constructor principal.
        /// </summary>
        public GenerarHorarioViewModel()
        {
            Request = new GenerarHorarioRequestUI();

            Grupos =
            [
                new GrupoHorarioOption
                {
                    IdGrupo = 1,
                    NombreGrupo = "Grupo 1",
                    Semestre = 1,
                    Jornada = "Diurna"
                },

                new GrupoHorarioOption
                {
                    IdGrupo = 2,
                    NombreGrupo = "Grupo 2",
                    Semestre = 3,
                    Jornada = "Nocturna"
                },

                new GrupoHorarioOption
                {
                    IdGrupo = 3,
                    NombreGrupo = "Grupo TAPSI",
                    Semestre = 4,
                    Jornada = "Diurna"
                }
            ];
        }
    }
}
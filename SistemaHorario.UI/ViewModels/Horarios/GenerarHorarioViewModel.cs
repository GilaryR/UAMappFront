using SistemaHorario.UI.Models.UI;
using System.Collections.ObjectModel;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    /// <summary>
    /// ViewModel encargado de la lógica visual
    /// para la generación de horarios académicos.
    ///
    /// Actualmente utiliza datos mock temporales.
    ///
    /// Más adelante deberá conectarse con:
    ///
    /// GET  /api/grupos/activos
    /// POST /api/horarios/generar
    /// </summary>
    public class GenerarHorarioViewModel
    {
        /// <summary>
        /// Lista visual de grupos disponibles.
        /// </summary>
        public ObservableCollection<GrupoHorarioOption> Grupos { get; set; }

        /// <summary>
        /// Request temporal utilizado para generar horarios.
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
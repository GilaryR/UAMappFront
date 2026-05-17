using SistemaHorario.UI.Models.UI;
using System.Collections.ObjectModel;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    /// <summary>
    /// ViewModel de la vista previa de horarios.
    ///
    /// Maneja la información visual del horario semanal.
    ///
    /// Actualmente usa datos temporales.
    ///
    /// Más adelante deberá consumir:
    /// GET /api/horarios/{id}/vista-previa
    /// POST /api/horarios/{id}/aprobar
    /// POST /api/horarios/{id}/rechazar
    /// PUT /api/horarios/{id}/asignatura
    /// </summary>
    public class VistaPreviaHorarioViewModel
    {
        public HorarioItem Horario { get; set; }

        public bool ModoEdicion { get; set; }

        public ObservableCollection<BloqueHorarioItem> Bloques { get; set; }

        public VistaPreviaHorarioViewModel(
            HorarioItem horario,
            bool modoEdicion)
        {
            Horario = horario;
            ModoEdicion = modoEdicion;
            Bloques = new ObservableCollection<BloqueHorarioItem>();

            CargarDatosTemporales();
        }

        /// <summary>
        /// Datos temporales para validar diseño.
        ///
        /// TODO:
        /// Reemplazar por GET /api/horarios/{id}/vista-previa.
        /// </summary>
        private void CargarDatosTemporales()
        {
            Bloques.Add(new BloqueHorarioItem
            {
                IdHorario = Horario.IdHorario,
                Dia = "Lunes",
                HoraInicio = "7:00",
                HoraFinal = "9:00",
                Materia = "C. Diferencial",
                Docente = "Jairo",
                Aula = "F404",
                Modalidad = "Presencial",
                ColorVisual = "#20A848"
            });

            Bloques.Add(new BloqueHorarioItem
            {
                IdHorario = Horario.IdHorario,
                Dia = "Miércoles",
                HoraInicio = "7:00",
                HoraFinal = "9:00",
                Materia = "POO",
                Docente = "Sebastián",
                Aula = "16-102",
                Modalidad = "Presencial",
                ColorVisual = "#C77EE8"
            });

            Bloques.Add(new BloqueHorarioItem
            {
                IdHorario = Horario.IdHorario,
                Dia = "Martes",
                HoraInicio = "10:00",
                HoraFinal = "12:00",
                Materia = "Inglés II",
                Docente = "Yairo",
                Aula = "F204",
                Modalidad = "Presencial",
                ColorVisual = "#0FB8C8"
            });

            Bloques.Add(new BloqueHorarioItem
            {
                IdHorario = Horario.IdHorario,
                Dia = "Jueves",
                HoraInicio = "9:00",
                HoraFinal = "11:00",
                Materia = "Filosofía",
                Docente = "Mercury",
                Aula = "Virtual",
                Modalidad = "Híbrido",
                ColorVisual = "#F3D98B"
            });

            Bloques.Add(new BloqueHorarioItem
            {
                IdHorario = Horario.IdHorario,
                Dia = "Sábado",
                HoraInicio = "7:00",
                HoraFinal = "10:00",
                Materia = "Bases I",
                Docente = "Pedro",
                Aula = "Virtual",
                Modalidad = "Híbrido",
                ColorVisual = "#F58B8B"
            });
        }
    }
}
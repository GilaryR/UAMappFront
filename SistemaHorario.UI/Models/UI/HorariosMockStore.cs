using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Almacén temporal de horarios mientras se conecta la API.
    ///
    /// TODO:
    /// Reemplazar esta clase por consumo real de:
    /// GET /api/horarios
    /// POST /api/horarios/generar
    /// POST /api/horarios/{id}/aprobar
    /// POST /api/horarios/{id}/rechazar
    /// </summary>
    public static class HorariosMockStore
    {
        private static readonly List<HorarioItem> _horarios = new();

        private static int _siguienteId = 100;

        public static List<HorarioItem> ObtenerHorarios()
        {
            InicializarSiEstaVacio();

            return _horarios.ToList();
        }

        public static void GuardarHorarioAprobado(HorarioItem horario)
        {
            horario.Estado = "Aprobado";

            HorarioItem? existente = _horarios
                .FirstOrDefault(h => h.IdHorario == horario.IdHorario);

            if (existente != null)
            {
                existente.Estado = "Aprobado";
                return;
            }

            if (horario.IdHorario == 0)
            {
                horario.IdHorario = _siguienteId;
                _siguienteId++;
            }

            _horarios.Insert(0, horario);
        }

        public static void EliminarHorario(HorarioItem horario)
        {
            HorarioItem? existente = _horarios
                .FirstOrDefault(h => h.IdHorario == horario.IdHorario);

            if (existente != null)
                _horarios.Remove(existente);
        }

        private static void InicializarSiEstaVacio()
        {
            if (_horarios.Count > 0)
                return;

            _horarios.Add(new HorarioItem
            {
                IdHorario = 1,
                Nombre = "Horario_G1_2026-01",
                Grupo = "Grupo 1",
                Tipo = "Regular",
                Jornada = "Diurna",
                FechaGeneracion = "27/04/2026",
                Estado = "Pendiente"
            });
        }
    }
}
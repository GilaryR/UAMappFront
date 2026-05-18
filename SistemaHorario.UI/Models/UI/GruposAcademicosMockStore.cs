using System.Collections.ObjectModel;
using System.Linq;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Datos temporales para trabajar la pantalla de grupos académicos.
    /// 
    /// Este archivo se usa mientras todavía no está conectado el backend.
    /// Después, estos datos se deben reemplazar por llamadas a endpoints.
    /// </summary>
    public static class GruposAcademicosMockStore
    {
        private static readonly ObservableCollection<GrupoAcademicoItem> _grupos = new();

        public static ObservableCollection<GrupoAcademicoItem> ObtenerGrupos()
        {
            if (_grupos.Count == 0)
            {
                _grupos.Add(new GrupoAcademicoItem
                {
                    IdGrupoAcademico = 1,
                    NombreGrupo = "1A-2026-2",
                    Codigo = "12885",
                    Jornada = "Diurno",
                    Materia = "Técnicas de programación",
                    EstudiantesActuales = 30,
                    PlazasDisponibles = 45,
                    Tipo = "Mixto",
                    Estado = "Activa",
                    Dias = new ObservableCollection<string> { "Lunes", "Jueves" }
                });

                _grupos.Add(new GrupoAcademicoItem
                {
                    IdGrupoAcademico = 2,
                    NombreGrupo = "1B-2026-2",
                    Codigo = "12885",
                    Jornada = "Diurno",
                    Materia = "POO",
                    EstudiantesActuales = 20,
                    PlazasDisponibles = 28,
                    Tipo = "Regular",
                    Estado = "Activa",
                    Dias = new ObservableCollection<string> { "Martes", "Miércoles" }
                });

                _grupos.Add(new GrupoAcademicoItem
                {
                    IdGrupoAcademico = 3,
                    NombreGrupo = "2A-2026-2",
                    Codigo = "12885",
                    Jornada = "Nocturno",
                    Materia = "Física II",
                    EstudiantesActuales = 19,
                    PlazasDisponibles = 25,
                    Tipo = "TAPSI",
                    Estado = "Activa",
                    Dias = new ObservableCollection<string> { "Lunes", "Viernes" }
                });
            }

            return _grupos;
        }

        public static ObservableCollection<string> ObtenerMateriasDisponibles()
        {
            return new ObservableCollection<string>
            {
                "Técnicas de programación",
                "POO",
                "Física II",
                "Redes LAN",
                "Bases de datos I",
                "Ingeniería de software I",
                "Cálculo diferencial",
                "Matemáticas básicas"
            };
        }

        public static void AgregarGrupo(GrupoAcademicoItem grupo)
        {
            int nuevoId = _grupos.Count == 0
                ? 1
                : _grupos.Max(item => item.IdGrupoAcademico) + 1;

            grupo.IdGrupoAcademico = nuevoId;
            _grupos.Add(grupo);
        }

        public static void ActualizarGrupo(GrupoAcademicoItem grupoActualizado)
        {
            GrupoAcademicoItem? grupo = _grupos
                .FirstOrDefault(item => item.IdGrupoAcademico == grupoActualizado.IdGrupoAcademico);

            if (grupo == null)
                return;

            grupo.CopiarDesde(grupoActualizado);
        }

        public static void EliminarGrupo(int idGrupoAcademico)
        {
            GrupoAcademicoItem? grupo = _grupos
                .FirstOrDefault(item => item.IdGrupoAcademico == idGrupoAcademico);

            if (grupo == null)
                return;

            _grupos.Remove(grupo);
        }
    }
}
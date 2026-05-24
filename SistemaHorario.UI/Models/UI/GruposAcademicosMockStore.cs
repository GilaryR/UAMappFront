namespace SistemaHorario.UI.Models.UI;

// Proporciona datos de prueba para grupos académicos cuando se requieran mocks locales.
public static class GruposAcademicosMockStore
{
    public static List<GrupoAcademicoItem> ObtenerGrupos()
    {
        return new List<GrupoAcademicoItem>
        {
            new GrupoAcademicoItem
            {
                IdGrupoAcademico = 1,
                IdPlanAcademico = 1,
                NombreGrupo = "Grupo 3A",
                Codigo = "3A",
                Jornada = "Diurna",
                Tipo = "Regular",
                Estado = "Activo",
                NumeroSemestre = 3,
                EstudiantesActuales = 28,
                PlazasDisponibles = 28
            },
            new GrupoAcademicoItem
            {
                IdGrupoAcademico = 2,
                IdPlanAcademico = 1,
                NombreGrupo = "Grupo 3B",
                Codigo = "3B",
                Jornada = "Diurna",
                Tipo = "Regular",
                Estado = "Activo",
                NumeroSemestre = 3,
                EstudiantesActuales = 25,
                PlazasDisponibles = 25
            },
            new GrupoAcademicoItem
            {
                IdGrupoAcademico = 3,
                IdPlanAcademico = 1,
                NombreGrupo = "Grupo 4N",
                Codigo = "4N",
                Jornada = "Nocturna",
                Tipo = "Regular",
                Estado = "Activo",
                NumeroSemestre = 4,
                EstudiantesActuales = 22,
                PlazasDisponibles = 22
            }
        };
    }
}
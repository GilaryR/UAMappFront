using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Datos temporales para que la parte gráfica de Plan Académico funcione
    /// mientras se conecta el backend.
    ///
    /// Esta clase se debe reemplazar por llamadas reales a la API.
    ///
    /// La idea es dejar todos los datos de prueba en un solo lugar y no
    /// mezclarlos con las vistas.
    /// </summary>
    public static class PlanAcademicoMockStore
    {
        /// <summary>
        /// Lista temporal de planes académicos.
        /// Se mantiene en memoria mientras la aplicación está abierta.
        /// </summary>
        private static readonly ObservableCollection<PlanAcademicoItem> _planes = new();

        /// <summary>
        /// Devuelve los planes principales que se muestran en las tarjetas:
        /// Plan Diurno, Plan Nocturno y Plan Tapsi.
        ///
        /// Más adelante estos datos deben venir desde la base de datos.
        /// </summary>
        public static ObservableCollection<PlanAcademicoItem> ObtenerPlanesPrincipales()
        {
            if (_planes.Count == 0)
            {
                _planes.Add(CrearPlan(1, "Plan Diurno", "Diurna", 10, "Mayor por semestre"));
                _planes.Add(CrearPlan(2, "Plan Nocturno", "Nocturna", 12, "Menor por semestre"));
                _planes.Add(CrearPlan(3, "Plan Tapsi", "Diurna / Nocturna", 12, "Menor por semestre"));
            }

            return _planes;
        }

        /// <summary>
        /// Busca un plan por su id.
        ///
        /// Se usa cuando el usuario da clic en "Ver Malla" para abrir
        /// los semestres y materias del plan seleccionado.
        /// </summary>
        public static PlanAcademicoItem? ObtenerPlanPorId(int idPlanAcademico)
        {
            return ObtenerPlanesPrincipales()
                .FirstOrDefault(plan => plan.IdPlanAcademico == idPlanAcademico);
        }

        /// <summary>
        /// Crea un plan nuevo de forma temporal.
        ///
        /// El usuario escoge la cantidad de semestres y la jornada.
        /// El plan se crea vacío para que luego pueda agregar materias.
        ///
        /// Más adelante esto debe guardarse usando el backend.
        /// </summary>
        public static PlanAcademicoItem CrearNuevoPlan(int cantidadSemestres, string jornada)
        {
            int nuevoId = ObtenerPlanesPrincipales().Max(plan => plan.IdPlanAcademico) + 1;

            PlanAcademicoItem plan = new()
            {
                IdPlanAcademico = nuevoId,
                Nombre = $"Nuevo Plan {nuevoId}",
                Jornada = jornada,
                TotalSemestres = cantidadSemestres,
                TotalMaterias = 0,
                TotalCreditos = 0,
                CargaPorSemestre = "Por definir",
                EsNuevo = true,
                TieneCambiosPendientes = true,
                Semestres = CrearSemestresVacios(cantidadSemestres, jornada)
            };

            _planes.Add(plan);

            return plan;
        }

        /// <summary>
        /// Devuelve materias de prueba para poder agregarlas a los semestres.
        ///
        /// En la versión final estas materias deben venir desde la base de datos.
        /// </summary>
        public static ObservableCollection<MateriaItem> ObtenerMateriasDisponibles()
        {
            ObservableCollection<MateriaItem> materias = new();

            string[] nombres =
            [
                "Matemáticas básicas",
                "Técnicas de programación",
                "Introducción a la ingeniería",
                "Cálculo diferencial",
                "Álgebra lineal",
                "Programación orientada a objetos",
                "Física mecánica",
                "Cálculo integral",
                "Bases de datos I",
                "Arquitectura de computadores",
                "Ingeniería de software I",
                "Sistemas operativos",
                "Redes LAN",
                "Bases de datos II",
                "Análisis de datos",
                "Ingeniería de software II",
                "Gestión de proyectos",
                "Desarrollo web",
                "Modelado de software",
                "Seguridad informática",
                "Inteligencia artificial",
                "Analítica de datos"
            ];

            for (int i = 1; i <= 74; i++)
            {
                string nombre = i <= nombres.Length
                    ? nombres[i - 1]
                    : $"Materia profesional {i:00}";

                int creditos = i <= 27 ? 3 : 2;

                materias.Add(new MateriaItem
                {
                    IdMateria = i,
                    Codigo = $"MAT{i:000}",
                    Nombre = nombre,
                    Creditos = creditos,
                    IntensidadHorariaSemanal = creditos,
                    Semestre = 0,
                    CantidadGrupos = 1,
                    Activa = true
                });
            }

            return materias;
        }

        /// <summary>
        /// Simula el guardado de un plan académico.
        ///
        /// Recalcula los totales y marca el plan como guardado.
        /// En la versión final aquí se debe llamar al backend.
        /// </summary>
        public static void GuardarCambiosPlan(PlanAcademicoItem plan)
        {
            plan.RecalcularTotalesDesdeSemestres();
            plan.TieneCambiosPendientes = false;
            plan.EsNuevo = false;
        }

        /// <summary>
        /// Crea un plan de prueba con sus semestres y materias.
        ///
        /// Estos planes simulan los planes que ya existen en la base de datos.
        /// </summary>
        private static PlanAcademicoItem CrearPlan(
            int id,
            string nombre,
            string jornada,
            int cantidadSemestres,
            string carga)
        {
            PlanAcademicoItem plan = new()
            {
                IdPlanAcademico = id,
                Nombre = nombre,
                Jornada = jornada,
                TotalSemestres = cantidadSemestres,
                TotalMaterias = 74,
                TotalCreditos = 175,
                CargaPorSemestre = carga,
                EsNuevo = false,
                Semestres = CrearSemestresConMaterias(cantidadSemestres, jornada)
            };

            return plan;
        }

        /// <summary>
        /// Crea semestres vacíos.
        ///
        /// Se usa para planes nuevos, porque al inicio todavía no tienen materias.
        /// </summary>
        private static ObservableCollection<SemestrePlanItem> CrearSemestresVacios(
            int cantidadSemestres,
            string jornada)
        {
            ObservableCollection<SemestrePlanItem> semestres = new();

            for (int i = 1; i <= cantidadSemestres; i++)
            {
                semestres.Add(new SemestrePlanItem
                {
                    IdSemestre = i,
                    NumeroSemestre = i,
                    Jornada = jornada
                });
            }

            return semestres;
        }

        /// <summary>
        /// Crea semestres con materias de prueba.
        ///
        /// Esta distribución es temporal. En la versión final, las materias
        /// de cada semestre deben venir desde la base de datos, porque hacen
        /// parte de la malla oficial del plan.
        /// </summary>
        private static ObservableCollection<SemestrePlanItem> CrearSemestresConMaterias(
            int cantidadSemestres,
            string jornada)
        {
            ObservableCollection<SemestrePlanItem> semestres =
                CrearSemestresVacios(cantidadSemestres, jornada);

            List<MateriaItem> materias = ObtenerMateriasDisponibles().ToList();

            for (int i = 0; i < materias.Count; i++)
            {
                int indiceSemestre = i % cantidadSemestres;
                semestres[indiceSemestre].AgregarMateria(materias[i]);
            }

            return semestres;
        }
    }
}
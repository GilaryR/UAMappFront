using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorarios.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Reportes
{
    // Maneja la consulta, vista previa y exportación de reportes académicos.
    public class ReportesAcademicosViewModel
    {
        private readonly ReportesApiService _api = new();
        private List<ReporteAcademicoItem> _reportesBase = new();

        public List<ReporteAcademicoItem> Reportes { get; private set; } = new();
        public List<string> TiposReporte { get; private set; } = new();
        public List<string> Periodos { get; private set; } = new();
        public string MensajeEstado { get; private set; } = string.Empty;
        public string Busqueda { get; set; } = string.Empty;
        public string TipoSeleccionado { get; set; } = "Todos los tipos";
        public DateTime? FechaSeleccionada { get; set; }

        public async Task CargarCatalogosAsync()
        {
            MensajeEstado = string.Empty;

            var tiposResp = await _api.ObtenerTiposReporteAsync();
            TiposReporte = tiposResp.Success && tiposResp.Data != null
                ? tiposResp.Data
                : new List<string>();

            AsegurarTiposReporteBasicos();

            var periodosResp = await _api.ObtenerSemestresAsync();
            Periodos = periodosResp.Success && periodosResp.Data != null
                ? periodosResp.Data.Select(item => item.ToString()).ToList()
                : new List<string>();
        }


        private void AsegurarTiposReporteBasicos()
        {
            string[] tiposNecesarios =
            {
                "Horarios generados",
                "Horario por grupo",
                "Horario por docente",
                "Carga docente",
                "Conflictos de horario"
            };

            foreach (string tipo in tiposNecesarios)
            {
                if (!TiposReporte.Contains(tipo))
                {
                    TiposReporte.Add(tipo);
                }
            }
        }

        public async Task CargarReportesAsync()
        {
            MensajeEstado = string.Empty;
            var reportes = new List<ReporteAcademicoItem>();

            await AgregarReportesHorariosGeneradosAsync(reportes);
            await AgregarReportesCargaDocenteAsync(reportes);
            await AgregarReporteConflictosAsync(reportes);

            _reportesBase = reportes
                .OrderBy(reporte => reporte.TipoReporte)
                .ThenBy(reporte => reporte.Detalle)
                .ToList();

            Reportes = _reportesBase.ToList();

            if (Reportes.Count == 0)
            {
                MensajeEstado = "No se encontraron reportes disponibles. Primero genera horarios académicos.";
            }
        }

        private async Task AgregarReportesHorariosGeneradosAsync(
            List<ReporteAcademicoItem> reportes)
        {
            ApiResponse<List<ReporteHorarioGeneradoBackendDto>> resp =
                await _api.ObtenerHorariosGeneradosAsync();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar los horarios generados."
                    : resp.Message;

                return;
            }

            reportes.Add(new ReporteAcademicoItem
            {
                IdReporte = 1,
                TipoCodigo = "HorariosGenerados",
                TipoReporte = "Horarios generados",
                Detalle = "Resumen general",
                Periodo = "Todos",
                FormatoInicial = "CSV",
                Estado = resp.Data.Count == 0 ? "Sin datos" : "Disponible",
                Descripcion = $"Grupos con horario generado: {resp.Data.Count}."
            });

            int contador = 100;

            foreach (ReporteHorarioGeneradoBackendDto item in resp.Data)
            {
                reportes.Add(new ReporteAcademicoItem
                {
                    IdReporte = contador,
                    TipoCodigo = "HorarioGrupo",
                    TipoReporte = "Horario por grupo",
                    Detalle = $"{item.CodigoGrupo} - {item.NombreGrupo}",
                    Periodo = $"Semestre {item.NumeroSemestre}",
                    FormatoInicial = "PDF",
                    IdGrupo = item.IdGrupo,
                    Estado = item.Estado,
                    Descripcion =
                        $"{item.Jornada}. Materias: {item.TotalMaterias}. " +
                        $"Docentes: {item.TotalDocentes}. Bloques: {item.TotalBloques}. " +
                        $"Horas semanales: {item.TotalHoras}."
                });

                contador++;
            }
        }

        private async Task AgregarReportesCargaDocenteAsync(
            List<ReporteAcademicoItem> reportes)
        {
            ApiResponse<List<ReporteCargaDocenteBackendDto>> resp =
                await _api.ObtenerCargaDocenteAsync();

            if (!resp.Success || resp.Data == null)
            {
                return;
            }

            int horasTotales = resp.Data.Sum(item => item.HorasSemanales);

            reportes.Add(new ReporteAcademicoItem
            {
                IdReporte = 2,
                TipoCodigo = "CargaDocente",
                TipoReporte = "Carga docente",
                Detalle = "Resumen general",
                Periodo = "Todos",
                FormatoInicial = "CSV",
                Estado = "Disponible",
                Descripcion = $"Docentes consultados: {resp.Data.Count}. Horas semanales asignadas: {horasTotales}."
            });

            int contador = 300;

            foreach (ReporteCargaDocenteBackendDto item in resp.Data.Where(docente => docente.BloquesSemanales > 0))
            {
                reportes.Add(new ReporteAcademicoItem
                {
                    IdReporte = contador,
                    TipoCodigo = "HorarioDocente",
                    TipoReporte = "Horario por docente",
                    Detalle = item.Docente,
                    Periodo = "Todos",
                    FormatoInicial = "PDF",
                    IdDocente = item.IdDocente,
                    Estado = item.EstadoDocente,
                    Descripcion =
                        $"Materias: {item.CantidadMaterias}. Grupos: {item.CantidadGrupos}. " +
                        $"Bloques: {item.BloquesSemanales}. Horas semanales: {item.HorasSemanales}."
                });

                contador++;
            }
        }

        private async Task AgregarReporteConflictosAsync(
            List<ReporteAcademicoItem> reportes)
        {
            ApiResponse<List<ReporteConflictoHorarioBackendDto>> resp =
                await _api.ObtenerConflictosHorarioAsync();

            if (!resp.Success || resp.Data == null)
            {
                return;
            }

            reportes.Add(new ReporteAcademicoItem
            {
                IdReporte = 3,
                TipoCodigo = "ConflictosHorario",
                TipoReporte = "Conflictos de horario",
                Detalle = resp.Data.Count == 0 ? "Sin conflictos" : $"{resp.Data.Count} conflicto(s)",
                Periodo = "Todos",
                FormatoInicial = "CSV",
                Estado = resp.Data.Count == 0 ? "OK" : "Revisar",
                Descripcion = resp.Data.Count == 0
                    ? "No se encontraron conflictos activos en los horarios."
                    : "Existen inconsistencias que deben revisarse antes de aprobar horarios."
            });
        }

        public void AplicarFiltros()
        {
            IEnumerable<ReporteAcademicoItem> consulta = _reportesBase;

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                string busqueda = Busqueda.Trim().ToLower();

                consulta = consulta.Where(reporte =>
                    reporte.TipoReporte.ToLower().Contains(busqueda) ||
                    reporte.Detalle.ToLower().Contains(busqueda) ||
                    reporte.Periodo.ToLower().Contains(busqueda) ||
                    reporte.Descripcion.ToLower().Contains(busqueda) ||
                    reporte.Estado.ToLower().Contains(busqueda));
            }

            if (!string.IsNullOrWhiteSpace(TipoSeleccionado) &&
                TipoSeleccionado != "Todos los tipos")
            {
                consulta = consulta.Where(reporte => reporte.TipoReporte == TipoSeleccionado);
            }

            if (FechaSeleccionada.HasValue)
            {
                consulta = consulta.Where(reporte => reporte.Fecha.Date == FechaSeleccionada.Value.Date);
            }

            Reportes = consulta.ToList();
        }

        public void LimpiarFiltros()
        {
            Busqueda = string.Empty;
            TipoSeleccionado = "Todos los tipos";
            FechaSeleccionada = null;
            Reportes = _reportesBase.ToList();
        }

        public async Task<string> ObtenerVistaPreviaAsync(ReporteAcademicoItem reporte)
        {
            return reporte.TipoCodigo switch
            {
                "HorarioGrupo" => await ConstruirHorarioGrupoAsync(reporte),
                "HorarioDocente" => await ConstruirHorarioDocenteAsync(reporte),
                "CargaDocente" => await ConstruirCargaDocenteAsync(),
                "ConflictosHorario" => await ConstruirConflictosAsync(),
                _ => await ConstruirHorariosGeneradosAsync()
            };
        }

        public async Task<ApiResponse<string>> DescargarReporteAsync(
            ReporteAcademicoItem reporte,
            string formato,
            string rutaDestino)
        {
            return await _api.DescargarReporteAsync(
                reporte.TipoCodigo,
                formato,
                reporte.IdGrupo,
                reporte.IdDocente,
                rutaDestino);
        }

        private async Task<string> ConstruirHorariosGeneradosAsync()
        {
            var resp = await _api.ObtenerHorariosGeneradosAsync();

            if (!resp.Success || resp.Data == null)
            {
                return ObtenerMensajeError(resp.Message);
            }

            var builder = CrearEncabezado("HORARIOS GENERADOS");

            if (resp.Data.Count == 0)
            {
                builder.AppendLine("No hay horarios generados.");
                return builder.ToString();
            }

            foreach (ReporteHorarioGeneradoBackendDto item in resp.Data)
            {
                builder.AppendLine($"Grupo: {item.CodigoGrupo} - {item.NombreGrupo}");
                builder.AppendLine($"Jornada: {item.Jornada} | Semestre: {item.NumeroSemestre} | Estado: {item.Estado}");
                builder.AppendLine($"Materias: {item.TotalMaterias} | Docentes: {item.TotalDocentes} | Bloques: {item.TotalBloques} | Horas: {item.TotalHoras}");
                builder.AppendLine(new string('-', 90));
            }

            return builder.ToString();
        }

        private async Task<string> ConstruirHorarioGrupoAsync(ReporteAcademicoItem reporte)
        {
            if (!reporte.IdGrupo.HasValue)
            {
                return "Este reporte no tiene grupo asociado.";
            }

            var resp = await _api.ObtenerHorarioGrupoAsync(reporte.IdGrupo.Value);

            if (!resp.Success || resp.Data == null)
            {
                return ObtenerMensajeError(resp.Message);
            }

            var builder = CrearEncabezado($"HORARIO DEL GRUPO - {reporte.Detalle}");

            foreach (ReporteHorarioGrupoBackendDto item in resp.Data)
            {
                builder.AppendLine($"{item.DiaSemana,-10} {item.HoraInicio} - {item.HoraFin} | {item.CodigoMateria} - {item.Materia}");
                builder.AppendLine($"Docente: {item.Docente} | Estado: {item.Estado}");
                builder.AppendLine(new string('-', 90));
            }

            if (resp.Data.Count == 0)
            {
                builder.AppendLine("Este grupo todavía no tiene horario generado.");
            }

            return builder.ToString();
        }

        private async Task<string> ConstruirHorarioDocenteAsync(ReporteAcademicoItem reporte)
        {
            if (!reporte.IdDocente.HasValue)
            {
                return "Este reporte no tiene docente asociado.";
            }

            var resp = await _api.ObtenerHorarioDocenteAsync(reporte.IdDocente.Value);

            if (!resp.Success || resp.Data == null)
            {
                return ObtenerMensajeError(resp.Message);
            }

            var builder = CrearEncabezado($"HORARIO DOCENTE - {reporte.Detalle}");

            foreach (ReporteHorarioDocenteBackendDto item in resp.Data)
            {
                builder.AppendLine($"{item.DiaSemana,-10} {item.HoraInicio} - {item.HoraFin} | {item.Materia}");
                builder.AppendLine($"Grupo: {item.CodigoGrupo} | Jornada: {item.Jornada} | Semestre: {item.NumeroSemestre} | Estado: {item.Estado}");
                builder.AppendLine(new string('-', 90));
            }

            if (resp.Data.Count == 0)
            {
                builder.AppendLine("Este docente todavía no tiene horario asignado.");
            }

            return builder.ToString();
        }

        private async Task<string> ConstruirCargaDocenteAsync()
        {
            var resp = await _api.ObtenerCargaDocenteAsync();

            if (!resp.Success || resp.Data == null)
            {
                return ObtenerMensajeError(resp.Message);
            }

            var builder = CrearEncabezado("CARGA DOCENTE");

            foreach (ReporteCargaDocenteBackendDto item in resp.Data)
            {
                builder.AppendLine($"Docente: {item.Docente} | Estado: {item.EstadoDocente}");
                builder.AppendLine($"Materias: {item.CantidadMaterias} | Grupos: {item.CantidadGrupos} | Bloques: {item.BloquesSemanales} | Horas semanales: {item.HorasSemanales}");
                builder.AppendLine(new string('-', 90));
            }

            return builder.ToString();
        }

        private async Task<string> ConstruirConflictosAsync()
        {
            var resp = await _api.ObtenerConflictosHorarioAsync();

            if (!resp.Success || resp.Data == null)
            {
                return ObtenerMensajeError(resp.Message);
            }

            var builder = CrearEncabezado("CONFLICTOS DE HORARIO");

            if (resp.Data.Count == 0)
            {
                builder.AppendLine("No se encontraron conflictos activos.");
                return builder.ToString();
            }

            foreach (ReporteConflictoHorarioBackendDto item in resp.Data)
            {
                builder.AppendLine($"Tipo: {item.TipoConflicto}");
                builder.AppendLine($"Descripción: {item.Descripcion}");
                builder.AppendLine($"Grupo: {item.Grupo} | Docente: {item.Docente}");
                builder.AppendLine($"Materia: {item.Materia} | {item.DiaSemana} {item.HoraInicio} - {item.HoraFin}");
                builder.AppendLine(new string('-', 90));
            }

            return builder.ToString();
        }

        private static StringBuilder CrearEncabezado(string titulo)
        {
            var builder = new StringBuilder();
            builder.AppendLine(titulo);
            builder.AppendLine($"Fecha de consulta: {DateTime.Now:dd/MM/yyyy HH:mm}");
            builder.AppendLine(new string('=', 90));
            builder.AppendLine();
            return builder;
        }

        private static string ObtenerMensajeError(string? mensaje)
        {
            return string.IsNullOrWhiteSpace(mensaje)
                ? "No se pudo consultar la información del reporte."
                : mensaje;
        }
    }
}

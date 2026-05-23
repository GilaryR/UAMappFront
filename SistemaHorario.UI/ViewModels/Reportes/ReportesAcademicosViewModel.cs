using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Reportes
{
    /// <summary>
    /// Maneja la información real del módulo de reportes académicos.
    /// </summary>
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

            await CargarTiposReporteAsync();
            await CargarPeriodosAsync();
        }

        private async Task CargarTiposReporteAsync()
        {
            var tiposResp = await _api.ObtenerTiposReporteAsync();

            if (!tiposResp.Success || tiposResp.Data == null)
            {
                TiposReporte = new List<string>();

                MensajeEstado = string.IsNullOrWhiteSpace(tiposResp.Message)
                    ? "No se pudieron cargar los tipos de reporte."
                    : tiposResp.Message;

                return;
            }

            TiposReporte = tiposResp.Data;
        }

        private async Task CargarPeriodosAsync()
        {
            var periodosResp = await _api.ObtenerSemestresAsync();

            if (!periodosResp.Success || periodosResp.Data == null)
            {
                Periodos = new List<string>();

                if (string.IsNullOrWhiteSpace(MensajeEstado))
                {
                    MensajeEstado = string.IsNullOrWhiteSpace(periodosResp.Message)
                        ? "No se pudieron cargar los periodos."
                        : periodosResp.Message;
                }

                return;
            }

            Periodos = periodosResp.Data
                .Select(x => x.ToString())
                .ToList();
        }

        public async Task CargarReportesAsync()
        {
            MensajeEstado = string.Empty;

            List<ReporteAcademicoItem> reportes = new();

            await AgregarReporteGeneralAsync(reportes);
            await AgregarReporteUsuariosPorRolAsync(reportes);
            await AgregarReporteMateriasPorSemestreAsync(reportes);
            await AgregarReporteFranjasPorDiaAsync(reportes);
            await AgregarReportePlanesAcademicosAsync(reportes);

            _reportesBase = reportes
                .OrderByDescending(r => r.Fecha)
                .ToList();

            Reportes = _reportesBase.ToList();

            if (Reportes.Count == 0)
            {
                MensajeEstado = "No se encontraron reportes disponibles desde la API.";
            }
        }

        private async Task AgregarReporteGeneralAsync(
            List<ReporteAcademicoItem> reportes)
        {
            var resp = await _api.ObtenerReporteGeneralAsync();

            if (!resp.Success || resp.Data == null)
            {
                return;
            }

            reportes.Add(new ReporteAcademicoItem
            {
                IdReporte = 1,
                Fecha = DateTime.Now,
                TipoReporte = "Reporte general",
                Usuario = "Sistema",
                Detalle = "Resumen general",
                Periodo = "Global",
                FormatoInicial = "PDF",
                Descripcion =
                    $"Usuarios: {resp.Data.TotalUsuarios}, " +
                    $"Roles: {resp.Data.TotalRoles}, " +
                    $"Materias: {resp.Data.TotalMaterias}, " +
                    $"Planes académicos: {resp.Data.TotalPlanesAcademicos}, " +
                    $"Franjas horarias: {resp.Data.TotalFranjasHorarias}"
            });
        }

        private async Task AgregarReporteUsuariosPorRolAsync(
            List<ReporteAcademicoItem> reportes)
        {
            var resp = await _api.ObtenerUsuariosPorRolAsync();

            if (!resp.Success || resp.Data == null)
            {
                return;
            }

            int contador = 10;

            foreach (ReporteUsuariosPorRolBackendDto item in resp.Data)
            {
                reportes.Add(new ReporteAcademicoItem
                {
                    IdReporte = contador,
                    Fecha = DateTime.Now,
                    TipoReporte = "Usuarios por rol",
                    Usuario = "Sistema",
                    Detalle = item.Rol,
                    Periodo = "Global",
                    FormatoInicial = "CSV",
                    Descripcion =
                        $"Usuarios registrados con rol {item.Rol}: {item.TotalUsuarios}"
                });

                contador++;
            }
        }

        private async Task AgregarReporteMateriasPorSemestreAsync(
            List<ReporteAcademicoItem> reportes)
        {
            var resp = await _api.ObtenerMateriasPorSemestreAsync();

            if (!resp.Success || resp.Data == null)
            {
                return;
            }

            foreach (ReporteMateriasPorSemestreBackendDto item in resp.Data)
            {
                reportes.Add(new ReporteAcademicoItem
                {
                    IdReporte = 100 + item.Semestre,
                    Fecha = DateTime.Now,
                    TipoReporte = "Materias por semestre",
                    Usuario = "Sistema",
                    Detalle = $"Semestre {item.Semestre}",
                    Periodo = item.Semestre.ToString(),
                    FormatoInicial = "CSV",
                    Descripcion =
                        $"Total materias: {item.TotalMaterias}. " +
                        $"Materias: {string.Join(", ", item.Materias)}"
                });
            }
        }

        private async Task AgregarReporteFranjasPorDiaAsync(
            List<ReporteAcademicoItem> reportes)
        {
            var resp = await _api.ObtenerFranjasPorDiaAsync();

            if (!resp.Success || resp.Data == null)
            {
                return;
            }

            int contador = 200;

            foreach (ReporteFranjaHorariaBackendDto item in resp.Data)
            {
                reportes.Add(new ReporteAcademicoItem
                {
                    IdReporte = contador,
                    Fecha = DateTime.Now,
                    TipoReporte = "Franjas por día",
                    Usuario = "Sistema",
                    Detalle = item.Dia,
                    Periodo = "Global",
                    FormatoInicial = "PDF",
                    Descripcion = $"Total de franjas para {item.Dia}: {item.TotalFranjas}"
                });

                contador++;
            }
        }

        private async Task AgregarReportePlanesAcademicosAsync(
            List<ReporteAcademicoItem> reportes)
        {
            var resp = await _api.ObtenerPlanesAcademicosAsync();

            if (!resp.Success || resp.Data == null)
            {
                return;
            }

            foreach (ReportePlanAcademicoBackendDto item in resp.Data)
            {
                reportes.Add(new ReporteAcademicoItem
                {
                    IdReporte = 300 + item.IdPlanAcademico,
                    Fecha = DateTime.Now,
                    TipoReporte = "Planes académicos",
                    Usuario = "Sistema",
                    Detalle = item.Nombre,
                    Periodo = item.Anio.ToString(),
                    FormatoInicial = "PDF",
                    Descripcion =
                        $"Programa: {item.Programa}. " +
                        $"Semestres: {item.TotalSemestres}. " +
                        $"Materias: {item.TotalMaterias}."
                });
            }
        }

        public void AplicarFiltros()
        {
            IEnumerable<ReporteAcademicoItem> consulta = _reportesBase;

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                string busqueda = Busqueda.Trim().ToLower();

                consulta = consulta.Where(reporte =>
                    reporte.TipoReporte.ToLower().Contains(busqueda) ||
                    reporte.Usuario.ToLower().Contains(busqueda) ||
                    reporte.Detalle.ToLower().Contains(busqueda) ||
                    reporte.Periodo.ToLower().Contains(busqueda) ||
                    reporte.Descripcion.ToLower().Contains(busqueda));
            }

            if (!string.IsNullOrWhiteSpace(TipoSeleccionado) &&
                TipoSeleccionado != "Todos los tipos")
            {
                consulta = consulta.Where(reporte =>
                    reporte.TipoReporte == TipoSeleccionado);
            }

            if (FechaSeleccionada.HasValue)
            {
                consulta = consulta.Where(reporte =>
                    reporte.Fecha.Date == FechaSeleccionada.Value.Date);
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
    }
}
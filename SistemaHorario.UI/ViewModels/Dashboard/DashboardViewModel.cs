using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Dashboard
{
	/// <summary>
	/// ViewModel del Dashboard.
	///
	/// Esta clase prepara toda la información visual
	/// utilizada por DashboardView.
	///
	/// Actualmente utiliza datos temporales para validar UI.
	///
	/// Más adelante deberá conectarse con:
	///
	/// GET /api/dashboard/resumen
	///
	/// Endpoint futuro:
	/// GET /api/dashboard/ultimos-horarios
	///
	/// IMPORTANTE:
	/// Aunque algunos datos/endpoints todavía no existan,
	/// se deja toda la interfaz preparada para evitar
	/// modificar el diseño posteriormente.
	/// </summary>
	public class DashboardViewModel : ViewModelBase
	{
	private readonly DashboardApiService _api = new();
	private readonly HorariosApiService _horariosApi = new();

	private string _totalMaterias = "50";
	private string _totalPlanesAcademicos = "4";
	private string _totalUsuarios = "25";
	private string _totalGrupos = "12";
	private string _mensajeEstado = string.Empty;

	/// <summary>
	/// Total de materias.
	/// </summary>
	public string TotalMaterias
	{
		get => _totalMaterias;
		private set => SetProperty(ref _totalMaterias, value);
	}

	/// <summary>
	/// Total de planes académicos.
	/// </summary>
	public string TotalPlanesAcademicos
	{
		get => _totalPlanesAcademicos;
		private set => SetProperty(ref _totalPlanesAcademicos, value);
	}

	/// <summary>
	/// Total de usuarios.
	/// </summary>
	public string TotalUsuarios
	{
		get => _totalUsuarios;
		private set => SetProperty(ref _totalUsuarios, value);
	}

	/// <summary>
	/// Total de grupos.
	///
	/// TODO:
	/// Este dato todavía no existe en el endpoint,
	/// pero se deja preparada la card visual.
	/// </summary>
	public string TotalGrupos
	{
		get => _totalGrupos;
		private set => SetProperty(ref _totalGrupos, value);
	}

	public string MensajeEstado
	{
		get => _mensajeEstado;
		private set => SetProperty(ref _mensajeEstado, value);
	}

	/// <summary>
	/// Lista temporal de últimos horarios generados.
	///
	/// La tabla debe mostrar máximo 5 registros.
	///
	/// TODO:
	/// Reemplazar por endpoint real:
	/// GET /api/dashboard/ultimos-horarios
	/// o integrar desde dashboard/resumen si backend
	/// decide devolverlos allí.
	/// </summary>
	public ObservableCollection<HorarioGeneradoItem> UltimosHorarios { get; } = new();

	/// <summary>
	/// Constructor principal.
	/// </summary>
	public DashboardViewModel()
	{
		CargarDatosTemporales();
	}

	/// <summary>
	/// Consulta el resumen real desde backend.
	/// </summary>
	public async Task CargarResumenAsync()
	{
		var resp = await _api.ObtenerResumenAsync();

		if (!resp.Success || resp.Data == null)
		{
			MensajeEstado = resp.Message;
		}
		else
		{
			TotalMaterias = resp.Data.TotalMaterias.ToString();
			TotalPlanesAcademicos = resp.Data.TotalPlanesAcademicos.ToString();
			TotalUsuarios = resp.Data.TotalUsuarios.ToString();
			TotalGrupos = resp.Data.TotalGrupos.ToString();
		}

		await CargarUltimosHorariosAsync();
	}

	private async Task CargarUltimosHorariosAsync()
	{
		var resp = await _horariosApi.ObtenerHorariosAsync();
		if (!resp.Success || resp.Data == null || resp.Data.Count == 0)
			return;

		UltimosHorarios.Clear();

		foreach (var h in resp.Data.Take(5))
		{
			UltimosHorarios.Add(new HorarioGeneradoItem
			{
				Nombre = h.Nombre,
				Fecha = h.FechaGeneracion,
				Jornada = h.Jornada,
				Grupos = h.Grupo,
				Semestre = "-",
				Estado = h.Estado,
				JornadaFondo = ObtenerFondoJornada(h.Jornada),
				JornadaColorTexto = ObtenerTextoJornada(h.Jornada)
			});
		}
	}

	private static SolidColorBrush ObtenerFondoJornada(string jornada) =>
		jornada.ToLower() switch
		{
			"nocturna" => new SolidColorBrush(Color.FromRgb(221, 206, 255)),
			"diurna"   => new SolidColorBrush(Color.FromRgb(191, 225, 247)),
			_          => new SolidColorBrush(Color.FromRgb(220, 220, 220))
		};

	private static SolidColorBrush ObtenerTextoJornada(string jornada) =>
		jornada.ToLower() switch
		{
			"nocturna" => new SolidColorBrush(Color.FromRgb(102, 93, 214)),
			"diurna"   => new SolidColorBrush(Color.FromRgb(0, 106, 166)),
			_          => new SolidColorBrush(Color.FromRgb(80, 80, 80))
		};

		/// <summary>
		/// Carga datos temporales únicamente para validar UI.
		///
		/// Estos valores deberán reemplazarse posteriormente
		/// por respuestas reales de API.
		/// </summary>
		private void CargarDatosTemporales()
		{
			UltimosHorarios.Clear();

			UltimosHorarios.Add(new HorarioGeneradoItem
			{
				Nombre = "Horario_2026-01",
				Fecha = "27/04/2026",
				Jornada = "Nocturna",
				Grupos = "12",
				Semestre = "1",
				Estado = "Aprobado",
				JornadaFondo = new SolidColorBrush(
					Color.FromRgb(221, 206, 255)
				),
				JornadaColorTexto = new SolidColorBrush(
					Color.FromRgb(102, 93, 214)
				)
			});

			UltimosHorarios.Add(new HorarioGeneradoItem
			{
				Nombre = "Horario_2026-02",
				Fecha = "27/04/2026",
				Jornada = "Diurna",
				Grupos = "48",
				Semestre = "2",
				Estado = "Aprobado",
				JornadaFondo = new SolidColorBrush(
					Color.FromRgb(191, 225, 247)
				),
				JornadaColorTexto = new SolidColorBrush(
					Color.FromRgb(0, 106, 166)
				)
			});

			UltimosHorarios.Add(new HorarioGeneradoItem
			{
				Nombre = "Horario_2026-03",
				Fecha = "27/04/2026",
				Jornada = "Diurna",
				Grupos = "64",
				Semestre = "2",
				Estado = "Aprobado",
				JornadaFondo = new SolidColorBrush(
					Color.FromRgb(191, 225, 247)
				),
				JornadaColorTexto = new SolidColorBrush(
					Color.FromRgb(0, 106, 166)
				)
			});
		}
	}

	/// <summary>
	/// Modelo visual temporal para representar horarios generados
	/// dentro del Dashboard.
	///
	/// Esta clase pertenece únicamente a UI.
	///
	/// Más adelante deberá mapearse desde DTOs reales
	/// obtenidos desde la API.
	/// </summary>
	public class HorarioGeneradoItem
	{
		/// <summary>
		/// Nombre del horario generado.
		/// </summary>
		public string Nombre { get; set; } = string.Empty;

		/// <summary>
		/// Fecha de generación.
		/// </summary>
		public string Fecha { get; set; } = string.Empty;

		/// <summary>
		/// Jornada del horario.
		/// </summary>
		public string Jornada { get; set; } = string.Empty;

		/// <summary>
		/// Total de grupos.
		/// </summary>
		public string Grupos { get; set; } = string.Empty;

		/// <summary>
		/// Semestre asociado.
		/// </summary>
		public string Semestre { get; set; } = string.Empty;

		/// <summary>
		/// Estado del horario.
		/// </summary>
		public string Estado { get; set; } = string.Empty;

		/// <summary>
		/// Color de fondo visual para jornada.
		/// </summary>
		public Brush JornadaFondo { get; set; } = Brushes.Transparent;

		/// <summary>
		/// Color de texto visual para jornada.
		/// </summary>
		public Brush JornadaColorTexto { get; set; } = Brushes.Black;
	}
}

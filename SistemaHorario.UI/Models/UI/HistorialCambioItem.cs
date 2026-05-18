using System;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Modelo visual para representar una actividad registrada
	/// dentro del historial de cambios del sistema.
	///
	/// Este modelo está preparado para mapearse desde:
	///
	/// GET /api/historial-cambios
	///
	/// La finalidad del historial es mostrar acciones importantes
	/// realizadas dentro de la aplicación, por ejemplo:
	/// - creación de docentes,
	/// - actualización de horarios,
	/// - modificación del plan académico,
	/// - eliminación de materias,
	/// - generación de reportes.
	/// </summary>
	public class HistorialCambioItem
	{
		/// <summary>
		/// Identificador único del registro de historial.
		/// </summary>
		public int IdHistorial { get; set; }

		/// <summary>
		/// Fecha y hora en la que ocurrió la acción.
		/// </summary>
		public DateTime FechaHora { get; set; }

		/// <summary>
		/// Tipo de usuario que realizó la acción.
		///
		/// Ejemplo:
		/// Docente, Coordinador.
		/// </summary>
		public string Usuario { get; set; } = string.Empty;

		/// <summary>
		/// Módulo del sistema donde ocurrió el cambio.
		///
		/// Ejemplo:
		/// Plan académico, Horarios, Docentes, Materias.
		/// </summary>
		public string Modulo { get; set; } = string.Empty;

		/// <summary>
		/// Descripción clara de la acción realizada.
		/// </summary>
		public string Descripcion { get; set; } = string.Empty;

		/// <summary>
		/// Texto formateado para mostrar en la lista visual.
		/// </summary>
		public string TextoActividad =>
			$"{FechaHora:dd/MM/yyyy}  {FechaHora:HH:mm}  {Usuario} {Descripcion}";
	}
}
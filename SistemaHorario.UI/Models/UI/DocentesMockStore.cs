using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Almacén temporal para validar la interfaz del módulo Docentes.
	///
	/// TODO:
	/// Reemplazar esta clase por consumo real de endpoints:
	/// - GET /api/Docentes
	/// - POST /api/Docentes
	/// - PUT /api/Docentes/{id}
	/// - DELETE /api/Docentes/{id}
	/// - GET /api/Docentes/{id}/disponibilidad
	/// - PUT /api/Docentes/{id}/disponibilidad
	/// </summary>
	public static class DocentesMockStore
	{
		private static readonly List<DocenteItem> _docentes = new();

		private static readonly Dictionary<int, List<DisponibilidadDocenteItem>> _disponibilidades = new();

		private static int _siguienteId = 10;

		public static List<DocenteItem> ObtenerDocentes()
		{
			InicializarSiEstaVacio();

			return _docentes.ToList();
		}

		public static DocenteItem CrearDocente(DocenteItem docente)
		{
			docente.IdDocente = _siguienteId;
			_siguienteId++;

			_docentes.Insert(0, docente);

			return docente;
		}

		public static void ActualizarDocente(DocenteItem docenteActualizado)
		{
			DocenteItem? existente = _docentes
				.FirstOrDefault(d => d.IdDocente == docenteActualizado.IdDocente);

			if (existente == null)
				return;

			existente.NombreCompleto = docenteActualizado.NombreCompleto;
			existente.Identificacion = docenteActualizado.Identificacion;
			existente.CorreoInstitucional = docenteActualizado.CorreoInstitucional;
			existente.Materias = docenteActualizado.Materias;
			existente.Estado = docenteActualizado.Estado;
		}

		public static void EliminarDocente(DocenteItem docente)
		{
			DocenteItem? existente = _docentes
				.FirstOrDefault(d => d.IdDocente == docente.IdDocente);

			if (existente != null)
				_docentes.Remove(existente);

			if (_disponibilidades.ContainsKey(docente.IdDocente))
				_disponibilidades.Remove(docente.IdDocente);
		}

		public static List<DisponibilidadDocenteItem> ObtenerDisponibilidad(int idDocente)
		{
			if (_disponibilidades.TryGetValue(idDocente, out List<DisponibilidadDocenteItem>? disponibilidad))
			{
				return disponibilidad.Select(d => new DisponibilidadDocenteItem
				{
					Dia = d.Dia,
					HoraInicio = d.HoraInicio,
					HoraFin = d.HoraFin,
					Disponible = d.Disponible
				}).ToList();
			}

			return CrearDisponibilidadBase();
		}

		public static void GuardarDisponibilidad(
			int idDocente,
			List<DisponibilidadDocenteItem> disponibilidad)
		{
			_disponibilidades[idDocente] = disponibilidad.Select(d => new DisponibilidadDocenteItem
			{
				Dia = d.Dia,
				HoraInicio = d.HoraInicio,
				HoraFin = d.HoraFin,
				Disponible = d.Disponible
			}).ToList();
		}

		public static List<DisponibilidadDocenteItem> CrearDisponibilidadBase()
		{
			string[] dias =
			[
				"Lunes",
				"Martes",
				"Miércoles",
				"Jueves",
				"Viernes",
				"Sábado"
			];

			string[] franjas =
			[
				"7:00 AM",
				"8:00 AM",
				"9:00 AM",
				"10:00 AM",
				"11:00 AM",
				"12:00 - 2:00",
				"2:00 PM",
				"3:00 PM",
				"4:00 PM",
				"5:00 PM",
				"6:00 PM",
				"7:00 PM",
				"8:00 PM",
				"9:00 PM",
				"10:00 PM"
			];

			List<DisponibilidadDocenteItem> disponibilidad = new();

			foreach (string franja in franjas)
			{
				foreach (string dia in dias)
				{
					disponibilidad.Add(new DisponibilidadDocenteItem
					{
						Dia = dia,
						HoraInicio = franja,
						HoraFin = string.Empty,
						Disponible = false
					});
				}
			}

			return disponibilidad;
		}

		private static void InicializarSiEstaVacio()
		{
			if (_docentes.Count > 0)
				return;

			_docentes.Add(new DocenteItem
			{
				IdDocente = 1,
				NombreCompleto = "María Osorio",
				Identificacion = "10294675",
				CorreoInstitucional = "maria.osorio@autonoma.edu.co",
				Materias = "Física I, Física II",
				Estado = "Activo"
			});

			_docentes.Add(new DocenteItem
			{
				IdDocente = 2,
				NombreCompleto = "Luna Ricardo López",
				Identificacion = "10294676",
				CorreoInstitucional = "luna.lopez@autonoma.edu.co",
				Materias = "Cálculo integral",
				Estado = "Activo"
			});

			_docentes.Add(new DocenteItem
			{
				IdDocente = 3,
				NombreCompleto = "Harry Potter",
				Identificacion = "10294679",
				CorreoInstitucional = "harry.potter@autonoma.edu.co",
				Materias = "Ética",
				Estado = "Activo"
			});

			_docentes.Add(new DocenteItem
			{
				IdDocente = 4,
				NombreCompleto = "Docente temporal",
				Identificacion = "00000000",
				CorreoInstitucional = "temporal@autonoma.edu.co",
				Materias = "Sin asignar",
				Estado = "Inactivo"
			});
		}

		/// <summary>
		/// Retorna las materias disponibles para asignar a un docente.
		/// 
		/// Por ahora son datos temporales para la interfaz.
		/// Cuando backend esté listo, este método se reemplaza por el endpoint
		/// que devuelva las materias disponibles.
		/// </summary>
		public static List<string> ObtenerMateriasDisponibles()
		{
			return new List<string>
		    {
			"Cálculo Diferencial",
			"Álgebra Lineal",
			"Cálculo Integral",
			"Física I",
			"Física II",
			"POO",
			"Redes LAN",
			"Bases de datos I",
			"Ingeniería de software I",
			"Técnicas de programación"
		     };
		}
	}
}
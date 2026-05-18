using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.Models.UI
{
	/// <summary>
	/// Mock temporal del módulo coordinadores.
	///
	/// IMPORTANTE:
	/// Este archivo debe eliminarse cuando
	/// el backend esté conectado.
	/// </summary>
	public static class CoordinadoresMockStore
	{
		private static readonly List<CoordinadorItem> _coordinadores =
		[
			new()
			{
				IdCoordinador = 1,
				NombreCompleto = "Marcela Gómez",
				Cedula = "12345678",
				CorreoInstitucional = "marcela@uam.edu.co",
				Rol = "Coordinador",
				Estado = "Activo",
				Celular = "3151234567"
			},

			new()
			{
				IdCoordinador = 2,
				NombreCompleto = "Juan Pérez",
				Cedula = "10987654",
				CorreoInstitucional = "juan@uam.edu.co",
				Rol = "Coordinador",
				Estado = "Inactivo",
				Celular = "3204567890"
			}
		];

		public static List<CoordinadorItem> Obtener()
		{
			return _coordinadores.ToList();
		}

		public static void Crear(CoordinadorItem coordinador)
		{
			coordinador.IdCoordinador =
				_coordinadores.Max(x => x.IdCoordinador) + 1;

			_coordinadores.Add(coordinador);
		}

		public static void Eliminar(int id)
		{
			CoordinadorItem? coordinador =
				_coordinadores.FirstOrDefault(x =>
					x.IdCoordinador == id);

			if (coordinador != null)
				_coordinadores.Remove(coordinador);
		}
	}
}
using System;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual de coordinador dentro del sistema.
    /// </summary>
    public class CoordinadorItem
    {
        public int IdCoordinador { get; set; }

        public string NombreCompleto { get; set; } = string.Empty;

        public string Cedula { get; set; } = string.Empty;

        public string CorreoInstitucional { get; set; } = string.Empty;

        public string Rol { get; set; } = "Coordinador";

        public string Estado { get; set; } = "Activo";

        public string Celular { get; set; } = string.Empty;

        public string ContrasenaInicial { get; set; } = "Coordinador123!";

        public bool EstaActivo =>
            string.Equals(Estado, "Activo", StringComparison.OrdinalIgnoreCase);
    }
}


namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Define una columna dinámica para TablaPaginada.
    /// </summary>
    public class TableColumnDefinition
    {
        public string Header { get; set; } = string.Empty;
        public string Binding { get; set; } = string.Empty;
        public double Width { get; set; } = 1;
    }
}
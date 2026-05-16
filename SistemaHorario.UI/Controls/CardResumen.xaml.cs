using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SistemaHorario.UI.Controls
{
    /// <summary>
    /// Control reutilizable utilizado para mostrar
    /// tarjetas resumen dentro del sistema.
    ///
    /// Este control permite visualizar:
    /// - Un icono representativo.
    /// - Un título descriptivo.
    /// - Un valor numérico o estadístico.
    ///
    /// Ejemplos de uso:
    /// - Total de materias.
    /// - Total de docentes.
    /// - Horarios generados.
    /// - Reportes disponibles.
    ///
    /// El contenido se configura dinámicamente
    /// mediante el método Configurar.
    /// </summary>
    public partial class CardResumen : UserControl
    {
        /// <summary>
        /// Constructor del control CardResumen.
        ///
        /// Inicializa todos los componentes visuales
        /// definidos en CardResumen.xaml.
        /// </summary>
        public CardResumen()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Configura dinámicamente la información
        /// mostrada en la tarjeta resumen.
        ///
        /// Parámetros:
        /// - titulo: texto principal de la tarjeta.
        /// - valor: valor estadístico o cantidad.
        /// - rutaIcono: ruta del icono mostrado.
        ///
        /// Este método permite reutilizar el mismo control
        /// para distintos módulos del sistema.
        /// </summary>
        /// <param name="titulo">
        /// Título descriptivo de la tarjeta.
        /// </param>
        /// <param name="valor">
        /// Valor numérico o texto principal.
        /// </param>
        /// <param name="rutaIcono">
        /// Ruta relativa o absoluta del icono.
        /// </param>
        public void Configurar(string titulo, string valor, string rutaIcono)
        {
            TxtTitulo.Text = titulo;
            TxtValor.Text = valor;

            if (!string.IsNullOrWhiteSpace(rutaIcono))
            {
                ImgIcono.Source =
                    new BitmapImage(
                        new Uri(rutaIcono, UriKind.RelativeOrAbsolute)
                    );
            }
        }
    }
}
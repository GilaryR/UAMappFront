using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SistemaHorario.UI.Controls
{
    /// <summary>
    /// Control reutilizable de selección múltiple.
    ///
    /// Permite seleccionar varios elementos desde una lista desplegable
    /// y mostrarlos como etiquetas dentro de un panel.
    ///
    /// Este control no consume endpoints directamente.
    /// Los datos deben ser enviados desde el ViewModel o desde la vista.
    ///
    /// Puede utilizarse para:
    /// - idsMaterias
    /// - dias
    /// - prerrequisitos
    /// - docentes
    /// - grupos
    /// </summary>
    public partial class MultiSelectDropdown : UserControl
    {
        /// <summary>
        /// Lista completa de opciones disponibles.
        /// </summary>
        private readonly List<MultiSelectItem> _opciones = new();

        /// <summary>
        /// Lista de opciones actualmente seleccionadas.
        /// </summary>
        private readonly List<MultiSelectItem> _seleccionados = new();

        /// <summary>
        /// Constructor del control MultiSelectDropdown.
        /// </summary>
        public MultiSelectDropdown()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Configura el título visible del control.
        /// </summary>
        /// <param name="titulo">
        /// Texto que aparecerá como título del selector.
        /// </param>
        public void ConfigurarTitulo(string titulo)
        {
            TxtTitulo.Text = titulo;
        }

        /// <summary>
        /// Carga dinámicamente las opciones disponibles.
        ///
        /// Este método será usado cuando los datos vengan
        /// desde endpoints como:
        /// - GET /api/materias/activas
        /// - GET /api/catalogos/dias
        /// - GET /api/docentes/activos
        /// </summary>
        /// <param name="opciones">
        /// Lista de opciones disponibles.
        /// </param>
        public void CargarOpciones(List<MultiSelectItem> opciones)
        {
            _opciones.Clear();
            _seleccionados.Clear();

            _opciones.AddRange(opciones);

            RefrescarOpciones();
            RefrescarSeleccionados();
            ActualizarResumen();
        }

        /// <summary>
        /// Obtiene todos los elementos seleccionados.
        /// </summary>
        /// <returns>
        /// Lista de elementos seleccionados.
        /// </returns>
        public List<MultiSelectItem> ObtenerSeleccionados()
        {
            return _seleccionados.ToList();
        }

        /// <summary>
        /// Obtiene únicamente los Id seleccionados.
        ///
        /// Útil para requests como:
        /// idsMaterias: [1, 2, 3]
        /// </summary>
        /// <returns>
        /// Lista de identificadores seleccionados.
        /// </returns>
        public List<int> ObtenerIdsSeleccionados()
        {
            return _seleccionados
                .Select(x => x.Id)
                .ToList();
        }

        /// <summary>
        /// Obtiene únicamente los textos seleccionados.
        ///
        /// Útil para requests como:
        /// dias: ["Lunes", "Martes"]
        /// </summary>
        /// <returns>
        /// Lista de textos seleccionados.
        /// </returns>
        public List<string> ObtenerTextosSeleccionados()
        {
            return _seleccionados
                .Select(x => x.Texto)
                .ToList();
        }

        /// <summary>
        /// Limpia todos los elementos seleccionados.
        /// </summary>
        public void Limpiar()
        {
            _seleccionados.Clear();

            foreach (MultiSelectItem opcion in _opciones)
            {
                opcion.Seleccionado = false;
            }

            RefrescarOpciones();
            RefrescarSeleccionados();
            ActualizarResumen();
        }

        /// <summary>
        /// Muestra u oculta la lista de opciones disponibles.
        /// </summary>
        private void BtnDesplegar_Click(object sender, RoutedEventArgs e)
        {
            PnlOpciones.Visibility =
                PnlOpciones.Visibility == Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }

        /// <summary>
        /// Evento ejecutado al hacer clic sobre una opción disponible.
        /// Agrega el elemento al panel de seleccionados.
        /// </summary>
        private void BtnOpcion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.DataContext is not MultiSelectItem item)
                return;

            if (item.Seleccionado)
                return;

            item.Seleccionado = true;
            _seleccionados.Add(item);

            RefrescarOpciones();
            RefrescarSeleccionados();
            ActualizarResumen();
        }

        /// <summary>
        /// Elimina un elemento seleccionado.
        /// </summary>
        private void BtnEliminarSeleccion_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.Tag is not MultiSelectItem item)
                return;

            item.Seleccionado = false;
            _seleccionados.RemoveAll(x => x.Id == item.Id);

            RefrescarOpciones();
            RefrescarSeleccionados();
            ActualizarResumen();
        }

        /// <summary>
        /// Actualiza la lista de opciones visibles,
        /// ocultando las que ya fueron seleccionadas.
        /// </summary>
        private void RefrescarOpciones()
        {
            LstOpciones.ItemsSource = null;

            LstOpciones.ItemsSource = _opciones
                .Where(x => !x.Seleccionado)
                .ToList();
        }

        /// <summary>
        /// Refresca visualmente el panel inferior
        /// donde se muestran los elementos seleccionados.
        /// </summary>
        private void RefrescarSeleccionados()
        {
            PnlSeleccionados.Children.Clear();

            foreach (MultiSelectItem item in _seleccionados)
            {
                Border chip = CrearChipSeleccionado(item);
                PnlSeleccionados.Children.Add(chip);
            }
        }

        /// <summary>
        /// Crea una etiqueta visual para un elemento seleccionado.
        /// </summary>
        /// <param name="item">
        /// Elemento seleccionado.
        /// </param>
        /// <returns>
        /// Border que representa visualmente el chip.
        /// </returns>
        private Border CrearChipSeleccionado(MultiSelectItem item)
        {
            Button botonEliminar = new()
            {
                Content = "×",
                Tag = item,
                Width = 22,
                Height = 22,
                Margin = new Thickness(8, 0, 0, 0),
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                Foreground = Brushes.White,
                Cursor = System.Windows.Input.Cursors.Hand
            };

            botonEliminar.Click += BtnEliminarSeleccion_Click;

            StackPanel contenido = new()
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };

            contenido.Children.Add(new TextBlock
            {
                Text = item.Texto,
                Foreground = Brushes.White,
                FontSize = 12,
                VerticalAlignment = VerticalAlignment.Center
            });

            contenido.Children.Add(botonEliminar);

            return new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(8, 120, 184)),
                CornerRadius = new CornerRadius(14),
                Padding = new Thickness(10, 5, 6, 5),
                Margin = new Thickness(4),
                Child = contenido
            };
        }

        /// <summary>
        /// Actualiza el texto resumen superior.
        /// </summary>
        private void ActualizarResumen()
        {
            if (_seleccionados.Count == 0)
            {
                TxtResumen.Text = "Sin elementos seleccionados";
                return;
            }

            TxtResumen.Text = $"{_seleccionados.Count} elemento(s) seleccionado(s)";
        }
    }
}
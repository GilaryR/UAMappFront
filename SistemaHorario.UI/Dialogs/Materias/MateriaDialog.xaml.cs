using SistemaHorario.UI.Models.UI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SistemaHorario.UI.Dialogs.Materias
{
    /// <summary>
    /// Define el modo de uso del dialog de materias.
    ///
    /// Crear:
    /// - Campos vacíos.
    /// - Permite guardar una nueva materia.
    ///
    /// Editar:
    /// - Campos llenos.
    /// - Permite modificar información.
    ///
    /// Ver:
    /// - Campos llenos.
    /// - No permite editar información.
    /// </summary>
    public enum ModoMateriaDialog
    {
        Crear,
        Editar,
        Ver
    }

    /// <summary>
    /// Dialog reutilizable para crear, editar o visualizar materias.
    ///
    /// Este dialog pertenece únicamente a la capa UI.
    ///
    /// Actualmente trabaja con MateriaItem como modelo visual.
    ///
    /// Más adelante, cuando se conecte la API, el servicio deberá mapear:
    /// - CrearMateriaRequest
    /// - ActualizarMateriaRequest
    /// - MateriaResponse
    /// hacia este modelo visual o hacia el ViewModel correspondiente.
    ///
    /// El endpoint actual de materias maneja campos como:
    /// codigo, nombre, creditos, intensidadHorariaSemanal,
    /// semestre y activa. El campo CantidadGrupos queda preparado
    /// para integración futura.
    /// </summary>
    public partial class MateriaDialog : Window
    {
        private readonly ModoMateriaDialog _modo;

        private readonly ObservableCollection<MateriaItem> _prerrequisitosDisponibles;
        private readonly ObservableCollection<MateriaItem> _prerrequisitosSeleccionados;

        /// <summary>
        /// Resultado final del dialog.
        ///
        /// Contiene la materia creada o editada cuando el usuario
        /// presiona Guardar.
        /// </summary>
        public MateriaItem? MateriaResultado { get; private set; }

        /// <summary>
        /// Lista de prerrequisitos seleccionados por el usuario.
        /// </summary>
        public List<MateriaItem> PrerrequisitosSeleccionados =>
            _prerrequisitosSeleccionados.ToList();

        /// <summary>
        /// Constructor principal del dialog.
        /// </summary>
        /// <param name="modo">
        /// Modo de apertura: Crear, Editar o Ver.
        /// </param>
        /// <param name="materia">
        /// Materia existente cuando el modo es Editar o Ver.
        /// En modo Crear puede venir nula.
        /// </param>
        /// <param name="prerrequisitosDisponibles">
        /// Lista de materias disponibles para seleccionar como prerrequisito.
        /// </param>
        /// <param name="prerrequisitosSeleccionados">
        /// Lista de prerrequisitos ya seleccionados.
        /// </param>
        public MateriaDialog(
            ModoMateriaDialog modo,
            MateriaItem? materia = null,
            IEnumerable<MateriaItem>? prerrequisitosDisponibles = null,
            IEnumerable<MateriaItem>? prerrequisitosSeleccionados = null)
        {
            InitializeComponent();

            _modo = modo;

            _prerrequisitosDisponibles =
                new ObservableCollection<MateriaItem>(
                    prerrequisitosDisponibles ?? new List<MateriaItem>()
                );

            _prerrequisitosSeleccionados =
                new ObservableCollection<MateriaItem>(
                    prerrequisitosSeleccionados ?? new List<MateriaItem>()
                );

            MateriaResultado = materia == null
                ? new MateriaItem()
                : ClonarMateria(materia);

            ConfigurarModo();
            CargarDatosMateria();
            CargarListaPrerrequisitos();
            PintarPrerrequisitosSeleccionados();
        }

        /// <summary>
        /// Configura el aspecto y comportamiento del dialog
        /// según el modo seleccionado.
        /// </summary>
        private void ConfigurarModo()
        {
            switch (_modo)
            {
                case ModoMateriaDialog.Crear:
                    Title = "Crear materia";
                    BtnGuardar.Content = "Guardar";
                    break;

                case ModoMateriaDialog.Editar:
                    Title = "Editar materia";
                    BtnGuardar.Content = "Guardar";
                    break;

                case ModoMateriaDialog.Ver:
                    Title = "Ver materia";
                    BtnGuardar.Visibility = Visibility.Collapsed;
                    BtnCancelar.Content = "Cerrar";
                    DeshabilitarFormulario();
                    break;
            }
        }

        /// <summary>
        /// Carga en los controles visuales la información
        /// de la materia actual.
        /// </summary>
        private void CargarDatosMateria()
        {
            if (MateriaResultado == null)
                return;

            TxtCodigo.Text = MateriaResultado.Codigo;
            TxtNombre.Text = MateriaResultado.Nombre;
            TxtCreditos.Text = MateriaResultado.Creditos == 0
                ? string.Empty
                : MateriaResultado.Creditos.ToString();

            TxtIntensidad.Text = MateriaResultado.IntensidadHorariaSemanal == 0
                ? string.Empty
                : MateriaResultado.IntensidadHorariaSemanal.ToString();

            TxtCantidadGrupos.Text = MateriaResultado.CantidadGrupos == 0
                ? string.Empty
                : MateriaResultado.CantidadGrupos.ToString();

            SeleccionarSemestre(MateriaResultado.Semestre);
        }

        /// <summary>
        /// Selecciona el semestre correspondiente en el ComboBox.
        /// </summary>
        private void SeleccionarSemestre(int semestre)
        {
            foreach (ComboBoxItem item in CmbSemestre.Items)
            {
                if (item.Content?.ToString() == semestre.ToString())
                {
                    CmbSemestre.SelectedItem = item;
                    return;
                }
            }
        }

        /// <summary>
        /// Carga las materias disponibles en la lista desplegable
        /// de prerrequisitos.
        /// </summary>
        private void CargarListaPrerrequisitos()
        {
            ListaPrerrequisitos.ItemsSource =
                _prerrequisitosDisponibles
                    .Where(m => !_prerrequisitosSeleccionados
                        .Any(s => s.IdMateria == m.IdMateria))
                    .ToList();
        }

        /// <summary>
        /// Pinta visualmente los prerrequisitos seleccionados
        /// como chips dentro del panel.
        /// </summary>
        private void PintarPrerrequisitosSeleccionados()
        {
            PanelPrerrequisitos.Children.Clear();

            foreach (MateriaItem materia in _prerrequisitosSeleccionados)
            {
                Border chip = CrearChipPrerrequisito(materia);
                PanelPrerrequisitos.Children.Add(chip);
            }
        }

        /// <summary>
        /// Crea un chip visual para un prerrequisito seleccionado.
        /// </summary>
        private Border CrearChipPrerrequisito(MateriaItem materia)
        {
            Border contenedor = new Border
            {
                Background = new SolidColorBrush(Color.FromRgb(238, 238, 238)),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 8, 8),
                Padding = new Thickness(10, 4, 8, 4)
            };

            StackPanel panel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            TextBlock texto = new TextBlock
            {
                Text = $"{materia.Nombre} ({materia.Codigo})",
                FontWeight = FontWeights.SemiBold,
                FontSize = 13,
                VerticalAlignment = VerticalAlignment.Center
            };

            Button botonEliminar = new Button
            {
                Content = "×",
                Margin = new Thickness(8, 0, 0, 0),
                Width = 20,
                Height = 20,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0),
                FontSize = 16,
                Cursor = System.Windows.Input.Cursors.Hand,
                Tag = materia
            };

            botonEliminar.Click += BtnEliminarPrerrequisito_Click;

            if (_modo == ModoMateriaDialog.Ver)
                botonEliminar.Visibility = Visibility.Collapsed;

            panel.Children.Add(texto);
            panel.Children.Add(botonEliminar);

            contenedor.Child = panel;

            return contenedor;
        }

        /// <summary>
        /// Deshabilita todos los campos del formulario
        /// cuando el dialog se abre en modo Ver.
        /// </summary>
        private void DeshabilitarFormulario()
        {
            TxtCodigo.IsEnabled = false;
            TxtNombre.IsEnabled = false;
            TxtCreditos.IsEnabled = false;
            TxtIntensidad.IsEnabled = false;
            TxtCantidadGrupos.IsEnabled = false;
            CmbSemestre.IsEnabled = false;
            BtnMostrarPrerrequisitos.IsEnabled = false;
        }

        /// <summary>
        /// Muestra u oculta la lista de prerrequisitos.
        /// </summary>
        private void BtnMostrarPrerrequisitos_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (_modo == ModoMateriaDialog.Ver)
                return;

            PopupPrerrequisitos.IsOpen = true;
        }

        /// <summary>
        /// Agrega una materia como prerrequisito.
        /// </summary>
        private void BtnAgregarPrerrequisito_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.DataContext is not MateriaItem materia)
                return;

            if (_prerrequisitosSeleccionados.Any(m => m.IdMateria == materia.IdMateria))
                return;

            _prerrequisitosSeleccionados.Add(materia);

            CargarListaPrerrequisitos();
            PintarPrerrequisitosSeleccionados();
        }

        /// <summary>
        /// Elimina una materia de los prerrequisitos seleccionados.
        /// </summary>
        private void BtnEliminarPrerrequisito_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button boton)
                return;

            if (boton.Tag is not MateriaItem materia)
                return;

            _prerrequisitosSeleccionados.Remove(materia);

            CargarListaPrerrequisitos();
            PintarPrerrequisitosSeleccionados();
        }

        /// <summary>
        /// Cancela el dialog o lo cierra en modo Ver.
        /// </summary>
        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Valida la información y guarda el resultado del dialog.
        /// </summary>
        private void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!FormularioEsValido())
                return;

            GuardarDatosEnResultado();

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Valida los campos obligatorios del formulario.
        /// </summary>
        private bool FormularioEsValido()
        {
            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
            {
                MessageBox.Show(
                    "El código es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MessageBox.Show(
                    "El nombre es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            if (!int.TryParse(TxtCreditos.Text, out _))
            {
                MessageBox.Show(
                    "Los créditos deben ser un número válido.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            if (!int.TryParse(TxtIntensidad.Text, out _))
            {
                MessageBox.Show(
                    "La intensidad horaria debe ser un número válido.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            if (!int.TryParse(TxtCantidadGrupos.Text, out _))
            {
                MessageBox.Show(
                    "La cantidad de grupos debe ser un número válido.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            if (CmbSemestre.SelectedItem == null)
            {
                MessageBox.Show(
                    "Debe seleccionar un semestre.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            return true;
        }

        /// <summary>
        /// Copia los valores del formulario al modelo resultado.
        /// </summary>
        private void GuardarDatosEnResultado()
        {
            if (MateriaResultado == null)
                MateriaResultado = new MateriaItem();

            MateriaResultado.Codigo = TxtCodigo.Text.Trim();
            MateriaResultado.Nombre = TxtNombre.Text.Trim();
            MateriaResultado.Creditos = int.Parse(TxtCreditos.Text.Trim());
            MateriaResultado.IntensidadHorariaSemanal = int.Parse(TxtIntensidad.Text.Trim());
            MateriaResultado.CantidadGrupos = int.Parse(TxtCantidadGrupos.Text.Trim());

            if (CmbSemestre.SelectedItem is ComboBoxItem item)
            {
                MateriaResultado.Semestre =
                    int.Parse(item.Content?.ToString() ?? "1");
            }

            if (_modo == ModoMateriaDialog.Crear)
                MateriaResultado.Activa = true;
        }

        /// <summary>
        /// Crea una copia simple de una materia para evitar
        /// modificar directamente el elemento original de la tabla.
        /// </summary>
        private MateriaItem ClonarMateria(MateriaItem materia)
        {
            return new MateriaItem
            {
                IdMateria = materia.IdMateria,
                Codigo = materia.Codigo,
                Nombre = materia.Nombre,
                Creditos = materia.Creditos,
                IntensidadHorariaSemanal = materia.IntensidadHorariaSemanal,
                Semestre = materia.Semestre,
                CantidadGrupos = materia.CantidadGrupos,
                Activa = materia.Activa
            };
        }
    }
}
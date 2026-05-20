using SistemaHorario.UI.Models.UI;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.GruposAcademicos
{
    public enum ModoGrupoAcademicoDialog
    {
        Crear,
        Editar,
        Ver
    }

    public partial class GrupoAcademicoDialog : Window
    {
        private readonly ModoGrupoAcademicoDialog _modo;

        public GrupoAcademicoItem GrupoResultado { get; private set; }

        public GrupoAcademicoDialog(
            ModoGrupoAcademicoDialog modo,
            GrupoAcademicoItem? grupo,
            ObservableCollection<string> materiasDisponibles)
        {
            InitializeComponent();

            _modo = modo;
            GrupoResultado = grupo?.Clonar() ?? new GrupoAcademicoItem();

            CmbMateria.ItemsSource = materiasDisponibles;

            ConfigurarModo();
            CargarDatos();
        }

        private void ConfigurarModo()
        {
            if (_modo == ModoGrupoAcademicoDialog.Crear)
            {
                TxtTitulo.Text = "Nuevo grupo académico";
                BtnGuardar.Content = "Crear";
                return;
            }

            if (_modo == ModoGrupoAcademicoDialog.Editar)
            {
                TxtTitulo.Text = "Editar grupo académico";
                BtnGuardar.Content = "Guardar";
                return;
            }

            TxtTitulo.Text = "Detalle del grupo académico";
            BtnGuardar.Visibility = Visibility.Collapsed;

            TxtNombreGrupo.IsReadOnly = true;
            TxtCodigo.IsReadOnly = true;
            TxtPlazas.IsReadOnly = true;
            TxtSemestre.IsReadOnly = true;

            CmbJornada.IsEnabled = false;
            CmbMateria.IsEnabled = false;
            CmbEstado.IsEnabled = false;

            ChkLunes.IsEnabled = false;
            ChkMartes.IsEnabled = false;
            ChkMiercoles.IsEnabled = false;
            ChkJueves.IsEnabled = false;
            ChkViernes.IsEnabled = false;
            ChkSabado.IsEnabled = false;
        }

        private void CargarDatos()
        {
            TxtNombreGrupo.Text = GrupoResultado.NombreGrupo;
            TxtCodigo.Text = GrupoResultado.Codigo;

            TxtPlazas.Text = GrupoResultado.PlazasDisponibles == 0
                ? string.Empty
                : GrupoResultado.PlazasDisponibles.ToString();

            TxtSemestre.Text = GrupoResultado.NumeroSemestre == 0
                ? "1"
                : GrupoResultado.NumeroSemestre.ToString();

            SeleccionarComboPorTexto(CmbJornada, GrupoResultado.Jornada);
            SeleccionarComboPorTexto(CmbEstado, GrupoResultado.Estado);

            CmbMateria.SelectedItem = GrupoResultado.Materia;

            ChkLunes.IsChecked = GrupoResultado.Dias.Contains("Lunes");
            ChkMartes.IsChecked = GrupoResultado.Dias.Contains("Martes");
            ChkMiercoles.IsChecked = GrupoResultado.Dias.Contains("Miércoles");
            ChkJueves.IsChecked = GrupoResultado.Dias.Contains("Jueves");
            ChkViernes.IsChecked = GrupoResultado.Dias.Contains("Viernes");
            ChkSabado.IsChecked = GrupoResultado.Dias.Contains("Sábado");
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidarFormulario())
                return;

            GrupoResultado.NombreGrupo = TxtNombreGrupo.Text.Trim();
            GrupoResultado.Codigo = TxtCodigo.Text.Trim();
            GrupoResultado.Jornada = ObtenerTextoCombo(CmbJornada);
            GrupoResultado.Materia = CmbMateria.SelectedItem?.ToString() ?? string.Empty;
            GrupoResultado.Estado = ObtenerTextoCombo(CmbEstado);
            GrupoResultado.PlazasDisponibles = int.Parse(TxtPlazas.Text.Trim());
            GrupoResultado.NumeroSemestre = int.Parse(TxtSemestre.Text.Trim());
            GrupoResultado.Dias = ObtenerDiasSeleccionados();

            GrupoResultado.Tipo = GrupoResultado.Jornada == "Nocturno"
                ? "TAPSI"
                : "Regular";

            DialogResult = true;
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(TxtNombreGrupo.Text))
            {
                MessageBox.Show("⚠ Ingresa el nombre del grupo.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCodigo.Text))
            {
                MessageBox.Show("⚠ Ingresa el código del grupo.");
                return false;
            }

            if (CmbJornada.SelectedItem == null)
            {
                MessageBox.Show("⚠ Selecciona la jornada.");
                return false;
            }

            if (CmbMateria.SelectedItem == null)
            {
                MessageBox.Show("⚠ Selecciona la materia.");
                return false;
            }

            if (CmbEstado.SelectedItem == null)
            {
                MessageBox.Show("⚠ Selecciona el estado.");
                return false;
            }

            if (!int.TryParse(TxtPlazas.Text.Trim(), out int plazas) || plazas <= 0)
            {
                MessageBox.Show("⚠ Ingresa una cantidad válida de plazas.");
                return false;
            }

            if (!int.TryParse(TxtSemestre.Text.Trim(), out int semestre) || semestre <= 0)
            {
                MessageBox.Show("⚠ Ingresa un número de semestre válido (ej: 1, 2, 3...).");
                return false;
            }

            if (ObtenerDiasSeleccionados().Count == 0)
            {
                MessageBox.Show("⚠ Selecciona al menos un día.");
                return false;
            }

            return true;
        }

        private ObservableCollection<string> ObtenerDiasSeleccionados()
        {
            ObservableCollection<string> dias = new();

            if (ChkLunes.IsChecked == true)
                dias.Add("Lunes");

            if (ChkMartes.IsChecked == true)
                dias.Add("Martes");

            if (ChkMiercoles.IsChecked == true)
                dias.Add("Miércoles");

            if (ChkJueves.IsChecked == true)
                dias.Add("Jueves");

            if (ChkViernes.IsChecked == true)
                dias.Add("Viernes");

            if (ChkSabado.IsChecked == true)
                dias.Add("Sábado");

            return dias;
        }

        private static string ObtenerTextoCombo(ComboBox combo)
        {
            if (combo.SelectedItem is ComboBoxItem item)
                return item.Content?.ToString() ?? string.Empty;

            return combo.SelectedItem?.ToString() ?? string.Empty;
        }

        private static void SeleccionarComboPorTexto(ComboBox combo, string texto)
        {
            foreach (object item in combo.Items)
            {
                if (item is ComboBoxItem comboItem &&
                    comboItem.Content?.ToString() == texto)
                {
                    combo.SelectedItem = comboItem;
                    return;
                }
            }
        }
    }
}
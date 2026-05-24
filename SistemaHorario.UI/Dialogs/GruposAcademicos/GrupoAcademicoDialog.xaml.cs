using SistemaHorario.UI.Models.UI;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.GruposAcademicos;

public partial class GrupoAcademicoDialog : Window
{
    private readonly ModoGrupoAcademicoDialog _modo;

    public GrupoAcademicoItem GrupoResultado { get; private set; }

    public GrupoAcademicoDialog(
        ModoGrupoAcademicoDialog modo,
        GrupoAcademicoItem? grupo)
    {
        InitializeComponent();

        _modo = modo;
        GrupoResultado = grupo?.Clonar() ?? new GrupoAcademicoItem();

        ConfigurarModo();
        CargarDatos();
        SeleccionarValoresPorDefecto();
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

        TxtTitulo.Text = "Ver grupo académico";
        BtnGuardar.Visibility = Visibility.Collapsed;
        //BtnCancelar.Content = "Cerrar";

        TxtNombreGrupo.IsEnabled = false;
        TxtCodigo.IsEnabled = false;
        CmbJornada.IsEnabled = false;
        CmbTipoGrupo.IsEnabled = false;
        CmbEstado.IsEnabled = false;
        TxtPlazas.IsEnabled = false;
        TxtSemestre.IsEnabled = false;
    }

    private void CargarDatos()
    {
        TxtNombreGrupo.Text = GrupoResultado.NombreGrupo;
        TxtCodigo.Text = GrupoResultado.Codigo;
        TxtPlazas.Text = GrupoResultado.PlazasDisponibles.ToString();
        TxtSemestre.Text = GrupoResultado.NumeroSemestre.ToString();

        SeleccionarComboPorTexto(CmbJornada, GrupoResultado.Jornada);
        SeleccionarComboPorTexto(CmbTipoGrupo, GrupoResultado.Tipo);
        SeleccionarComboPorTexto(CmbEstado, GrupoResultado.Estado);
    }

    private void SeleccionarValoresPorDefecto()
    {
        if (_modo != ModoGrupoAcademicoDialog.Crear)
        {
            return;
        }

        if (CmbJornada.SelectedItem == null)
        {
            CmbJornada.SelectedIndex = 0;
        }

        if (CmbTipoGrupo.SelectedItem == null)
        {
            CmbTipoGrupo.SelectedIndex = 0;
        }

        if (CmbEstado.SelectedItem == null)
        {
            CmbEstado.SelectedIndex = 0;
        }

        if (string.IsNullOrWhiteSpace(TxtSemestre.Text) || TxtSemestre.Text == "0")
        {
            TxtSemestre.Text = "1";
        }
    }

    private void BtnGuardar_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidarFormulario())
        {
            return;
        }

        int cantidadEstudiantes = int.Parse(TxtPlazas.Text.Trim());
        int numeroSemestre = int.Parse(TxtSemestre.Text.Trim());

        GrupoResultado.NombreGrupo = TxtNombreGrupo.Text.Trim();
        GrupoResultado.Codigo = TxtCodigo.Text.Trim();
        GrupoResultado.Jornada = ObtenerTextoCombo(CmbJornada);
        GrupoResultado.Tipo = ObtenerTextoCombo(CmbTipoGrupo);
        GrupoResultado.Estado = ObtenerTextoCombo(CmbEstado);
        GrupoResultado.NumeroSemestre = numeroSemestre;
        GrupoResultado.EstudiantesActuales = cantidadEstudiantes;
        GrupoResultado.PlazasDisponibles = cantidadEstudiantes;

        DialogResult = true;
        Close();
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

        if (CmbTipoGrupo.SelectedItem == null)
        {
            MessageBox.Show("⚠ Selecciona el tipo de grupo.");
            return false;
        }

        if (CmbEstado.SelectedItem == null)
        {
            MessageBox.Show("⚠ Selecciona el estado.");
            return false;
        }

        if (!int.TryParse(TxtPlazas.Text.Trim(), out int cantidadEstudiantes) ||
            cantidadEstudiantes <= 0)
        {
            MessageBox.Show("⚠ Ingresa una cantidad de estudiantes válida.");
            return false;
        }

        if (!int.TryParse(TxtSemestre.Text.Trim(), out int numeroSemestre) ||
            numeroSemestre <= 0)
        {
            MessageBox.Show("⚠ Ingresa un número de semestre válido.");
            return false;
        }

        return true;
    }

    private static string ObtenerTextoCombo(ComboBox combo)
    {
        if (combo.SelectedItem is ComboBoxItem item)
        {
            return item.Content?.ToString() ?? string.Empty;
        }

        return combo.SelectedItem?.ToString() ?? string.Empty;
    }

    private static void SeleccionarComboPorTexto(ComboBox combo, string texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            return;
        }

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

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
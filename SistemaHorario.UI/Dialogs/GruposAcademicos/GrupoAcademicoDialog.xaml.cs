using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly ObservableCollection<string> _materiasBase;
        private readonly ObservableCollection<PlanAcademicoOption> _planesDisponibles;

        public GrupoAcademicoItem GrupoResultado { get; private set; }

        public GrupoAcademicoDialog(
            ModoGrupoAcademicoDialog modo,
            GrupoAcademicoItem? grupo,
            ObservableCollection<string> materiasDisponibles,
            ObservableCollection<PlanAcademicoOption> planesDisponibles)
        {
            InitializeComponent();

            _modo = modo;
            _materiasBase = materiasDisponibles;
            _planesDisponibles = planesDisponibles;
            GrupoResultado = grupo?.Clonar() ?? new GrupoAcademicoItem();

            CmbPlanAcademico.ItemsSource = _planesDisponibles;
            CmbMateria.ItemsSource = _materiasBase;

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

            CmbJornada.IsEnabled = false;
            CmbMateria.IsEnabled = false;
            CmbEstado.IsEnabled = false;
            CmbPlanAcademico.IsEnabled = false;
            CmbSemestre.IsEnabled = false;

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

            SeleccionarComboPorTexto(CmbJornada, GrupoResultado.Jornada);
            SeleccionarComboPorTexto(CmbEstado, GrupoResultado.Estado);

            SeleccionarPlanAcademico();
            ActualizarSemestresDisponibles();
            SeleccionarSemestre();
            ActualizarMateriasDisponibles();

            if (!string.IsNullOrWhiteSpace(GrupoResultado.Materia))
            {
                CmbMateria.SelectedItem = GrupoResultado.Materia;
            }

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
            GrupoResultado.IdPlanAcademico =
                (CmbPlanAcademico.SelectedItem as PlanAcademicoOption)?.IdPlanAcademico ?? 0;
            GrupoResultado.NumeroSemestre =
                CmbSemestre.SelectedItem is int semestre ? semestre : 0;
            GrupoResultado.Dias = ObtenerDiasSeleccionados();

            GrupoResultado.Tipo = GrupoResultado.Jornada == "Nocturna"
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

            if (CmbPlanAcademico.SelectedItem is not PlanAcademicoOption)
            {
                MessageBox.Show("⚠ Selecciona el plan académico del grupo.");
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

            if (CmbSemestre.SelectedItem is not int semestre || semestre <= 0)
            {
                MessageBox.Show("⚠ Selecciona un semestre válido del plan académico.");
                return false;
            }

            if (ObtenerDiasSeleccionados().Count == 0)
            {
                MessageBox.Show("⚠ Selecciona al menos un día.");
                return false;
            }

            return true;
        }

        private void CmbPlanAcademico_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            ActualizarSemestresDisponibles();
            ActualizarMateriasDisponibles();
        }

        private void CmbSemestre_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            ActualizarMateriasDisponibles();
        }

        private void SeleccionarPlanAcademico()
        {
            PlanAcademicoOption? plan = _planesDisponibles.FirstOrDefault(
                p => p.IdPlanAcademico == GrupoResultado.IdPlanAcademico);

            CmbPlanAcademico.SelectedItem = plan ?? _planesDisponibles.FirstOrDefault();
        }

        private void ActualizarSemestresDisponibles()
        {
            int? semestreActual = CmbSemestre.SelectedItem as int?;
            PlanAcademicoOption? plan = CmbPlanAcademico.SelectedItem as PlanAcademicoOption;
            CmbSemestre.ItemsSource = plan?.Semestres ?? new List<int>();

            if (plan == null || plan.Semestres.Count == 0)
            {
                CmbSemestre.SelectedItem = null;
                return;
            }

            if (GrupoResultado.NumeroSemestre > 0 &&
                plan.Semestres.Contains(GrupoResultado.NumeroSemestre))
            {
                CmbSemestre.SelectedItem = GrupoResultado.NumeroSemestre;
                return;
            }

            if (semestreActual.HasValue && plan.Semestres.Contains(semestreActual.Value))
            {
                CmbSemestre.SelectedItem = semestreActual.Value;
                return;
            }

            CmbSemestre.SelectedItem = plan.Semestres.First();
        }

        private void SeleccionarSemestre()
        {
            if (CmbPlanAcademico.SelectedItem is not PlanAcademicoOption plan)
                return;

            if (GrupoResultado.NumeroSemestre > 0 &&
                plan.Semestres.Contains(GrupoResultado.NumeroSemestre))
            {
                CmbSemestre.SelectedItem = GrupoResultado.NumeroSemestre;
            }
        }

        private void ActualizarMateriasDisponibles()
        {
            string? materiaActual = CmbMateria.SelectedItem?.ToString();
            PlanAcademicoOption? plan = CmbPlanAcademico.SelectedItem as PlanAcademicoOption;
            int? semestre = CmbSemestre.SelectedItem as int?;

            List<string> materias = new();
            if (plan != null &&
                semestre.HasValue &&
                plan.MateriasPorSemestre.TryGetValue(semestre.Value, out List<string>? materiasPlan))
            {
                materias = materiasPlan;
            }

            if (materias.Count == 0)
            {
                materias = _materiasBase.ToList();
            }

            CmbMateria.ItemsSource = materias;

            if (!string.IsNullOrWhiteSpace(materiaActual) &&
                materias.Contains(materiaActual, StringComparer.OrdinalIgnoreCase))
            {
                CmbMateria.SelectedItem = materias.First(
                    m => string.Equals(m, materiaActual, StringComparison.OrdinalIgnoreCase));
                return;
            }

            if (!string.IsNullOrWhiteSpace(GrupoResultado.Materia) &&
                materias.Contains(GrupoResultado.Materia, StringComparer.OrdinalIgnoreCase))
            {
                CmbMateria.SelectedItem = materias.First(
                    m => string.Equals(m, GrupoResultado.Materia, StringComparison.OrdinalIgnoreCase));
                return;
            }

            CmbMateria.SelectedItem = materias.FirstOrDefault();
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

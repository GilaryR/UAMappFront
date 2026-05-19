using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Docentes;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SistemaHorario.UI.Dialogs.Docentes
{
	/// <summary>
	/// Ventana utilizada para crear, editar o ver docentes.
	/// 
	/// En este formulario también se asignan las materias del docente.
	/// Por ahora las materias se guardan como texto separado por comas,
	/// para mantener compatibilidad con la tabla actual de docentes.
	/// 
	/// Cuando se conecte backend, la lista de materias disponibles
	/// debe venir desde un endpoint de materias.
	/// </summary>
	public partial class AgregarEditarDocenteDialog : Window
	{
		private readonly AgregarEditarDocenteViewModel _viewModel;

		private readonly bool _soloLectura;

		/// <summary>
		/// Materias seleccionadas para el docente en este formulario.
		/// Al guardar, se convierten en texto separado por comas.
		/// </summary>
		private readonly ObservableCollection<string> _materiasSeleccionadas = new();

		/// <summary>
		/// Materias disponibles para asignar.
		/// Actualmente vienen desde DocentesMockStore.
		/// Después pueden venir desde backend.
		/// </summary>
		private readonly ObservableCollection<string> _materiasDisponibles = new();

		public AgregarEditarDocenteDialog()
		{
			InitializeComponent();

			_viewModel = new AgregarEditarDocenteViewModel();
			_soloLectura = false;

			ConfigurarModo();
			CargarDatos();
		}

		public AgregarEditarDocenteDialog(
			DocenteItem docente,
			bool soloLectura = false)
		{
			InitializeComponent();

			_viewModel = new AgregarEditarDocenteViewModel(docente);
			_soloLectura = soloLectura;

			ConfigurarModo();
			CargarDatos();
		}

		private void ConfigurarModo()
		{
			if (_soloLectura)
			{
				TxtTitulo.Text = "Detalle docente";
				TxtSubtitulo.Text = "Información general del docente";
				BtnGuardar.Visibility = Visibility.Collapsed;

				BloquearFormulario();
				return;
			}

			if (_viewModel.EsEdicion)
			{
				TxtTitulo.Text = "Editar docente";
				BtnGuardar.Content = "Guardar cambios";
			}
		}

		private void BloquearFormulario()
		{
			TxtNombreCompleto.IsReadOnly = true;
			TxtIdentificacion.IsReadOnly = true;
			TxtCorreo.IsReadOnly = true;

			CmbEstado.IsEnabled = false;
			CmbMateriaDisponible.IsEnabled = false;

			BtnAgregarMateria.Visibility = Visibility.Collapsed;
			BtnDisponibilidad.IsEnabled = false;
		}

		private void CargarDatos()
		{
			TxtNombreCompleto.Text = _viewModel.Docente.NombreCompleto;
			TxtIdentificacion.Text = _viewModel.Docente.Identificacion;
			TxtCorreo.Text = _viewModel.Docente.CorreoInstitucional;

			CargarMateriasDisponibles();
			CargarMateriasSeleccionadas(_viewModel.Docente.Materias);
			DibujarMateriasSeleccionadas();
			ActualizarComboMaterias();

			foreach (ComboBoxItem item in CmbEstado.Items)
			{
				if (item.Content?.ToString() == _viewModel.Docente.Estado)
				{
					CmbEstado.SelectedItem = item;
					break;
				}
			}
		}

		private void BtnDisponibilidad_Click(
			object sender,
			RoutedEventArgs e)
		{
			DisponibilidadDocenteDialog dialog =
				new(_viewModel.Disponibilidad)
				{
					Owner = this
				};

			if (dialog.ShowDialog() != true)
				return;

			_viewModel.Disponibilidad = dialog.DisponibilidadResultado;
		}

		private async void BtnGuardar_Click(
    object sender,
    RoutedEventArgs e)
{
    if (!FormularioEsValido())
        return;

    _viewModel.Docente.NombreCompleto = TxtNombreCompleto.Text.Trim();
    _viewModel.Docente.Identificacion = TxtIdentificacion.Text.Trim();
    _viewModel.Docente.CorreoInstitucional = TxtCorreo.Text.Trim();

    _viewModel.Docente.Materias =
        string.Join(", ", _materiasSeleccionadas);

    if (CmbEstado.SelectedItem is ComboBoxItem item)
    {
        _viewModel.Docente.Estado =
            item.Content?.ToString() ?? "Activo";
    }

    var api = new SistemaHorario.UI.Services.DocentesApiService();
    SistemaHorarios.Application.Common.ApiResponse<string> resp;

    if (_viewModel.EsEdicion)
        resp = await api.ActualizarDocenteAsync(_viewModel.Docente);
    else
        resp = await api.CrearDocenteAsync(_viewModel.Docente);

    if (!resp.Success)
    {
        MessageBox.Show(
            "Error al guardar: " + resp.Message,
            "Error",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
        return;
    }

    MensajeExitoDialog exito =
        new(_viewModel.EsEdicion
            ? "Docente actualizado"
            : "Docente creado")
        {
            Owner = this
        };

    exito.ShowDialog();

    DialogResult = true;
    Close();
}

		private bool FormularioEsValido()
		{
			if (string.IsNullOrWhiteSpace(TxtNombreCompleto.Text))
			{
				MessageBox.Show(
					"El nombre completo es obligatorio.",
					"⚠ Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return false;
			}

			if (string.IsNullOrWhiteSpace(TxtIdentificacion.Text))
			{
				MessageBox.Show(
					"La identificación es obligatoria.",
					"⚠ Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return false;
			}

			if (string.IsNullOrWhiteSpace(TxtCorreo.Text))
			{
				MessageBox.Show(
					"El correo institucional es obligatorio.",
					"⚠ Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return false;
			}

			if (_materiasSeleccionadas.Count == 0)
			{
				MessageBox.Show(
					"⚠ Debes agregar al menos una materia.",
					"Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return false;
			}

			return true;
		}

		private void BtnAgregarMateria_Click(object sender, RoutedEventArgs e)
		{
			if (CmbMateriaDisponible.SelectedItem is not string materia)
				return;

			if (_materiasSeleccionadas.Contains(materia))
				return;

			_materiasSeleccionadas.Add(materia);

			DibujarMateriasSeleccionadas();
			ActualizarComboMaterias();
		}

		private void CargarMateriasDisponibles()
		{
			_materiasDisponibles.Clear();

			foreach (string materia in DocentesMockStore.ObtenerMateriasDisponibles())
			{
				_materiasDisponibles.Add(materia);
			}
		}

		private void CargarMateriasSeleccionadas(string materiasTexto)
		{
			_materiasSeleccionadas.Clear();

			if (string.IsNullOrWhiteSpace(materiasTexto))
				return;

			string[] materias = materiasTexto.Split(',');

			foreach (string materia in materias)
			{
				string materiaLimpia = materia.Trim();

				if (!string.IsNullOrWhiteSpace(materiaLimpia) &&
					!_materiasSeleccionadas.Contains(materiaLimpia))
				{
					_materiasSeleccionadas.Add(materiaLimpia);
				}
			}
		}

		private void ActualizarComboMaterias()
		{
			CmbMateriaDisponible.ItemsSource = null;

			List<string> disponibles = _materiasDisponibles
				.Where(materia => !_materiasSeleccionadas.Contains(materia))
				.ToList();

			CmbMateriaDisponible.ItemsSource = disponibles;

			if (disponibles.Count > 0)
				CmbMateriaDisponible.SelectedIndex = 0;
		}

		private void DibujarMateriasSeleccionadas()
		{
			PanelMateriasSeleccionadas.Children.Clear();

			foreach (string materia in _materiasSeleccionadas)
			{
				Border chip = new()
				{
					Background = new SolidColorBrush(Color.FromRgb(219, 234, 254)),
					CornerRadius = new CornerRadius(14),
					Padding = new Thickness(10, 5, 8, 5),
					Margin = new Thickness(0, 0, 8, 8)
				};

				StackPanel contenido = new()
				{
					Orientation = Orientation.Horizontal
				};

				TextBlock texto = new()
				{
					Text = materia,
					Foreground = new SolidColorBrush(Color.FromRgb(30, 64, 175)),
					FontWeight = FontWeights.SemiBold,
					VerticalAlignment = VerticalAlignment.Center
				};

				contenido.Children.Add(texto);

				if (!_soloLectura)
				{
					Button quitar = new()
					{
						Content = "x",
						Width = 20,
						Height = 20,
						Margin = new Thickness(8, 0, 0, 0),
						Background = Brushes.Transparent,
						BorderThickness = new Thickness(0),
						Foreground = new SolidColorBrush(Color.FromRgb(30, 64, 175)),
						FontWeight = FontWeights.Bold,
						Cursor = Cursors.Hand,
						Tag = materia
					};

					quitar.Click += BtnQuitarMateria_Click;

					contenido.Children.Add(quitar);
				}

				chip.Child = contenido;
				PanelMateriasSeleccionadas.Children.Add(chip);
			}
		}

		private void BtnQuitarMateria_Click(object sender, RoutedEventArgs e)
		{
			if (sender is not Button boton)
				return;

			if (boton.Tag is not string materia)
				return;

			_materiasSeleccionadas.Remove(materia);

			DibujarMateriasSeleccionadas();
			ActualizarComboMaterias();
		}

		private void BtnCancelar_Click(
			object sender,
			RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		private void BtnCerrar_Click(
			object sender,
			RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
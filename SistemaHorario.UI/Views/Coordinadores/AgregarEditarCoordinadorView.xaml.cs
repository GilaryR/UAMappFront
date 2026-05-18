using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Coordinadores;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Coordinadores
{
	/// <summary>
	/// Formulario visual para crear o editar coordinadores.
	///
	/// El rol se mantiene fijo como Coordinador.
	///
	/// TODO:
	/// Conectar con:
	/// POST /api/coordinadores
	/// PUT /api/coordinadores/{id}
	/// </summary>
	public partial class AgregarEditarCoordinadorView : UserControl
	{
		private readonly AgregarEditarCoordinadorViewModel _viewModel;

		public AgregarEditarCoordinadorView(
			CoordinadorItem? coordinador = null)
		{
			InitializeComponent();

			_viewModel = coordinador == null
				? new AgregarEditarCoordinadorViewModel()
				: new AgregarEditarCoordinadorViewModel(coordinador);

			CargarDatos();
		}

		private void CargarDatos()
		{
			TxtNombre.Text = _viewModel.Coordinador.NombreCompleto;
			TxtCedula.Text = _viewModel.Coordinador.Cedula;
			TxtCorreo.Text = _viewModel.Coordinador.CorreoInstitucional;
			TxtCelular.Text = _viewModel.Coordinador.Celular;
			TxtRol.Text = "Coordinador";

			foreach (ComboBoxItem item in CmbEstado.Items)
			{
				if (item.Content?.ToString() ==
					_viewModel.Coordinador.Estado)
				{
					CmbEstado.SelectedItem = item;
					break;
				}
			}

			if (_viewModel.EsEdicion)
			{
				TxtTitulo.Text = "Editar coordinador";
			}
		}

		private void BtnGuardar_Click(
			object sender,
			RoutedEventArgs e)
		{
			if (!FormularioEsValido())
				return;

			_viewModel.Coordinador.NombreCompleto =
				TxtNombre.Text.Trim();

			_viewModel.Coordinador.Cedula =
				TxtCedula.Text.Trim();

			_viewModel.Coordinador.CorreoInstitucional =
				TxtCorreo.Text.Trim();

			_viewModel.Coordinador.Celular =
				TxtCelular.Text.Trim();

			_viewModel.Coordinador.Rol = "Coordinador";

			if (CmbEstado.SelectedItem is ComboBoxItem item)
			{
				_viewModel.Coordinador.Estado =
					item.Content?.ToString() ?? "Activo";
			}

			if (!_viewModel.EsEdicion)
			{
				CoordinadoresMockStore.Crear(
					_viewModel.Coordinador);
			}

			MensajeExitoDialog dialog = new(
				_viewModel.EsEdicion
					? "Coordinador actualizado"
					: "Coordinador creado")
			{
				Owner = Window.GetWindow(this)
			};

			dialog.ShowDialog();

			VolverAPrincipal();
		}

		private bool FormularioEsValido()
		{
			if (string.IsNullOrWhiteSpace(TxtNombre.Text))
			{
				MessageBox.Show(
					"El nombre completo es obligatorio.",
					"⚠ Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return false;
			}

			if (string.IsNullOrWhiteSpace(TxtCedula.Text))
			{
				MessageBox.Show(
					"La cédula es obligatoria.",
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

			return true;
		}

		private void VolverAPrincipal()
		{
			ContentControl? contentArea = BuscarContentArea();

			if (contentArea == null)
				return;

			contentArea.Content = new CoordinadoresView();
		}

		private ContentControl? BuscarContentArea()
		{
			DependencyObject? actual = this;

			while (actual != null)
			{
				if (actual is ContentControl content &&
					content.Name == "ContentArea")
				{
					return content;
				}

				actual =
					System.Windows.Media.VisualTreeHelper.GetParent(actual);
			}

			return null;
		}
	}
}
using SistemaHorario.UI.Controls;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Coordinadores;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Coordinadores
{
	/// <summary>
	/// Vista principal del módulo Coordinadores.
	///
	/// Permite:
	/// - listar coordinadores,
	/// - buscar por nombre, cédula o correo,
	/// - filtrar por estado,
	/// - crear coordinadores después de verificar credenciales,
	/// - editar coordinadores,
	/// - eliminar coordinadores.
	///
	/// Consume el backend de usuarios para coordinadores.
	///
	/// Endpoints utilizados:
	/// GET /api/usuarios
	/// POST /api/usuarios
	/// PUT /api/usuarios/{id}
	/// DELETE /api/usuarios/{id}
	/// </summary>
	public partial class CoordinadoresView : UserControl
	{
		private readonly CoordinadoresApiService _api = new();

		/// <summary>
		/// ViewModel principal de coordinadores.
		/// </summary>
		private readonly CoordinadoresViewModel _viewModel;

		/// <summary>
		/// Constructor principal.
		/// </summary>
		public CoordinadoresView()
		{
			InitializeComponent();

			_viewModel = new CoordinadoresViewModel();

			ConfigurarTabla();

			Loaded += CoordinadoresView_Loaded;
		}

		private async void CoordinadoresView_Loaded(object sender, RoutedEventArgs e)
		{
			await _viewModel.CargarDatosAsync();

			if (!string.IsNullOrWhiteSpace(_viewModel.MensajeEstado))
			{
				MessageBox.Show(_viewModel.MensajeEstado, "Error al cargar coordinadores", MessageBoxButton.OK, MessageBoxImage.Error);
			}

			CargarDatos();
		}

		/// <summary>
		/// Configura columnas y acciones de la tabla reutilizable.
		/// </summary>
		private void ConfigurarTabla()
		{
			TablaCoordinadores.ConfigurarColumnas(
			[
				new TableColumnDefinition
				{
					Header = "Nombre",
					Binding = "NombreCompleto",
					Width = 2
				},

				new TableColumnDefinition
				{
					Header = "Cédula",
					Binding = "Cedula",
					Width = 1.2
				},

				new TableColumnDefinition
				{
					Header = "Correo electrónico",
					Binding = "CorreoInstitucional",
					Width = 2
				},

				new TableColumnDefinition
				{
					Header = "Rol",
					Binding = "Rol",
					Width = 1
				},

				new TableColumnDefinition
				{
					Header = "Estado",
					Binding = "Estado",
					Width = 1
				},

				new TableColumnDefinition
				{
					Header = "Celular",
					Binding = "Celular",
					Width = 1.2
				}
			]);

			TablaCoordinadores.ConfigurarAcciones(
			[
				new TableActionDefinition
				{
					Nombre = "editar",
					Texto = "✏"
				},

				new TableActionDefinition
				{
					Nombre = "estado",
					Texto = "↻"
				}
            ]);

			TablaCoordinadores.AccionEjecutada +=
				TablaCoordinadores_AccionEjecutada;
		}

		/// <summary>
		/// Carga la información actual en la tabla.
		/// </summary>
		private void CargarDatos()
		{
			TablaCoordinadores.CargarDatos(
				_viewModel.Coordinadores);
		}

		/// <summary>
		/// Ejecuta búsqueda automática cuando cambia el texto.
		/// </summary>
		private void SearchCoordinadores_BusquedaCambiada(
			object sender,
			RoutedEventArgs e)
		{
			AplicarFiltros();
		}

		/// <summary>
		/// Aplica filtros al presionar el botón Filtrar.
		/// </summary>
		private void BtnFiltrar_Click(
			object sender,
			RoutedEventArgs e)
		{
			AplicarFiltros();
		}

		/// <summary>
		/// Limpia búsqueda y filtros.
		/// </summary>
		private void BtnLimpiar_Click(
			object sender,
			RoutedEventArgs e)
		{
			SearchCoordinadores.Limpiar();

			CmbBuscarPor.SelectedIndex = 0;
			CmbEstado.SelectedIndex = 0;

			AplicarFiltros();
		}

		/// <summary>
		/// Aplica búsqueda por texto, tipo de búsqueda y estado.
		/// </summary>
		private void AplicarFiltros()
		{
			_viewModel.Filtrar(
				SearchCoordinadores.TextoBusqueda,
				ObtenerTextoCombo(CmbBuscarPor),
				ObtenerTextoCombo(CmbEstado));

			CargarDatos();
		}

		/// <summary>
		/// Obtiene el texto seleccionado de un ComboBox.
		/// </summary>
		private static string ObtenerTextoCombo(ComboBox combo)
		{
			if (combo.SelectedItem is ComboBoxItem item)
				return item.Content?.ToString() ?? string.Empty;

			return string.Empty;
		}

		/// <summary>
		/// Maneja acciones ejecutadas desde TablaPaginada.
		/// </summary>
		private void TablaCoordinadores_AccionEjecutada(
			object? sender,
			TableActionEventArgs e)
		{
			if (e.Fila is not CoordinadorItem coordinador)
				return;

			switch (e.Accion.ToLower())
			{
				case "editar":
					EditarCoordinador(coordinador);
					break;

                case "estado":
                    CambiarEstadoCoordinador(coordinador);
                    break;
            }
		}

        /// <summary>
        /// Inicia el flujo de creación:
        /// primero verifica credenciales y luego abre formulario.
        /// </summary>
        private void BtnAgregar_Click(
			object sender,
			RoutedEventArgs e)
        {
            NavegarAFormulario();
        }

        /// <summary>
        /// Abre formulario de edición.
        /// </summary>
        private void EditarCoordinador(
			CoordinadorItem coordinador)
		{
			NavegarAFormulario(coordinador);
		}

        private async void CambiarEstadoCoordinador(
            CoordinadorItem coordinador)
        {
            bool estaActivo =
                coordinador.Estado.Equals(
                    "Activo",
                    StringComparison.OrdinalIgnoreCase);

            if (estaActivo)
            {
                EliminarConfirmacionDialog dialog =
                    new(
                        "DESACTIVAR",
                        $"¿Desea desactivar el coordinador {coordinador.NombreCompleto}?\nDebes escribir la palabra desactivar.",
                        "desactivar")
                    {
                        Owner = Window.GetWindow(this)
                    };

                if (dialog.ShowDialog() != true)
                {
                    return;
                }
            }
            else
            {
                ConfirmacionDialog dialog =
                    new(
                        "Reactivar coordinador",
                        $"¿Está seguro que desea reactivar al coordinador {coordinador.NombreCompleto}?")
                    {
                        Owner = Window.GetWindow(this)
                    };

                if (dialog.ShowDialog() != true)
                {
                    return;
                }
            }

            bool ok =
                await _viewModel.EliminarCoordinadorAsync(coordinador);

            if (!ok)
            {
                MessageBox.Show(
                    _viewModel.MensajeEstado,
                    "Error al cambiar estado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            CargarDatos();

            MensajeExitoDialog exito =
                new(
                    estaActivo
                        ? "Coordinador desactivado correctamente."
                        : "Coordinador reactivado correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        /// <summary>
        /// Navega al formulario de crear o editar coordinador.
        /// </summary>
        private void NavegarAFormulario(
			CoordinadorItem? coordinador = null)
		{
			ContentControl? contentArea =
				BuscarContentArea();

			if (contentArea == null)
				return;

			contentArea.Content =
				new AgregarEditarCoordinadorView(
					coordinador);
		}

		/// <summary>
		/// Busca el ContentArea del MainShell para navegar internamente.
		/// </summary>
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

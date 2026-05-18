using SistemaHorario.UI.Controls;
using SistemaHorario.UI.Dialogs.Docentes;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Docentes;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Docentes
{
	/// <summary>
	/// Vista principal del módulo Docentes.
	///
	/// Esta vista permite administrar los docentes registrados
	/// dentro del sistema académico.
	///
	/// Funcionalidades principales:
	/// - Listar docentes.
	/// - Buscar docentes por nombre, identificación o correo.
	/// - Filtrar docentes por estado.
	/// - Crear nuevos docentes.
	/// - Editar información existente.
	/// - Visualizar detalles del docente.
	/// - Eliminar docentes.
	///
	/// La vista interactúa directamente con:
	/// - DocentesViewModel.
	/// - Tabla reutilizable personalizada.
	/// - Diálogos compartidos del sistema.
	///
	/// TODO:
	/// Reemplazar DocentesMockStore por endpoints reales.
	/// </summary>
	public partial class DocentesView : UserControl
	{
		/// <summary>
		/// Instancia principal del ViewModel utilizado por la vista.
		///
		/// Contiene:
		/// - Lista de docentes.
		/// - Métodos de búsqueda.
		/// - Métodos de filtrado.
		/// - Resumen estadístico.
		/// </summary>
		private readonly DocentesViewModel _viewModel;

		/// <summary>
		/// Constructor principal de la vista.
		///
		/// Inicializa:
		/// - Componentes visuales.
		/// - ViewModel.
		/// - Configuración de la tabla.
		/// - Carga inicial de datos.
		/// </summary>
		public DocentesView()
		{
			InitializeComponent();

			_viewModel = new DocentesViewModel();

			ConfigurarTabla();
			CargarDatos();
		}

		/// <summary>
		/// Configura las columnas y acciones de la tabla de docentes.
		///
		/// Define:
		/// - Columnas visibles.
		/// - Bindings de datos.
		/// - Acciones disponibles por fila.
		///
		/// También registra el evento de ejecución de acciones.
		/// </summary>
		private void ConfigurarTabla()
		{
			TablaDocentes.ConfigurarColumnas(
			[
				new TableColumnDefinition
				{
					Header = "Nombre",
					Binding = "NombreCompleto",
					Width = 2
				},
				new TableColumnDefinition
				{
					Header = "Identificación",
					Binding = "Identificacion",
					Width = 1
				},
				new TableColumnDefinition
				{
					Header = "Correo institucional",
					Binding = "CorreoInstitucional",
					Width = 2
				},
				new TableColumnDefinition
				{
					Header = "Materias",
					Binding = "Materias",
					Width = 1.5
				},
				new TableColumnDefinition
				{
					Header = "Estado",
					Binding = "Estado",
					Width = 1
				}
			]);

			TablaDocentes.ConfigurarAcciones(
			[
				new TableActionDefinition
				{
					Nombre = "ver",
					Texto = "👁"
				},
				new TableActionDefinition
				{
					Nombre = "editar",
					Texto = "✏"
				},
				new TableActionDefinition
				{
					Nombre = "eliminar",
					Texto = "🗑"
				}
			]);

			TablaDocentes.AccionEjecutada += TablaDocentes_AccionEjecutada;
		}

		/// <summary>
		/// Carga los datos de docentes en la tabla.
		///
		/// Realiza:
		/// - Obtención de datos desde el ViewModel.
		/// - Carga de filas en la tabla visual.
		/// - Actualización del resumen estadístico.
		/// </summary>
		private void CargarDatos()
		{
			_viewModel.CargarDatos();

			TablaDocentes.CargarDatos(_viewModel.Docentes);

			ActualizarResumen();
		}

		/// <summary>
		/// Actualiza los indicadores visuales del resumen.
		///
		/// Muestra:
		/// - Total de docentes.
		/// - Docentes activos.
		/// - Docentes inactivos.
		/// - Docentes disponibles.
		/// </summary>
		private void ActualizarResumen()
		{
			TxtTotalDocentes.Text = _viewModel.Resumen.TotalDocentes.ToString();
			TxtActivos.Text = _viewModel.Resumen.Activos.ToString();
			TxtInactivos.Text = _viewModel.Resumen.Inactivos.ToString();
			TxtDisponibles.Text = _viewModel.Resumen.Disponibles.ToString();
		}

		/// <summary>
		/// Aplica los filtros seleccionados por el usuario.
		///
		/// Obtiene:
		/// - Texto de búsqueda.
		/// - Estado seleccionado.
		///
		/// Luego:
		/// - Filtra la lista.
		/// - Recarga la tabla.
		/// - Actualiza el resumen.
		/// </summary>
		private void AplicarFiltros()
		{
			_viewModel.Filtrar(
				SearchDocentes.TextoBusqueda,
				ObtenerEstadoSeleccionado());

			TablaDocentes.CargarDatos(_viewModel.Docentes);

			ActualizarResumen();
		}

		/// <summary>
		/// Obtiene el estado actualmente seleccionado
		/// en el ComboBox de filtros.
		///
		/// Retorna:
		/// - "Activo"
		/// - "Inactivo"
		/// - "Todos"
		/// </summary>
		/// <returns>
		/// Estado seleccionado como texto.
		/// </returns>
		private string ObtenerEstadoSeleccionado()
		{
			if (CmbEstadoFiltro.SelectedItem is ComboBoxItem item)
				return item.Content?.ToString() ?? "Todos";

			return "Todos";
		}

		/// <summary>
		/// Evento ejecutado cuando cambia el texto
		/// del buscador de docentes.
		///
		/// Aplica automáticamente los filtros.
		/// </summary>
		private void SearchDocentes_BusquedaCambiada(
			object sender,
			RoutedEventArgs e)
		{
			AplicarFiltros();
		}

		/// <summary>
		/// Evento ejecutado cuando cambia el estado
		/// seleccionado en el filtro.
		///
		/// Evita ejecutar filtros antes de que
		/// la vista termine de cargarse.
		/// </summary>
		private void CmbEstadoFiltro_SelectionChanged(
			object sender,
			SelectionChangedEventArgs e)
		{
			if (!IsLoaded)
				return;

			AplicarFiltros();
		}

		/// <summary>
		/// Evento del botón Aplicar filtros.
		///
		/// Ejecuta manualmente el filtrado.
		/// </summary>
		private void BtnAplicarFiltros_Click(
			object sender,
			RoutedEventArgs e)
		{
			AplicarFiltros();
		}

		/// <summary>
		/// Evento del botón Limpiar filtros.
		///
		/// Limpia:
		/// - Texto de búsqueda.
		/// - Estado seleccionado.
		///
		/// Luego vuelve a cargar todos los datos.
		/// </summary>
		private void BtnLimpiarFiltros_Click(
			object sender,
			RoutedEventArgs e)
		{
			SearchDocentes.Limpiar();

			CmbEstadoFiltro.SelectedIndex = 0;

			AplicarFiltros();
		}

		/// <summary>
		/// Evento del botón Agregar docente.
		///
		/// Abre el formulario en modo creación.
		/// </summary>
		private void BtnAgregarDocente_Click(
			object sender,
			RoutedEventArgs e)
		{
			AbrirFormularioDocente();
		}

		/// <summary>
		/// Evento ejecutado cuando el usuario interactúa
		/// con alguna acción de la tabla.
		///
		/// Acciones disponibles:
		/// - Ver.
		/// - Editar.
		/// - Eliminar.
		/// </summary>
		private void TablaDocentes_AccionEjecutada(
			object? sender,
			TableActionEventArgs e)
		{
			if (e.Fila is not DocenteItem docente)
				return;

			switch (e.Accion.ToLower())
			{
				case "ver":
					VerDocente(docente);
					break;

				case "editar":
					AbrirFormularioDocente(docente);
					break;

				case "eliminar":
					EliminarDocente(docente);
					break;
			}
		}

		/// <summary>
		/// Abre el formulario del docente en modo visualización.
		///
		/// El usuario puede consultar la información
		/// sin modificarla.
		/// </summary>
		/// <param name="docente">
		/// Docente que será visualizado.
		/// </param>
		private void VerDocente(DocenteItem docente)
		{
			AgregarEditarDocenteDialog dialog = new(docente, true)
			{
				Owner = Window.GetWindow(this)
			};

			dialog.ShowDialog();
		}

		/// <summary>
		/// Abre el formulario de docente.
		///
		/// Si no se envía un docente:
		/// - Se abre en modo creación.
		///
		/// Si se envía un docente:
		/// - Se abre en modo edición.
		///
		/// Después de guardar correctamente,
		/// recarga la información de la tabla.
		/// </summary>
		/// <param name="docente">
		/// Docente a editar.
		/// Puede ser null.
		/// </param>
		private void AbrirFormularioDocente(
			DocenteItem? docente = null)
		{
			AgregarEditarDocenteDialog dialog =
				docente == null
					? new AgregarEditarDocenteDialog()
					: new AgregarEditarDocenteDialog(docente);

			dialog.Owner = Window.GetWindow(this);

			bool? resultado = dialog.ShowDialog();

			if (resultado != true)
				return;

			CargarDatos();
		}

		/// <summary>
		/// Elimina un docente del sistema.
		///
		/// Flujo:
		/// 1. Solicita confirmación.
		/// 2. Elimina el docente.
		/// 3. Recarga la tabla.
		/// 4. Muestra mensaje de éxito.
		/// </summary>
		/// <param name="docente">
		/// Docente que será eliminado.
		/// </param>
		private void EliminarDocente(DocenteItem docente)
		{
			EliminarConfirmacionDialog dialog =
				new($"¿Deseas eliminar al docente {docente.NombreCompleto}?\nDebes escribir la palabra eliminar.")
				{
					Owner = Window.GetWindow(this)
				};

			if (dialog.ShowDialog() != true)
				return;

			DocentesMockStore.EliminarDocente(docente);

			CargarDatos();

			MensajeExitoDialog exito =
				new("Docente eliminado")
				{
					Owner = Window.GetWindow(this)
				};

			exito.ShowDialog();
		}
	}
}
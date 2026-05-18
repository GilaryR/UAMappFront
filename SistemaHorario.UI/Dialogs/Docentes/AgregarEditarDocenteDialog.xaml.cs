using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Librerías WPF utilizadas para:
 * - Controles visuales
 * - Eventos
 * - Navegación
 * - Estilos
 * - Componentes gráficos
 */
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

/*
 * Referencias internas del sistema:
 * - Dialogs compartidos
 * - Modelos UI
 * - ViewModels
 */
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Docentes;

using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.Docentes
{
	/// <summary>
	/// Ventana modal utilizada para:
	/// 
	/// - Crear docentes nuevos
	/// - Editar docentes existentes
	/// - Visualizar información en modo lectura
	/// 
	/// Funcionalidades principales:
	/// - Gestión de información básica del docente
	/// - Configuración de disponibilidad horaria
	/// - Validaciones de formulario
	/// - Persistencia temporal mediante MockStore
	/// 
	/// TODO:
	/// Conectar operaciones de guardado con:
	/// - POST /api/Docentes
	/// - PUT /api/Docentes
	/// </summary>
	public partial class AgregarEditarDocenteDialog : Window
	{
		/// <summary>
		/// ViewModel principal del diálogo.
		/// 
		/// Contiene:
		/// - Datos del docente
		/// - Disponibilidad
		/// - Estado de edición
		/// </summary>
		private readonly AgregarEditarDocenteViewModel _viewModel;

		/// <summary>
		/// Indica si la ventana se encuentra en modo solo lectura.
		/// 
		/// TRUE:
		/// - No permite editar campos
		/// - Oculta botón guardar
		/// 
		/// FALSE:
		/// - Permite edición completa
		/// </summary>
		private readonly bool _soloLectura;

		/// <summary>
		/// Constructor utilizado para:
		/// - Crear un nuevo docente
		/// 
		/// Inicializa:
		/// - Componentes visuales
		/// - ViewModel vacío
		/// - Configuración inicial
		/// - Carga de datos
		/// </summary>
		public AgregarEditarDocenteDialog()
		{
			InitializeComponent();

			_viewModel = new AgregarEditarDocenteViewModel();
			_soloLectura = false;

			ConfigurarModo();
			CargarDatos();
		}

		/// <summary>
		/// Constructor utilizado para:
		/// - Editar un docente existente
		/// - Visualizar detalle del docente
		/// 
		/// <param name="docente">
		/// Información del docente a cargar
		/// </param>
		/// 
		/// <param name="soloLectura">
		/// TRUE  = modo visualización
		/// FALSE = modo edición
		/// </param>
		/// </summary>
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

		/// <summary>
		/// Configura el comportamiento visual de la ventana
		/// dependiendo del modo actual:
		/// 
		/// - Modo lectura
		/// - Modo edición
		/// - Modo creación
		/// </summary>
		private void ConfigurarModo()
		{
			// Configuración de modo solo lectura
			if (_soloLectura)
			{
				TxtTitulo.Text = "Detalle docente";
				TxtSubtitulo.Text = "Información general del docente";
				BtnGuardar.Visibility = Visibility.Collapsed;

				BloquearFormulario();
				return;
			}

			// Configuración de modo edición
			if (_viewModel.EsEdicion)
			{
				TxtTitulo.Text = "Editar docente";
				BtnGuardar.Content = "Guardar cambios";
			}
		}

		/// <summary>
		/// Deshabilita todos los controles editables
		/// del formulario para evitar modificaciones.
		/// 
		/// Utilizado en:
		/// - Modo visualización
		/// </summary>
		private void BloquearFormulario()
		{
			TxtNombreCompleto.IsReadOnly = true;
			TxtIdentificacion.IsReadOnly = true;
			TxtCorreo.IsReadOnly = true;
			TxtMaterias.IsReadOnly = true;

			CmbEstado.IsEnabled = false;
			BtnDisponibilidad.IsEnabled = false;
		}

		/// <summary>
		/// Carga los datos del docente en los controles visuales.
		/// 
		/// Realiza:
		/// - Asignación de textos
		/// - Selección del estado actual
		/// </summary>
		private void CargarDatos()
		{
			TxtNombreCompleto.Text = _viewModel.Docente.NombreCompleto;
			TxtIdentificacion.Text = _viewModel.Docente.Identificacion;
			TxtCorreo.Text = _viewModel.Docente.CorreoInstitucional;
			TxtMaterias.Text = _viewModel.Docente.Materias;

			// Selecciona el estado actual del docente
			foreach (ComboBoxItem item in CmbEstado.Items)
			{
				if (item.Content?.ToString() == _viewModel.Docente.Estado)
				{
					CmbEstado.SelectedItem = item;
					break;
				}
			}
		}

		/// <summary>
		/// Evento ejecutado al presionar el botón
		/// "Disponibilidad".
		/// 
		/// Abre el diálogo de configuración horaria
		/// del docente.
		/// 
		/// Si el usuario confirma:
		/// - Se actualiza la disponibilidad
		/// </summary>
		private void BtnDisponibilidad_Click(
			object sender,
			RoutedEventArgs e)
		{
			DisponibilidadDocenteDialog dialog =
				new(_viewModel.Disponibilidad)
				{
					Owner = this
				};

			// Si el usuario cancela, no continuar
			if (dialog.ShowDialog() != true)
				return;

			// Guardar disponibilidad seleccionada
			_viewModel.Disponibilidad = dialog.DisponibilidadResultado;
		}

		/// <summary>
		/// Evento ejecutado al presionar el botón Guardar.
		/// 
		/// Flujo:
		/// 1. Validar formulario
		/// 2. Actualizar modelo
		/// 3. Crear o editar docente
		/// 4. Guardar disponibilidad
		/// 5. Mostrar mensaje de éxito
		/// 6. Cerrar ventana
		/// </summary>
		private void BtnGuardar_Click(
			object sender,
			RoutedEventArgs e)
		{
			// Validación inicial
			if (!FormularioEsValido())
				return;

			// Actualizar información desde controles UI
			_viewModel.Docente.NombreCompleto = TxtNombreCompleto.Text.Trim();
			_viewModel.Docente.Identificacion = TxtIdentificacion.Text.Trim();
			_viewModel.Docente.CorreoInstitucional = TxtCorreo.Text.Trim();
			_viewModel.Docente.Materias = TxtMaterias.Text.Trim();

			// Obtener estado seleccionado
			if (CmbEstado.SelectedItem is ComboBoxItem item)
			{
				_viewModel.Docente.Estado =
					item.Content?.ToString() ?? "Activo";
			}

			// Actualizar docente existente
			if (_viewModel.EsEdicion)
			{
				DocentesMockStore.ActualizarDocente(_viewModel.Docente);
			}
			else
			{
				// Crear nuevo docente
				DocentesMockStore.CrearDocente(_viewModel.Docente);
			}

			// Guardar disponibilidad del docente
			DocentesMockStore.GuardarDisponibilidad(
				_viewModel.Docente.IdDocente,
				_viewModel.Disponibilidad);

			// Mostrar mensaje de confirmación
			MensajeExitoDialog exito =
				new(_viewModel.EsEdicion
					? "Docente actualizado"
					: "Docente creado")
				{
					Owner = this
				};

			exito.ShowDialog();

			// Finalizar diálogo exitosamente
			DialogResult = true;
			Close();
		}

		/// <summary>
		/// Realiza validaciones básicas del formulario.
		/// 
		/// Validaciones:
		/// - Nombre obligatorio
		/// - Identificación obligatoria
		/// - Correo obligatorio
		/// 
		/// <returns>
		/// TRUE  = formulario válido
		/// FALSE = existen errores
		/// </returns>
		private bool FormularioEsValido()
		{
			// Validar nombre completo
			if (string.IsNullOrWhiteSpace(TxtNombreCompleto.Text))
			{
				MessageBox.Show(
					"El nombre completo es obligatorio.",
					"⚠ Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return false;
			}

			// Validar identificación
			if (string.IsNullOrWhiteSpace(TxtIdentificacion.Text))
			{
				MessageBox.Show(
					"La identificación es obligatoria.",
					"⚠ Validación",
					MessageBoxButton.OK,
					MessageBoxImage.Warning);

				return false;
			}

			// Validar correo institucional
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

		/// <summary>
		/// Evento ejecutado al presionar el botón Cancelar.
		/// 
		/// Cierra el diálogo indicando:
		/// - Operación cancelada
		/// </summary>
		private void BtnCancelar_Click(
			object sender,
			RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		/// <summary>
		/// Evento ejecutado al presionar el botón cerrar.
		/// 
		/// Finaliza la ventana sin guardar cambios.
		/// </summary>
		private void BtnCerrar_Click(
			object sender,
			RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}

}
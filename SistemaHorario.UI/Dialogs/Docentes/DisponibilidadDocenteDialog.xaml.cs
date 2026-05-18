using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Docentes;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SistemaHorario.UI.Dialogs.Docentes
{
	/// <summary>
	/// Dialog utilizado para seleccionar la disponibilidad horaria
	/// de un docente.
	///
	/// Permite:
	/// - Visualizar días de la semana.
	/// - Visualizar franjas horarias.
	/// - Marcar o desmarcar disponibilidad.
	/// - Bloquear horarios institucionales de descanso.
	///
	/// Soluciona el problema de horas duplicadas usando formato AM/PM.
	///
	/// TODO:
	/// Conectar con:
	/// GET /api/Docentes/{id}/disponibilidad
	/// PUT /api/Docentes/{id}/disponibilidad
	/// </summary>
	public partial class DisponibilidadDocenteDialog : Window
	{
		/// <summary>
		/// ViewModel encargado de administrar la disponibilidad
		/// seleccionada dentro del diálogo.
		/// </summary>
		private readonly DisponibilidadDocenteViewModel _viewModel;

		/// <summary>
		/// Lista final de disponibilidad que será devuelta
		/// al formulario principal cuando el usuario guarde.
		/// </summary>
		public List<DisponibilidadDocenteItem> DisponibilidadResultado { get; private set; }

		/// <summary>
		/// Días que se mostrarán como columnas
		/// en la tabla de disponibilidad.
		/// </summary>
		private readonly string[] _dias =
		[
			"Lunes",
			"Martes",
			"Miércoles",
			"Jueves",
			"Viernes",
			"Sábado"
		];

		/// <summary>
		/// Franjas horarias que se mostrarán como filas
		/// en la tabla de disponibilidad.
		///
		/// Algunas franjas representan descansos institucionales
		/// y serán bloqueadas para evitar selección.
		/// </summary>
		private readonly string[] _franjas =
		[
			"7:00 AM",
			"8:00 AM",
			"9:00 AM",
			"10:00 AM",
			"11:00 AM",
			"12:00 - 2:00",
			"2:00 PM",
			"3:00 PM",
			"4:00 PM",
			"5:00 PM",
			"6:00 - 6:30 PM",
			"7:00 PM",
			"8:00 PM",
			"9:00 PM",
			"10:00 PM"
		];

		/// <summary>
		/// Constructor del diálogo de disponibilidad docente.
		///
		/// Recibe la disponibilidad actual, crea una copia
		/// para trabajar dentro del diálogo y construye
		/// visualmente la tabla de disponibilidad.
		/// </summary>
		/// <param name="disponibilidades">
		/// Lista de disponibilidades existentes del docente.
		/// </param>
		public DisponibilidadDocenteDialog(
			List<DisponibilidadDocenteItem> disponibilidades)
		{
			InitializeComponent();

			// Crear copia de la disponibilidad recibida
			// para evitar modificar directamente la lista original.
			DisponibilidadResultado = disponibilidades
				.Select(d => new DisponibilidadDocenteItem
				{
					Dia = d.Dia,
					HoraInicio = d.HoraInicio,
					HoraFin = d.HoraFin,
					Disponible = d.Disponible
				})
				.ToList();

			_viewModel =
				new DisponibilidadDocenteViewModel(
					DisponibilidadResultado);

			ConstruirTablaDisponibilidad();
		}

		/// <summary>
		/// Construye dinámicamente la tabla de disponibilidad.
		///
		/// Limpia la tabla actual y genera:
		/// - Columnas para los días.
		/// - Filas para las franjas horarias.
		/// - Encabezados.
		/// - Celdas seleccionables o bloqueadas.
		/// </summary>
		private void ConstruirTablaDisponibilidad()
		{
			GridDisponibilidad.Children.Clear();
			GridDisponibilidad.RowDefinitions.Clear();
			GridDisponibilidad.ColumnDefinitions.Clear();

			// Primera columna: horas.
			GridDisponibilidad.ColumnDefinitions.Add(
				new ColumnDefinition { Width = new GridLength(110) });

			// Columnas para cada día.
			foreach (string dia in _dias)
			{
				GridDisponibilidad.ColumnDefinitions.Add(
					new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star)
					});
			}

			// Filas: encabezado + franjas horarias.
			for (int i = 0; i <= _franjas.Length; i++)
			{
				GridDisponibilidad.RowDefinitions.Add(
					new RowDefinition { Height = new GridLength(48) });
			}

			// Encabezado de columna de horas.
			AgregarCelda("Hora", 0, 0, true);

			// Encabezados de días.
			for (int i = 0; i < _dias.Length; i++)
				AgregarCelda(_dias[i], 0, i + 1, true);

			// Crear filas de franjas horarias y sus celdas por día.
			for (int fila = 0; fila < _franjas.Length; fila++)
			{
				string hora = _franjas[fila];

				AgregarCelda(hora, fila + 1, 0, true);

				for (int col = 0; col < _dias.Length; col++)
				{
					AgregarCeldaSeleccionable(
						_dias[col],
						hora,
						fila + 1,
						col + 1);
				}
			}
		}

		/// <summary>
		/// Agrega una celda visual al grid.
		///
		/// Puede utilizarse como:
		/// - Encabezado
		/// - Celda común
		/// </summary>
		private void AgregarCelda(
			string texto,
			int fila,
			int columna,
			bool encabezado)
		{
			Border border = new()
			{
				BorderBrush = Brushes.Gray,
				BorderThickness = new Thickness(0.5),
				Background = encabezado
					? new SolidColorBrush(Color.FromRgb(242, 242, 242))
					: Brushes.White,
				Child = new TextBlock
				{
					Text = texto,
					FontWeight = encabezado ? FontWeights.Bold : FontWeights.Normal,
					TextAlignment = TextAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				}
			};

			Grid.SetRow(border, fila);
			Grid.SetColumn(border, columna);

			GridDisponibilidad.Children.Add(border);
		}

		/// <summary>
		/// Agrega una celda de disponibilidad para un día
		/// y una franja horaria específica.
		///
		/// Si la franja está bloqueada:
		/// - Muestra la celda como no seleccionable.
		/// - Indica visualmente "Bloqueado".
		///
		/// Si la franja está disponible:
		/// - Permite hacer clic.
		/// - Alterna entre disponible y no disponible.
		/// </summary>
		private void AgregarCeldaSeleccionable(
			string dia,
			string hora,
			int fila,
			int columna)
		{
			// Validar si la franja corresponde a descanso institucional.
			if (EsFranjaBloqueada(hora))
			{
				Border bloqueado = new()
				{
					BorderBrush = Brushes.Gray,
					BorderThickness = new Thickness(0.5),
					Background = new SolidColorBrush(Color.FromRgb(217, 217, 217)),
					Child = new TextBlock
					{
						Text = "Bloqueado",
						FontSize = 12,
						FontWeight = FontWeights.Bold,
						Foreground = Brushes.DimGray,
						TextAlignment = TextAlignment.Center,
						HorizontalAlignment = HorizontalAlignment.Center,
						VerticalAlignment = VerticalAlignment.Center
					}
				};

				Grid.SetRow(bloqueado, fila);
				Grid.SetColumn(bloqueado, columna);

				GridDisponibilidad.Children.Add(bloqueado);
				return;
			}

			// Buscar si ya existe disponibilidad para el día y hora actual.
			DisponibilidadDocenteItem? franja =
				_viewModel.Disponibilidades.FirstOrDefault(d =>
					d.Dia == dia &&
					d.HoraInicio == hora);

			bool estaDisponible = franja?.Disponible == true;

			// Crear celda interactiva.
			Border border = new()
			{
				BorderBrush = Brushes.Gray,
				BorderThickness = new Thickness(0.5),
				Background = estaDisponible
					? new SolidColorBrush(Color.FromRgb(185, 239, 194))
					: Brushes.White,
				Cursor = System.Windows.Input.Cursors.Hand,
				Tag = new DatosCeldaDisponibilidad(dia, hora),
				Child = new TextBlock
				{
					Text = estaDisponible ? "Disponible" : "",
					FontSize = 12,
					FontWeight = FontWeights.Bold,
					Foreground = new SolidColorBrush(Color.FromRgb(26, 158, 59)),
					HorizontalAlignment = HorizontalAlignment.Center,
					VerticalAlignment = VerticalAlignment.Center
				}
			};

			// Asociar evento de clic a la celda.
			border.MouseLeftButtonDown += CeldaDisponibilidad_Click;

			Grid.SetRow(border, fila);
			Grid.SetColumn(border, columna);

			GridDisponibilidad.Children.Add(border);
		}

		/// <summary>
		/// Evento ejecutado al hacer clic sobre una celda
		/// de disponibilidad.
		///
		/// Obtiene el día y la hora desde el Tag de la celda,
		/// cambia el estado de disponibilidad y reconstruye
		/// la tabla para reflejar el cambio visualmente.
		/// </summary>
		private void CeldaDisponibilidad_Click(
			object sender,
			System.Windows.Input.MouseButtonEventArgs e)
		{
			if (sender is not Border border)
				return;

			if (border.Tag is not DatosCeldaDisponibilidad datos)
				return;

			_viewModel.CambiarEstadoFranja(
				datos.Dia,
				datos.Hora);

			ConstruirTablaDisponibilidad();
		}

		/// <summary>
		/// Indica si una franja horaria corresponde
		/// a un descanso institucional y no puede seleccionarse.
		///
		/// Franjas bloqueadas:
		/// - 12:00 - 2:00
		/// - 6:00 - 6:30 PM
		/// </summary>
		private static bool EsFranjaBloqueada(string hora)
		{
			return hora == "12:00 - 2:00" ||
				   hora == "6:00 - 6:30 PM";
		}

		/// <summary>
		/// Evento ejecutado al presionar el botón Guardar.
		///
		/// Asigna la disponibilidad actual como resultado,
		/// confirma el diálogo y cierra la ventana.
		/// </summary>
		private void BtnGuardar_Click(
			object sender,
			RoutedEventArgs e)
		{
			DisponibilidadResultado = _viewModel.Disponibilidades;

			DialogResult = true;
			Close();
		}

		/// <summary>
		/// Evento ejecutado al presionar el botón Cancelar.
		///
		/// Cancela la operación y cierra el diálogo
		/// sin confirmar cambios.
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
		/// Cierra el diálogo sin guardar cambios.
		/// </summary>
		private void BtnCerrar_Click(
			object sender,
			RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}

		/// <summary>
		/// Clase auxiliar privada utilizada para almacenar
		/// los datos asociados a una celda seleccionable.
		///
		/// Se guarda dentro de la propiedad Tag del Border.
		/// </summary>
		private class DatosCeldaDisponibilidad
		{
			/// <summary>
			/// Día correspondiente a la celda seleccionada.
			/// </summary>
			public string Dia { get; }

			/// <summary>
			/// Hora correspondiente a la celda seleccionada.
			/// </summary>
			public string Hora { get; }

			/// <summary>
			/// Constructor de datos de celda.
			/// </summary>
			public DatosCeldaDisponibilidad(
				string dia,
				string hora)
			{
				Dia = dia;
				Hora = hora;
			}
		}
	}
}
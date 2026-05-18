using SistemaHorario.UI.Dialogs.Shared;
using System.Windows;

namespace SistemaHorario.UI.Dialogs.Coordinadores
{
	/// <summary>
	/// Dialog temporal para verificar
	/// credenciales antes de crear coordinadores.
	/// </summary>
	public partial class VerificarCredencialesDialog : Window
	{
		public bool CredencialesValidas { get; private set; }

		public VerificarCredencialesDialog()
		{
			InitializeComponent();
		}

		/// <summary>
		/// Simula validación temporal.
		///
		/// Futuro:
		///
		/// POST /api/auth/verificar-credenciales
		/// </summary>
		private void BtnVerificar_Click(
			object sender,
			RoutedEventArgs e)
		{
			if (string.IsNullOrWhiteSpace(TxtCorreo.Text))
			{
				MessageBox.Show(
					"⚠ Debe ingresar el correo institucional.");

				return;
			}

			if (string.IsNullOrWhiteSpace(TxtPassword.Password))
			{
				MessageBox.Show(
					"⚠ Debe ingresar la contraseña.");

				return;
			}

			CredencialesValidas = true;

			MensajeExitoDialog dialog = new(
				"Credenciales verificadas correctamente")
			{
				Owner = this
			};

			dialog.ShowDialog();

			DialogResult = true;

			Close();
		}
	}
}
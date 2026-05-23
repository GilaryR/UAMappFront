using SistemaHorario.UI.Models.UI;
using System.Windows;

namespace SistemaHorario.UI.Dialogs.Perfil
{
    /// <summary>
    /// Ventana modal para editar información básica del perfil.
    /// </summary>
    public partial class EditarPerfilDialog : Window
    {
        public PerfilUsuarioItem PerfilResultado { get; private set; }

        public EditarPerfilDialog(PerfilUsuarioItem perfil)
        {
            InitializeComponent();

            PerfilResultado = new PerfilUsuarioItem
            {
                NombreCompleto = perfil.NombreCompleto,
                CorreoInstitucional = perfil.CorreoInstitucional,
                Rol = perfil.Rol,
                Telefono = perfil.Telefono,
                FacultadPrograma = perfil.FacultadPrograma,
                RutaImagen = perfil.RutaImagen
            };

            CargarDatos();
        }

        private void CargarDatos()
        {
            TxtNombreCompleto.Text = PerfilResultado.NombreCompleto;
            TxtCorreoInstitucional.Text = PerfilResultado.CorreoInstitucional;
            TxtTelefono.Text = PerfilResultado.Telefono;
            TxtRol.Text = PerfilResultado.Rol;
            TxtFacultadPrograma.Text = PerfilResultado.FacultadPrograma;
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!FormularioEsValido())
            {
                return;
            }

            PerfilResultado.NombreCompleto = TxtNombreCompleto.Text.Trim();
            PerfilResultado.Telefono = TxtTelefono.Text.Trim();

            // Estos campos se conservan, pero no se editan desde Perfil.
            PerfilResultado.CorreoInstitucional = TxtCorreoInstitucional.Text.Trim();
            PerfilResultado.Rol = TxtRol.Text.Trim();
            PerfilResultado.FacultadPrograma = TxtFacultadPrograma.Text.Trim();

            DialogResult = true;
            Close();
        }

        private bool FormularioEsValido()
        {
            if (string.IsNullOrWhiteSpace(TxtNombreCompleto.Text))
            {
                MessageBox.Show(
                    "El nombre completo es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtTelefono.Text))
            {
                MessageBox.Show(
                    "El teléfono es obligatorio.",
                    "Validación",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );

                return false;
            }

            return true;
        }
    }
}
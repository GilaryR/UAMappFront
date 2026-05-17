using SistemaHorario.UI.Models.UI;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Dialogs.Perfil
{
    /// <summary>
    /// Ventana modal utilizada para editar
    /// la información básica del perfil del usuario.
    ///
    /// Este dialog permite modificar:
    /// - Nombre completo.
    /// - Teléfono.
    /// - Facultad o programa.
    /// - Rol visual seleccionado.
    ///
    /// El correo institucional se muestra bloqueado porque
    /// no debe editarse desde esta pantalla.
    ///
    /// Actualmente solo actualiza información temporal
    /// en frontend.
    ///
    /// Más adelante deberá conectarse con:
    ///
    /// PUT /api/usuarios/perfil
    /// </summary>
    public partial class EditarPerfilDialog : Window
    {
        /// <summary>
        /// Resultado final con los datos editados por el usuario.
        ///
        /// PerfilView utiliza esta propiedad después de que
        /// el dialog se cierra correctamente con Guardar.
        /// </summary>
        public PerfilUsuarioItem PerfilResultado { get; private set; }

        /// <summary>
        /// Constructor principal del dialog.
        ///
        /// Recibe el perfil actual y crea una copia local para evitar
        /// modificar directamente los datos originales antes de guardar.
        /// </summary>
        /// <param name="perfil">
        /// Perfil actual del usuario autenticado.
        /// </param>
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

        /// <summary>
        /// Carga la información actual del perfil
        /// dentro de los controles visuales del formulario.
        /// </summary>
        private void CargarDatos()
        {
            TxtNombreCompleto.Text = PerfilResultado.NombreCompleto;
            TxtCorreoInstitucional.Text = PerfilResultado.CorreoInstitucional;
            TxtTelefono.Text = PerfilResultado.Telefono;
            TxtFacultadPrograma.Text = PerfilResultado.FacultadPrograma;

            foreach (ComboBoxItem item in CmbRol.Items)
            {
                if (item.Content?.ToString() == PerfilResultado.Rol)
                {
                    CmbRol.SelectedItem = item;
                    break;
                }
            }

            CmbRol.SelectedItem ??= CmbRol.Items[0];
        }

        /// <summary>
        /// Evento ejecutado al presionar Cancelar.
        ///
        /// Cierra el dialog sin aplicar cambios.
        /// </summary>
        private void BtnCancelar_Click(
            object sender,
            RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Evento ejecutado al presionar Guardar.
        ///
        /// Valida el formulario, actualiza PerfilResultado
        /// y cierra el dialog indicando operación exitosa.
        ///
        /// Más adelante este resultado será usado para construir
        /// el request de PUT /api/usuarios/perfil.
        /// </summary>
        private void BtnGuardar_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (!FormularioEsValido())
                return;

            PerfilResultado.NombreCompleto =
                TxtNombreCompleto.Text.Trim();

            PerfilResultado.Telefono =
                TxtTelefono.Text.Trim();

            PerfilResultado.FacultadPrograma =
                TxtFacultadPrograma.Text.Trim();

            if (CmbRol.SelectedItem is ComboBoxItem item)
            {
                PerfilResultado.Rol =
                    item.Content?.ToString() ?? "Administrador";
            }

            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Valida los campos obligatorios del formulario.
        ///
        /// Reglas actuales:
        /// - Nombre completo obligatorio.
        /// - Teléfono obligatorio.
        ///
        /// El correo institucional no se valida aquí porque
        /// permanece bloqueado y proviene del perfil actual.
        /// </summary>
        /// <returns>
        /// true si el formulario es válido.
        /// false si falta información obligatoria.
        /// </returns>
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
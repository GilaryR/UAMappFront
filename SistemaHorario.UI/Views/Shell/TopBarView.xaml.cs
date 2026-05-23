using SistemaHorario.UI.State;
using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistemaHorario.UI.Views.Shell
{
    public partial class TopBarView : UserControl
    {
        public event EventHandler? MenuUsuarioSolicitado;

        public TopBarView()
        {
            InitializeComponent();

            ConfigurarUsuario(
                UsuarioSesion.NombreCompleto,
                UsuarioSesion.CorreoInstitucional
            );
        }

        public void ConfigurarUsuario(
            string nombreCompleto,
            string correo)
        {
            TxtNombreUsuario.Text = ObtenerNombreCorto(nombreCompleto);
        }

        private string ObtenerNombreCorto(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
            {
                return "Usuario";
            }

            string[] partes = nombreCompleto.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries
            );

            if (partes.Length == 1)
            {
                return partes[0];
            }

            return $"{partes[0]} {partes[1]}";
        }

        private void BtnUsuario_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            MenuUsuarioSolicitado?.Invoke(this, EventArgs.Empty);
        }
    }
}
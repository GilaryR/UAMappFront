using SistemaHorario.UI.State;
using System;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Shell
{
    public partial class UserMenuView : UserControl
    {
        public event EventHandler? PerfilSolicitado;
        public event EventHandler? CerrarSesionSolicitado;

        public UserMenuView()
        {
            InitializeComponent();

            Visibility = Visibility.Collapsed;

            ConfigurarUsuario(
                UsuarioSesion.NombreCompleto,
                UsuarioSesion.CorreoInstitucional
            );
        }

        public void ConfigurarUsuario(
            string nombreCompleto,
            string correoInstitucional)
        {
            TxtNombreUsuario.Text = ObtenerNombreCorto(nombreCompleto);
            TxtCorreoUsuario.Text = correoInstitucional;
        }

        public void Mostrar()
        {
            Visibility = Visibility.Visible;
        }

        public void Ocultar()
        {
            Visibility = Visibility.Collapsed;
        }

        public void Alternar()
        {
            Visibility = Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
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

        private void BtnMiPerfil_Click(object sender, RoutedEventArgs e)
        {
            Ocultar();
            PerfilSolicitado?.Invoke(this, EventArgs.Empty);
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            Ocultar();
            CerrarSesionSolicitado?.Invoke(this, EventArgs.Empty);
        }
    }
}

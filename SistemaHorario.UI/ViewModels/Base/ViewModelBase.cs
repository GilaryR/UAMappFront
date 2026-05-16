using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.ViewModels.Base
{
    /// <summary>
    /// Clase base para todos los ViewModels del sistema.
    ///
    /// Implementa INotifyPropertyChanged para permitir
    /// actualización automática de la interfaz gráfica.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Evento ejecutado cuando una propiedad cambia.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifica a la interfaz que una propiedad cambió.
        /// </summary>
        protected void OnPropertyChanged(
            [CallerMemberName] string? nombre = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(nombre)
            );
        }

        /// <summary>
        /// Actualiza una propiedad y notifica el cambio
        /// solo cuando el valor realmente cambió.
        /// </summary>
        protected bool SetProperty<T>(
            ref T campo,
            T valor,
            [CallerMemberName] string? nombrePropiedad = null)
        {
            if (Equals(campo, valor))
                return false;

            campo = valor;
            OnPropertyChanged(nombrePropiedad);

            return true;
        }

    }
}
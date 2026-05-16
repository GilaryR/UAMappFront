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
    }
}
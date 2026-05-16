using System;
using System.Windows.Input;

namespace SistemaHorario.UI.ViewModels.Base
{
    /// <summary>
    /// Implementación básica de ICommand.
    ///
    /// Permite conectar acciones desde ViewModels
    /// hacia botones y eventos visuales.
    /// </summary>
    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool>? _canExecute;

        /// <summary>
        /// Evento ejecutado cuando cambia el estado del comando.
        /// </summary>
        public event EventHandler? CanExecuteChanged;

        /// <summary>
        /// Constructor del comando.
        /// </summary>
        public RelayCommand(
            Action execute,
            Func<bool>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Indica si el comando puede ejecutarse.
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            return _canExecute == null || _canExecute();
        }

        /// <summary>
        /// Ejecuta la acción del comando.
        /// </summary>
        public void Execute(object? parameter)
        {
            _execute();
        }

        /// <summary>
        /// Fuerza actualización del estado del comando.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
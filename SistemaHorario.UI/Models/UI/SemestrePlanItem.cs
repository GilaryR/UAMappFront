using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Representa un semestre dentro de un plan académico.
    ///
    /// Cada semestre tiene:
    /// - Un número.
    /// - Una jornada.
    /// - Una lista de materias asignadas.
    ///
    /// También calcula automáticamente cuántas materias y créditos tiene.
    /// </summary>
    public class SemestrePlanItem : INotifyPropertyChanged
    {
        /// <summary>
        /// Lista interna de materias del semestre.
        /// Se usa este campo privado para poder detectar cuando la lista cambia.
        /// </summary>
        private ObservableCollection<MateriaItem> _materias = new();

        /// <summary>
        /// Identificador del semestre.
        ///
        /// Cuando se conecte el backend, este valor debe venir desde la base de datos.
        /// </summary>
        public int IdSemestre { get; set; }

        /// <summary>
        /// Número del semestre dentro del plan académico.
        /// Ejemplo: 1, 2, 3, etc.
        /// </summary>
        public int Numero { get; set; }

        /// <summary>
        /// Número del semestre usado por la interfaz.
        ///
        /// Al cambiar este valor, también se actualizan los textos que dependen
        /// del número, como "Semestre 1" o "Semestre 2".
        /// </summary>
        public int NumeroSemestre
        {
            get => Numero;
            set
            {
                Numero = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Nombre));
                OnPropertyChanged(nameof(NombreSemestre));
            }
        }

        /// <summary>
        /// Jornada del semestre.
        /// Ejemplo: Diurna, Nocturna o Por definir.
        /// </summary>
        public string Jornada { get; set; } = string.Empty;

        /// <summary>
        /// Nombre corto del semestre.
        /// Ejemplo: Semestre 1.
        /// </summary>
        public string Nombre => $"Semestre {Numero}";

        /// <summary>
        /// Nombre usado en las tarjetas de la malla académica.
        /// Ejemplo: Semestre 1.
        /// </summary>
        public string NombreSemestre => $"Semestre {Numero}";

        /// <summary>
        /// Materias asignadas al semestre.
        ///
        /// Si se agrega o elimina una materia, la interfaz actualiza
        /// automáticamente el total de materias y créditos.
        /// </summary>
        public ObservableCollection<MateriaItem> Materias
        {
            get => _materias;
            set
            {
                if (_materias != null)
                {
                    _materias.CollectionChanged -= Materias_CollectionChanged;
                }

                _materias = value;
                _materias.CollectionChanged += Materias_CollectionChanged;

                OnPropertyChanged();
                NotificarTotales();
            }
        }

        /// <summary>
        /// Cantidad total de materias asignadas al semestre.
        /// Se calcula a partir de la lista Materias.
        /// </summary>
        public int TotalMaterias => Materias.Count;

        /// <summary>
        /// Cantidad total de créditos del semestre.
        /// Se calcula sumando los créditos de las materias asignadas.
        /// </summary>
        public int TotalCreditos => Materias.Sum(materia => materia.Creditos);

        /// <summary>
        /// Evento usado por WPF para actualizar la interfaz cuando cambia un dato.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Constructor del semestre.
        ///
        /// Se conecta al evento de cambios de la lista de materias para que,
        /// cuando se agreguen o quiten materias, se actualicen los totales.
        /// </summary>
        public SemestrePlanItem()
        {
            _materias.CollectionChanged += Materias_CollectionChanged;
        }

        /// <summary>
        /// Agrega una materia al semestre.
        ///
        /// Si la materia ya existe en el semestre, no la agrega nuevamente.
        /// </summary>
        public void AgregarMateria(MateriaItem materia)
        {
            if (Materias.Any(item => item.IdMateria == materia.IdMateria))
                return;

            Materias.Add(materia);
        }

        /// <summary>
        /// Quita una materia del semestre usando su id.
        ///
        /// Si no encuentra la materia, no realiza ningún cambio.
        /// </summary>
        public void QuitarMateria(int idMateria)
        {
            MateriaItem? materia = Materias
                .FirstOrDefault(item => item.IdMateria == idMateria);

            if (materia == null)
                return;

            Materias.Remove(materia);
        }

        /// <summary>
        /// Se ejecuta cuando cambia la lista de materias.
        /// Por ejemplo, cuando se agrega o se elimina una materia.
        /// </summary>
        private void Materias_CollectionChanged(
            object? sender,
            NotifyCollectionChangedEventArgs e)
        {
            NotificarTotales();
        }

        /// <summary>
        /// Notifica a la interfaz que debe actualizar:
        /// - Total de materias.
        /// - Total de créditos.
        /// - Lista de materias.
        /// </summary>
        private void NotificarTotales()
        {
            OnPropertyChanged(nameof(TotalMaterias));
            OnPropertyChanged(nameof(TotalCreditos));
            OnPropertyChanged(nameof(Materias));
        }

        /// <summary>
        /// Notifica a WPF que una propiedad cambió para refrescar la pantalla.
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }
    }
}
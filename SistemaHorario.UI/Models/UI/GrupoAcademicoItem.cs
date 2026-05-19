using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Representa un grupo académico en la pantalla.
    /// 
    /// Ejemplo:
    /// Una materia como Redes LAN puede tener varios grupos,
    /// por ejemplo un grupo diurno y otro nocturno.
    /// 
    /// Este modelo queda listo para llenarse después con datos del backend.
    /// </summary>
    public class GrupoAcademicoItem : INotifyPropertyChanged
    {
        private int _idGrupoAcademico;
        private int _idPlanAcademico;
        private int _numeroSemestre;
        private string _nombreGrupo = string.Empty;
        private string _codigo = string.Empty;
        private string _jornada = string.Empty;
        private string _materia = string.Empty;
        private string _tipo = string.Empty;
        private string _estado = string.Empty;
        private int _estudiantesActuales;
        private int _plazasDisponibles;
        private ObservableCollection<string> _dias = new();

        public int IdGrupoAcademico
        {
            get => _idGrupoAcademico;
            set { _idGrupoAcademico = value; OnPropertyChanged(); }
        }

        public int IdPlanAcademico
        {
            get => _idPlanAcademico;
            set { _idPlanAcademico = value; OnPropertyChanged(); }
        }

        public int NumeroSemestre
        {
            get => _numeroSemestre;
            set { _numeroSemestre = value; OnPropertyChanged(); }
        }

        public string NombreGrupo
        {
            get => _nombreGrupo;
            set
            {
                _nombreGrupo = value;
                OnPropertyChanged();
            }
        }

        public string Codigo
        {
            get => _codigo;
            set
            {
                _codigo = value;
                OnPropertyChanged();
            }
        }

        public string Jornada
        {
            get => _jornada;
            set
            {
                _jornada = value;
                OnPropertyChanged();
            }
        }

        public string Materia
        {
            get => _materia;
            set
            {
                _materia = value;
                OnPropertyChanged();
            }
        }

        public string Tipo
        {
            get => _tipo;
            set
            {
                _tipo = value;
                OnPropertyChanged();
            }
        }

        public string Estado
        {
            get => _estado;
            set
            {
                _estado = value;
                OnPropertyChanged();
            }
        }

        public int EstudiantesActuales
        {
            get => _estudiantesActuales;
            set
            {
                _estudiantesActuales = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstudiantesTexto));
            }
        }

        public int PlazasDisponibles
        {
            get => _plazasDisponibles;
            set
            {
                _plazasDisponibles = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EstudiantesTexto));
            }
        }

        public ObservableCollection<string> Dias
        {
            get => _dias;
            set
            {
                _dias = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Horario));
            }
        }

        public string Horario => Dias.Count == 0
            ? "Sin horario"
            : string.Join(", ", Dias);

        public string EstudiantesTexto => $"{EstudiantesActuales}/{PlazasDisponibles}";

        public event PropertyChangedEventHandler? PropertyChanged;

        public GrupoAcademicoItem Clonar()
        {
            return new GrupoAcademicoItem
            {
                IdGrupoAcademico = IdGrupoAcademico,
                IdPlanAcademico = IdPlanAcademico,
                NumeroSemestre = NumeroSemestre,
                NombreGrupo = NombreGrupo,
                Codigo = Codigo,
                Jornada = Jornada,
                Materia = Materia,
                Tipo = Tipo,
                Estado = Estado,
                EstudiantesActuales = EstudiantesActuales,
                PlazasDisponibles = PlazasDisponibles,
                Dias = new ObservableCollection<string>(Dias.ToList())
            };
        }

        public void CopiarDesde(GrupoAcademicoItem grupo)
        {
            IdGrupoAcademico = grupo.IdGrupoAcademico;
            IdPlanAcademico = grupo.IdPlanAcademico;
            NumeroSemestre = grupo.NumeroSemestre;
            NombreGrupo = grupo.NombreGrupo;
            Codigo = grupo.Codigo;
            Jornada = grupo.Jornada;
            Materia = grupo.Materia;
            Tipo = grupo.Tipo;
            Estado = grupo.Estado;
            EstudiantesActuales = grupo.EstudiantesActuales;
            PlazasDisponibles = grupo.PlazasDisponibles;
            Dias = new ObservableCollection<string>(grupo.Dias.ToList());

            OnPropertyChanged(nameof(Horario));
            OnPropertyChanged(nameof(EstudiantesTexto));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }
    }
}
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.Models.UI
{
    // Representa una sección de estudiantes dentro de un plan, jornada y semestre.
    public class GrupoAcademicoItem : INotifyPropertyChanged
    {
        private int _idGrupoAcademico;
        private int _idPlanAcademico;
        private int _numeroSemestre;
        private string _nombreGrupo = string.Empty;
        private string _codigo = string.Empty;
        private string _jornada = string.Empty;
        private string _tipo = string.Empty;
        private string _estado = string.Empty;
        private int _estudiantesActuales;
        private int _plazasDisponibles;

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
            set { _nombreGrupo = value; OnPropertyChanged(); }
        }

        public string Codigo
        {
            get => _codigo;
            set { _codigo = value; OnPropertyChanged(); }
        }

        public string Jornada
        {
            get => _jornada;
            set { _jornada = value; OnPropertyChanged(); }
        }

        public string Tipo
        {
            get => _tipo;
            set { _tipo = value; OnPropertyChanged(); }
        }

        public string Estado
        {
            get => _estado;
            set { _estado = value; OnPropertyChanged(); }
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
                Tipo = Tipo,
                Estado = Estado,
                EstudiantesActuales = EstudiantesActuales,
                PlazasDisponibles = PlazasDisponibles
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
            Tipo = grupo.Tipo;
            Estado = grupo.Estado;
            EstudiantesActuales = grupo.EstudiantesActuales;
            PlazasDisponibles = grupo.PlazasDisponibles;

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
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;

namespace SistemaHorario.UI.ViewModels.Materias
{
    /// <summary>
    /// ViewModel de la vista Materias.
    ///
    /// Centraliza los datos temporales, filtros y operaciones visuales
    /// del módulo de materias.
    ///
    /// Actualmente NO consume API.
    ///
    /// Más adelante deberá conectarse con:
    /// - GET /api/materias
    /// - POST /api/materias
    /// - PUT /api/materias/{id}
    /// - DELETE /api/materias/{id}
    /// - GET /api/materias/{id}/prerrequisitos
    /// </summary>
    public class MateriasViewModel : ViewModelBase
    {
        private string _busqueda = string.Empty;
        private string _semestreSeleccionado = "Todos";
        private string _estadoSeleccionado = "Todos";

        /// <summary>
        /// Lista completa temporal de materias.
        /// </summary>
        public ObservableCollection<MateriaItem> Materias { get; } = new();

        /// <summary>
        /// Lista filtrada que se muestra en pantalla.
        /// </summary>
        public ObservableCollection<MateriaItem> MateriasFiltradas { get; } = new();

        /// <summary>
        /// Texto de búsqueda.
        /// </summary>
        public string Busqueda
        {
            get => _busqueda;
            set
            {
                if (SetProperty(ref _busqueda, value))
                    AplicarFiltros();
            }
        }

        /// <summary>
        /// Semestre seleccionado.
        /// </summary>
        public string SemestreSeleccionado
        {
            get => _semestreSeleccionado;
            set
            {
                if (SetProperty(ref _semestreSeleccionado, value))
                    AplicarFiltros();
            }
        }

        /// <summary>
        /// Estado seleccionado.
        /// </summary>
        public string EstadoSeleccionado
        {
            get => _estadoSeleccionado;
            set
            {
                if (SetProperty(ref _estadoSeleccionado, value))
                    AplicarFiltros();
            }
        }

        /// <summary>
        /// Constructor del ViewModel.
        /// </summary>
        public MateriasViewModel()
        {
            CargarDatosTemporales();
            AplicarFiltros();
        }

        /// <summary>
        /// Carga datos temporales para validar la interfaz.
        ///
        /// TODO:
        /// Reemplazar por GET /api/materias.
        /// </summary>
        private void CargarDatosTemporales()
        {
            Materias.Clear();

            Materias.Add(new MateriaItem
            {
                IdMateria = 1,
                Codigo = "MAT101",
                Nombre = "Cálculo Diferencial",
                Creditos = 4,
                IntensidadHorariaSemanal = 64,
                Semestre = 1,
                CantidadGrupos = 1,
                Activa = true
            });

            Materias.Add(new MateriaItem
            {
                IdMateria = 2,
                Codigo = "MAT102",
                Nombre = "Álgebra Lineal",
                Creditos = 3,
                IntensidadHorariaSemanal = 48,
                Semestre = 2,
                CantidadGrupos = 2,
                Activa = true
            });

            Materias.Add(new MateriaItem
            {
                IdMateria = 3,
                Codigo = "MAT103",
                Nombre = "Cálculo Integral",
                Creditos = 4,
                IntensidadHorariaSemanal = 64,
                Semestre = 2,
                CantidadGrupos = 2,
                Activa = true
            });

            Materias.Add(new MateriaItem
            {
                IdMateria = 4,
                Codigo = "FIS101",
                Nombre = "Física I",
                Creditos = 4,
                IntensidadHorariaSemanal = 64,
                Semestre = 1,
                CantidadGrupos = 1,
                Activa = true
            });

            Materias.Add(new MateriaItem
            {
                IdMateria = 5,
                Codigo = "QUI101",
                Nombre = "Física II",
                Creditos = 3,
                IntensidadHorariaSemanal = 48,
                Semestre = 1,
                CantidadGrupos = 1,
                Activa = false
            });
        }

        /// <summary>
        /// Aplica los filtros de búsqueda, semestre y estado.
        /// </summary>
        public void AplicarFiltros()
        {
            var resultado = Materias.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                string busquedaNormalizada = Busqueda.ToLower();

                resultado = resultado.Where(m =>
                    m.Codigo.ToLower().Contains(busquedaNormalizada) ||
                    m.Nombre.ToLower().Contains(busquedaNormalizada));
            }

            if (SemestreSeleccionado != "Todos")
            {
                if (int.TryParse(SemestreSeleccionado, out int semestre))
                    resultado = resultado.Where(m => m.Semestre == semestre);
            }

            if (EstadoSeleccionado != "Todos")
            {
                bool activa = EstadoSeleccionado == "Activa";
                resultado = resultado.Where(m => m.Activa == activa);
            }

            MateriasFiltradas.Clear();

            foreach (MateriaItem materia in resultado)
                MateriasFiltradas.Add(materia);
        }

        /// <summary>
        /// Limpia todos los filtros.
        /// </summary>
        public void LimpiarFiltros()
        {
            Busqueda = string.Empty;
            SemestreSeleccionado = "Todos";
            EstadoSeleccionado = "Todos";

            AplicarFiltros();
        }

        /// <summary>
        /// Agrega una nueva materia temporalmente.
        ///
        /// TODO:
        /// Reemplazar por POST /api/materias.
        /// </summary>
        public void AgregarMateria(MateriaItem materia)
        {
            int nuevoId = Materias.Any()
                ? Materias.Max(m => m.IdMateria) + 1
                : 1;

            materia.IdMateria = nuevoId;
            materia.Activa = true;

            Materias.Add(materia);
            AplicarFiltros();
        }

        /// <summary>
        /// Actualiza una materia existente temporalmente.
        ///
        /// TODO:
        /// Reemplazar por PUT /api/materias/{id}.
        /// </summary>
        public void ActualizarMateria(MateriaItem materiaActualizada)
        {
            MateriaItem? materiaExistente =
                Materias.FirstOrDefault(m =>
                    m.IdMateria == materiaActualizada.IdMateria);

            if (materiaExistente == null)
                return;

            materiaExistente.Codigo = materiaActualizada.Codigo;
            materiaExistente.Nombre = materiaActualizada.Nombre;
            materiaExistente.Creditos = materiaActualizada.Creditos;
            materiaExistente.IntensidadHorariaSemanal =
                materiaActualizada.IntensidadHorariaSemanal;
            materiaExistente.Semestre = materiaActualizada.Semestre;
            materiaExistente.CantidadGrupos = materiaActualizada.CantidadGrupos;
            materiaExistente.Activa = materiaActualizada.Activa;

            AplicarFiltros();
        }

        /// <summary>
        /// Elimina una materia temporalmente.
        ///
        /// TODO:
        /// Reemplazar por DELETE /api/materias/{id}.
        /// </summary>
        public void EliminarMateria(MateriaItem materia)
        {
            Materias.Remove(materia);
            AplicarFiltros();
        }

        /// <summary>
        /// Retorna materias disponibles para usarse como prerrequisitos.
        /// </summary>
        public ObservableCollection<MateriaItem> ObtenerMateriasDisponiblesComoPrerrequisito(
            MateriaItem? materiaActual = null)
        {
            ObservableCollection<MateriaItem> resultado = new();

            foreach (MateriaItem materia in Materias)
            {
                if (materiaActual != null &&
                    materia.IdMateria == materiaActual.IdMateria)
                {
                    continue;
                }

                resultado.Add(materia);
            }

            return resultado;
        }
    }
}
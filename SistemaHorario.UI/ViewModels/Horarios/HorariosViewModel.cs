using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    public class HorariosViewModel
    {
        private readonly HorariosApiService _api = new();

        public ObservableCollection<HorarioItem> Horarios { get; set; }

        public ObservableCollection<HorarioItem> HorariosFiltrados { get; set; }

        public string Busqueda { get; set; } = string.Empty;

        public string JornadaSeleccionada { get; set; } = "Todas";

        public string EstadoSeleccionado { get; set; } = "Todos";

        public int PaginaActual { get; private set; } = 1;

        public int RegistrosPorPagina { get; set; } = 5;

        public int TotalPaginas { get; private set; } = 1;

        public string MensajeEstado { get; private set; } = string.Empty;

        public HorariosViewModel()
        {
            Horarios = new ObservableCollection<HorarioItem>();
            HorariosFiltrados = new ObservableCollection<HorarioItem>();
        }

        public async Task CargarDatosAsync()
        {
            var resp = await _api.ObtenerHorariosAsync();

            Horarios.Clear();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar los horarios."
                    : resp.Message;

                PaginaActual = 1;
                AplicarFiltros();
                return;
            }

            foreach (HorarioItem horario in resp.Data)
            {
                Horarios.Add(horario);
            }

            MensajeEstado = string.Empty;
            PaginaActual = 1;
            AplicarFiltros();
        }

        public void AplicarFiltros()
        {
            IEnumerable<HorarioItem> resultado = Horarios.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                string busquedaNormalizada = Busqueda.ToLower();

                resultado = resultado.Where(h =>
                    h.Nombre.ToLower().Contains(busquedaNormalizada) ||
                    h.Grupo.ToLower().Contains(busquedaNormalizada) ||
                    h.Tipo.ToLower().Contains(busquedaNormalizada) ||
                    h.Jornada.ToLower().Contains(busquedaNormalizada) ||
                    h.Estado.ToLower().Contains(busquedaNormalizada));
            }

            if (JornadaSeleccionada != "Todas")
            {
                resultado = resultado.Where(h => h.Jornada == JornadaSeleccionada);
            }

            if (EstadoSeleccionado != "Todos")
            {
                resultado = resultado.Where(h => h.Estado == EstadoSeleccionado);
            }

            List<HorarioItem> listaFiltrada = resultado.ToList();

            TotalPaginas = listaFiltrada.Count == 0
                ? 1
                : (int)Math.Ceiling(listaFiltrada.Count / (double)RegistrosPorPagina);

            if (PaginaActual > TotalPaginas)
            {
                PaginaActual = TotalPaginas;
            }

            HorariosFiltrados.Clear();

            foreach (HorarioItem horario in listaFiltrada
                .Skip((PaginaActual - 1) * RegistrosPorPagina)
                .Take(RegistrosPorPagina))
            {
                HorariosFiltrados.Add(horario);
            }
        }

        public void LimpiarFiltros()
        {
            Busqueda = string.Empty;
            JornadaSeleccionada = "Todas";
            EstadoSeleccionado = "Todos";
            PaginaActual = 1;

            AplicarFiltros();
        }

        public async Task<bool> EliminarHorarioGrupoAsync(HorarioItem horario)
        {
            var resp = await _api.EliminarHorarioGrupoAsync(horario.IdGrupo);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo eliminar el horario del grupo."
                    : resp.Message;

                return false;
            }

            Horarios.Remove(horario);
            AplicarFiltros();
            MensajeEstado = string.Empty;
            return true;
        }
    }
}

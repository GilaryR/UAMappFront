using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Controls
{
    /// <summary>
    /// Panel reutilizable de filtros para las pantallas del sistema.
    ///
    /// Este control permite configurar de forma dinámica:
    /// - Búsqueda por texto.
    /// - Uno o dos ComboBox de filtros.
    /// - Rango de fechas.
    /// - Botón Filtrar.
    /// - Botón Limpiar.
    ///
    /// Está preparado para endpoints que usan parámetros como:
    /// - busqueda
    /// - estado
    /// - rol
    /// - tipo
    /// - fecha
    /// - usuario
    /// - modulo
    /// - fechaDesde
    /// - fechaHasta
    /// - programa
    /// - jornada
    ///
    /// El control no consume la API directamente.
    /// Solo entrega los valores seleccionados para que el ViewModel
    /// o la vista construyan la consulta correspondiente.
    /// </summary>
    public partial class FilterPanel : UserControl
    {
        /// <summary>
        /// Evento que se ejecuta cuando el usuario presiona el botón Filtrar.
        /// </summary>
        public event EventHandler? FiltrosAplicados;

        /// <summary>
        /// Evento que se ejecuta cuando el usuario presiona el botón Limpiar.
        /// </summary>
        public event EventHandler? FiltrosLimpiados;

        /// <summary>
        /// Constructor del control FilterPanel.
        /// </summary>
        public FilterPanel()
        {
            InitializeComponent();

            OcultarFiltrosOpcionales();
        }

        /// <summary>
        /// Texto escrito en el campo de búsqueda.
        /// </summary>
        public string Busqueda => TxtBusqueda.Text.Trim();

        /// <summary>
        /// Valor seleccionado en el primer ComboBox.
        /// </summary>
        public string? ValorFiltro1 => CmbFiltro1.SelectedValue?.ToString();

        /// <summary>
        /// Valor seleccionado en el segundo ComboBox.
        /// </summary>
        public string? ValorFiltro2 => CmbFiltro2.SelectedValue?.ToString();

        /// <summary>
        /// Fecha inicial seleccionada.
        /// </summary>
        public DateTime? FechaDesde => DtpFechaDesde.SelectedDate;

        /// <summary>
        /// Fecha final seleccionada.
        /// </summary>
        public DateTime? FechaHasta => DtpFechaHasta.SelectedDate;

        /// <summary>
        /// Configura el título principal del panel.
        /// </summary>
        public void ConfigurarTitulo(string titulo)
        {
            TxtTitulo.Text = titulo;
        }

        /// <summary>
        /// Muestra u oculta el campo de búsqueda.
        /// </summary>
        public void MostrarBusqueda(bool mostrar)
        {
            PnlBusqueda.Visibility = mostrar
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        /// Configura el primer filtro desplegable.
        /// </summary>
        public void ConfigurarFiltro1(string titulo, List<FilterOptionItem> opciones)
        {
            LblFiltro1.Text = titulo;
            CmbFiltro1.ItemsSource = opciones;
            PnlFiltro1.Visibility = Visibility.Visible;

            if (opciones.Count > 0)
                CmbFiltro1.SelectedIndex = 0;
        }

        /// <summary>
        /// Configura el segundo filtro desplegable.
        /// </summary>
        public void ConfigurarFiltro2(string titulo, List<FilterOptionItem> opciones)
        {
            LblFiltro2.Text = titulo;
            CmbFiltro2.ItemsSource = opciones;
            PnlFiltro2.Visibility = Visibility.Visible;

            if (opciones.Count > 0)
                CmbFiltro2.SelectedIndex = 0;
        }

        /// <summary>
        /// Muestra u oculta el rango de fechas.
        /// </summary>
        public void MostrarRangoFechas(bool mostrar)
        {
            PnlFechaDesde.Visibility = mostrar
                ? Visibility.Visible
                : Visibility.Collapsed;

            PnlFechaHasta.Visibility = mostrar
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        /// <summary>
        /// Limpia todos los filtros seleccionados.
        /// </summary>
        public void Limpiar()
        {
            TxtBusqueda.Clear();

            if (CmbFiltro1.Items.Count > 0)
                CmbFiltro1.SelectedIndex = 0;

            if (CmbFiltro2.Items.Count > 0)
                CmbFiltro2.SelectedIndex = 0;

            DtpFechaDesde.SelectedDate = null;
            DtpFechaHasta.SelectedDate = null;
        }

        /// <summary>
        /// Oculta los filtros que no siempre se utilizan.
        /// </summary>
        private void OcultarFiltrosOpcionales()
        {
            PnlFiltro1.Visibility = Visibility.Collapsed;
            PnlFiltro2.Visibility = Visibility.Collapsed;
            PnlFechaDesde.Visibility = Visibility.Collapsed;
            PnlFechaHasta.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Ejecuta el evento de aplicación de filtros.
        /// </summary>
        private void BtnAplicar_Click(object sender, RoutedEventArgs e)
        {
            FiltrosAplicados?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Limpia el panel de filtros y ejecuta el evento correspondiente.
        /// </summary>
        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            Limpiar();
            FiltrosLimpiados?.Invoke(this, EventArgs.Empty);
        }
    }
}
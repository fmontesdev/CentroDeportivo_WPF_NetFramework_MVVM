using System;
using System.Windows;
using Model.DataSets;
using Reports.Reports;

namespace Reports.Windows
{
    /// <summary>
    /// Ventana para visualizar el informe de historial de reservas con Crystal Reports.
    /// Muestra el historial completo de reservas agrupado por socio y ordenado cronológicamente
    /// </summary>
    public partial class HistorialReservasWindow : Window
    {
        private HistorialReservas _report;

        /// <summary>
        /// Constructor que inicializa la ventana y carga el informe de Crystal Reports
        /// </summary>
        /// <param name="dataSet">DataSet tipado con el historial de reservas</param>
        public HistorialReservasWindow(dsReservasHistorial dataSet)
        {
            InitializeComponent();

            try
            {
                // Crear instancia del informe (recurso incrustado)
                _report = new HistorialReservas();

                // Asignar el DataSet al informe
                _report.SetDataSource(dataSet);

                // Asignar el documento al visor
                crystalReportViewer.ReportSource = _report;
                crystalReportViewer.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar el informe:\n\n{ex.Message}",
                    "Error en Informe",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                this.Close();
            }
        }

        /// <summary>
        /// Libera los recursos del informe cuando se cierra la ventana
        /// </summary>
        /// <param name="e">Argumentos del evento</param>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (_report != null)
            {
                _report.Close();
                _report.Dispose();
            }
        }
    }
}

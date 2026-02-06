using System;
using System.Windows;
using Model.DataSets;
using Reports.Reports;

namespace Reports.Windows
{
    /// <summary>
    /// Ventana para visualizar el informe de reservas por actividad con Crystal Reports.
    /// Muestra el listado de reservas filtrado por una actividad específica con cálculo de ocupación
    /// </summary>
    public partial class ListadoReservasPorActividadWindow : Window
    {
        private ListadoReserevasPorActividad _report;

        /// <summary>
        /// Constructor que inicializa la ventana y carga el informe de Crystal Reports con parámetros
        /// </summary>
        /// <param name="dataSet">DataSet tipado con las reservas de la actividad</param>
        /// <param name="nombreActividad">Nombre de la actividad para mostrar en el informe</param>
        public ListadoReservasPorActividadWindow(dsReservasPorActividad dataSet, string nombreActividad)
        {
            InitializeComponent();

            try
            {
                // Crear instancia del informe (recurso incrustado)
                _report = new ListadoReserevasPorActividad();

                // Asignar el DataSet al informe
                _report.SetDataSource(dataSet);

                // Pasar el parámetro del nombre de la actividad
                _report.SetParameterValue("Actividad", nombreActividad);

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

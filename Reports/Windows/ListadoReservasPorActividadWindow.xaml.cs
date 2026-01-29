using System;
using System.Windows;
using Model.DataSets;
using Reports.Reports;

namespace Reports.Windows
{
    public partial class ListadoReservasPorActividadWindow : Window
    {
        private ListadoReserevasPorActividad _report;

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

        // Libera recursos cuando se cierra la ventana
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

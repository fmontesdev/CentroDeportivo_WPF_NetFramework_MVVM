using System;
using System.Windows;
using Model.DataSets;
using Reports.Reports;

namespace Reports.Windows
{
    public partial class HistorialReservasWindow : Window
    {
        private HistorialReservas _report;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;
using CentroDeportivo.Windows;

namespace CentroDeportivo.Views
{
    /// <summary>
    /// Code-behind para ReservasView.xaml.
    /// Vista principal para la gestión de reservas del centro deportivo con filtrado
    /// </summary>
    public partial class ReservasView : UserControl
    {
        /// <summary>
        /// Constructor que inicializa la vista y configura el ViewModel
        /// </summary>
        public ReservasView()
        {
            InitializeComponent();

            // Crear e instanciar el ViewModel (variable local)
            var viewModel = new ReservaViewModel();

            // Asignar Actions (más simple que eventos)
            viewModel.VentanaNuevaReserva = VentanaNuevaReserva;
            viewModel.ConfirmarCancelar = ConfirmarCancelar;

            // Asignar DataContext
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva reserva
        /// </summary>
        private void VentanaNuevaReserva()
        {
            var ventana = new NuevaReservaWindow();
            
            // ShowDialog devuelve true si se guardó correctamente
            if (ventana.ShowDialog() == true)
            {
                // Recargar la lista de reservas
                ((ReservaViewModel)this.DataContext).InicializarAsync();
            }
        }

        /// <summary>
        /// Muestra un diálogo de confirmación antes de cancelar una reserva
        /// </summary>
        /// <param name="info">Tupla con el ID de reserva, nombre del socio y nombre de la actividad</param>
        private void ConfirmarCancelar((int IdReserva, string NombreSocio, string NombreActividad) info)
        {
            var resultado = MessageBox.Show(
                $"¿Está seguro que desea cancelar la reserva?\n\n" +
                $"Socio: {info.NombreSocio}\n" +
                $"Actividad: {info.NombreActividad}",
                "Confirmar cancelación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado == MessageBoxResult.Yes)
            {
                // Confirmar cancelación en el ViewModel pasando solo el ID
                ((ReservaViewModel)this.DataContext).ConfirmarCancelarReserva(info.IdReserva);
            }
        }

        /// <summary>
        /// Maneja la visibilidad de los placeholders de los TextBox de búsqueda
        /// </summary>
        /// <param name="sender">TextBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                // Determinar cuál placeholder corresponde
                if (textBox.Name == "txtBuscarSocio")
                {
                    placeholderBuscarSocio.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                else if (textBox.Name == "txtBuscarActividad")
                {
                    placeholderBuscarActividad.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
    }
}

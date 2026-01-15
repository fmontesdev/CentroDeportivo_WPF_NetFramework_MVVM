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
    // Code-behind para ReservasView.xaml
    public partial class ReservasView : UserControl
    {
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

        // Abre la ventana modal para crear una nueva reserva
        private void VentanaNuevaReserva()
        {
            var ventana = new NuevaReservaWindow();
            
            // ShowDialog devuelve true si se guardó correctamente
            if (ventana.ShowDialog() == true)
            {
                // Recargar la lista de reservas
                ((ReservaViewModel)this.DataContext).ActualizarListaDespuesDeCrear();
            }
        }

        // Muestra un MessageBox de confirmación antes de cancelar
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

        // Maneja la visibilidad de los placeholders
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

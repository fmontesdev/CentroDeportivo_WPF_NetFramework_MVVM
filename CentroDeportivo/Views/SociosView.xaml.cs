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
    // Code-behind para SociosView.xaml
    public partial class SociosView : UserControl
    {
        // Propiedades privadas
        private readonly SocioViewModel _viewModel;

        // Constructor
        public SociosView()
        {
            InitializeComponent();

            // Crear e instanciar el ViewModel
            _viewModel = new SocioViewModel();

            // Suscribirse a eventos del ViewModel
            _viewModel.VentanaNuevoSocio += VentanaNuevoSocio;
            _viewModel.ConfirmarEliminar += ConfirmarEliminar;

            // Asignar DataContext
            this.DataContext = _viewModel;
        }

        // Abre la ventana modal para crear un nuevo socio
        private void VentanaNuevoSocio(object sender, EventArgs e)
        {
            var ventana = new NuevoSocioWindow();
            
            // ShowDialog devuelve true si se guardó correctamente
            if (ventana.ShowDialog() == true)
            {
                // Recargar la lista de socios
                _viewModel.ActualizarListaDespuesDeCrear();
            }
        }

        // Muestra un MessageBox de confirmación antes de eliminar
        private void ConfirmarEliminar(object sender, (int IdSocio, string Nombre) info)
        {
            var resultado = MessageBox.Show(
                $"¿Está seguro que desea eliminar al socio '{info.Nombre}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado == MessageBoxResult.Yes)
            {
                // Confirmar eliminación en el ViewModel pasando solo el ID
                _viewModel.ConfirmarEliminarSocio(info.IdSocio);
            }
        }

        // Limpia eventos al descargar el control para evitar memory leaks
        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            _viewModel.VentanaNuevoSocio -= VentanaNuevoSocio;
            _viewModel.ConfirmarEliminar -= ConfirmarEliminar;
        }
    }
}




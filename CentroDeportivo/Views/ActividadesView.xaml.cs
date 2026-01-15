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
    // Code-behind para ActividadesView.xaml
    public partial class ActividadesView : UserControl
    {
        public ActividadesView()
        {
            InitializeComponent();

            // Crear e instanciar el ViewModel (variable local)
            var viewModel = new ActividadViewModel();

            // Asignar Actions (más simple que eventos)
            viewModel.VentanaNuevaActividad = VentanaNuevaActividad;
            viewModel.ConfirmarEliminar = ConfirmarEliminar;

            // Asignar DataContext
            this.DataContext = viewModel;
        }

        // Abre la ventana modal para crear una nueva actividad
        private void VentanaNuevaActividad()
        {
            var ventana = new NuevaActividadWindow();
            
            // ShowDialog devuelve true si se guardó correctamente
            if (ventana.ShowDialog() == true)
            {
                // Recargar la lista de actividades
                ((ActividadViewModel)this.DataContext).InicializarAsync();
            }
        }

        // Muestra un MessageBox de confirmación antes de eliminar
        private void ConfirmarEliminar((int IdActividad, string Nombre) info)
        {
            var resultado = MessageBox.Show(
                $"¿Está seguro que desea eliminar la actividad '{info.Nombre}'?",
                "Confirmar eliminación",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (resultado == MessageBoxResult.Yes)
            {
                // Confirmar eliminación en el ViewModel pasando solo el ID
                ((ActividadViewModel)this.DataContext).ConfirmarEliminarActividad(info.IdActividad);
            }
        }
    }
}

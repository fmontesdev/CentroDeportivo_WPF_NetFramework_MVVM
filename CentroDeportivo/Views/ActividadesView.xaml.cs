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
    /// Code-behind para ActividadesView.xaml.
    /// Vista principal para la gestión de actividades del centro deportivo
    /// </summary>
    public partial class ActividadesView : UserControl
    {
        /// <summary>
        /// Constructor que inicializa la vista y configura el ViewModel
        /// </summary>
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

        /// <summary>
        /// Abre la ventana modal para crear una nueva actividad
        /// </summary>
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

        /// <summary>
        /// Muestra un diálogo de confirmación antes de eliminar una actividad
        /// </summary>
        /// <param name="info">Tupla con el ID y nombre de la actividad a eliminar</param>
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

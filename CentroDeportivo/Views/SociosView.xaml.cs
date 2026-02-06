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
    /// Code-behind para SociosView.xaml.
    /// Vista principal para la gestión de socios del centro deportivo
    /// </summary>
    public partial class SociosView : UserControl
    {
        /// <summary>
        /// Constructor que inicializa la vista y configura el ViewModel
        /// </summary>
        public SociosView()
        {
            InitializeComponent();

            // Crear e instanciar el ViewModel (variable local)
            var viewModel = new SocioViewModel();

            // Asignar Actions (más simple que eventos)
            viewModel.VentanaNuevoSocio = VentanaNuevoSocio;
            viewModel.ConfirmarEliminar = ConfirmarEliminar;

            // Asignar DataContext
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Abre la ventana modal para crear un nuevo socio
        /// </summary>
        private void VentanaNuevoSocio()
        {
            var ventana = new NuevoSocioWindow();
            
            // ShowDialog devuelve true si se guardó correctamente
            if (ventana.ShowDialog() == true)
            {
                // Recargar la lista de socios
                ((SocioViewModel)this.DataContext).InicializarAsync();
            }
        }

        /// <summary>
        /// Muestra un diálogo de confirmación antes de eliminar un socio
        /// </summary>
        /// <param name="info">Tupla con el ID y nombre del socio a eliminar</param>
        private void ConfirmarEliminar((int IdSocio, string Nombre) info)
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
                ((SocioViewModel)this.DataContext).ConfirmarEliminarSocio(info.IdSocio);
            }
        }
    }
}

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
using CentroDeportivo.Views;
using ViewModel;

namespace CentroDeportivo
{
    /// <summary>
    /// Code-behind para MainWindow.xaml.
    /// Ventana principal de la aplicación que gestiona la navegación entre vistas
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor que inicializa la ventana principal y configura el ViewModel
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // Crear ViewModel
            var viewModel = new MainViewModel();

            // Asignar Actions (más simple que eventos)
            viewModel.CambiarVista = CambiarVista;
            viewModel.SolicitarCerrar = () => Application.Current.Shutdown();

            // Asignar DataContext
            this.DataContext = viewModel;

            // Inicializar vista por defecto
            viewModel.InicializarVistaInicial();
        }

        /// <summary>
        /// Maneja el cambio de vista en el panel de contenido principal
        /// </summary>
        /// <param name="nombreVista">Nombre de la vista a mostrar (Reservas, Socios, Actividades, Informes)</param>
        private void CambiarVista(string nombreVista)
        {
            switch (nombreVista)
            {
                case "Reservas":
                    MainContent.Content = new ReservasView();
                    break;

                case "Socios":
                    MainContent.Content = new SociosView();
                    break;

                case "Actividades":
                    MainContent.Content = new ActividadesView();
                    break;

                case "Informes":
                    MainContent.Content = new InformesView();
                    break;
            }
        }
    }
}

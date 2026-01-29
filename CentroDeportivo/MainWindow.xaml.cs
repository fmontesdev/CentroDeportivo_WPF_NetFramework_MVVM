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
    // Code-behind para MainWindow.xaml
    public partial class MainWindow : Window
    {
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

        // Maneja el cambio de vista (responsabilidad de la Vista)
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

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

namespace CentroDeportivo
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            
            // Cargar vista de Socios por defecto (Reservas aún no implementada)
            MostrarVistaSocios();
        }

        /// <summary>
        /// Muestra la vista de Reservas
        /// </summary>
        private void BtnReservas_Click(object sender, RoutedEventArgs e)
        {
            MostrarVistaReservas();
        }

        /// <summary>
        /// Muestra la vista de Socios
        /// </summary>
        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            MostrarVistaSocios();
        }

        /// <summary>
        /// Muestra la vista de Actividades
        /// </summary>
        private void BtnViajes_Click(object sender, RoutedEventArgs e)
        {
            MostrarVistaActividades();
        }

        /// <summary>
        /// Cierra la aplicación
        /// </summary>
        private void BtnSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Métodos auxiliares para cambiar vistas
        private void MostrarVistaReservas()
        {
            // TODO: Implementar ReservasView
            // MainContent.Content = new ReservasView();
            MainContent.Content = new TextBlock
            {
                Text = "Vista de Reservas - Pendiente de implementar",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18
            };
            txtTitulo.Text = "Gestión de Reservas";
        }

        private void MostrarVistaSocios()
        {
            MainContent.Content = new SociosView();
            txtTitulo.Text = "Gestión de Socios";
        }

        private void MostrarVistaActividades()
        {
            // TODO: Implementar ActividadesView
            // MainContent.Content = new ActividadesView();
            MainContent.Content = new TextBlock
            {
                Text = "Vista de Actividades - Pendiente de implementar",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                FontSize = 18
            };
            txtTitulo.Text = "Gestión de Actividades";
        }
    }
}


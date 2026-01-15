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
using System.Windows.Shapes;
using ViewModel;

namespace CentroDeportivo.Windows
{
    // Code-behind para NuevoSocioWindow.xaml
    public partial class NuevoSocioWindow : Window
    {
        // Constructor
        public NuevoSocioWindow()
        {
            InitializeComponent();

            // Crear ViewModel
            var viewModel = new NuevoSocioViewModel();

            // Asignar acciones para cerrar la ventana (más simple que eventos)
            viewModel.CerrarVentanaExito = () => { DialogResult = true; Close(); };
            viewModel.CerrarVentanaCancelar = () => { DialogResult = false; Close(); };

            // Asignar DataContext
            this.DataContext = viewModel;
        }

        // Maneja la visibilidad de los placeholders
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                // Determinar cuál placeholder corresponde
                if (textBox.Name == "txtNombre")
                {
                    placeholderNombre.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                else if (textBox.Name == "txtEmail")
                {
                    placeholderEmail.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
    }
}



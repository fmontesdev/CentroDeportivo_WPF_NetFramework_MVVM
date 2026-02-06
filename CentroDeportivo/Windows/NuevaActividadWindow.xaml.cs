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
    /// <summary>
    /// Code-behind para NuevaActividadWindow.xaml.
    /// Ventana modal para crear una nueva actividad con validación de formulario
    /// </summary>
    public partial class NuevaActividadWindow : Window
    {
        /// <summary>
        /// Constructor que inicializa la ventana y configura el ViewModel
        /// </summary>
        public NuevaActividadWindow()
        {
            InitializeComponent();

            // Crear ViewModel
            var viewModel = new NuevaActividadViewModel();

            // Asignar acciones para cerrar la ventana (más simple que eventos)
            viewModel.CerrarVentanaExito = () => { DialogResult = true; Close(); };
            viewModel.CerrarVentanaCancelar = () => { DialogResult = false; Close(); };

            // Asignar DataContext
            this.DataContext = viewModel;
        }

        /// <summary>
        /// Maneja la visibilidad de los placeholders de los TextBox
        /// </summary>
        /// <param name="sender">TextBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                // Determinar cuál placeholder corresponde
                if (textBox.Name == "txtNombreActividad")
                {
                    placeholderNombreActividad.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                else if (textBox.Name == "txtAforo")
                {
                    placeholderAforo.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }
    }
}


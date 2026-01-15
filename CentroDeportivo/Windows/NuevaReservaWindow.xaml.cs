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
    // Code-behind para NuevaReservaWindow.xaml
    public partial class NuevaReservaWindow : Window
    {
        private NuevaReservaViewModel _viewModel;

        // Constructor
        public NuevaReservaWindow()
        {
            InitializeComponent();

            // Crear ViewModel
            _viewModel = new NuevaReservaViewModel();

            // Asignar acciones para cerrar la ventana (más simple que eventos)
            _viewModel.CerrarVentanaExito = () => { DialogResult = true; Close(); };
            _viewModel.CerrarVentanaCancelar = () => { DialogResult = false; Close(); };

            // Asignar DataContext
            this.DataContext = _viewModel;
        }

        // Maneja la visibilidad de los placeholders de los ComboBox
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                // El TextBox interno del ComboBox tiene nombre "PART_EditableTextBox"
                // Identificamos el ComboBox por el placeholder que le corresponde
                if (cbSocio.Template.FindName("PART_EditableTextBox", cbSocio) == textBox)
                {
                    placeholderSocio.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
                else if (cbActividad.Template.FindName("PART_EditableTextBox", cbActividad) == textBox)
                {
                    placeholderActividad.Visibility = string.IsNullOrEmpty(textBox.Text)
                        ? Visibility.Visible
                        : Visibility.Collapsed;
                }
            }
        }

        // ==================== EVENTOS DE SOCIO ====================

        // Evento para la selección del socio al hacer clic en el ComboBox
        private void CbSocio_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Selecciona todo el texto del ComboBox editable
            if (cbSocio.IsEditable && cbSocio.Template.FindName("PART_EditableTextBox", cbSocio) is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        // Evento para filtrar los socios al escribir en el ComboBox
        private void CbSocio_KeyUp(object sender, KeyEventArgs e)
        {
            // Obtiene el criterio de filtrado
            string criterioSocio = cbSocio.Text;

            // Delegar el filtrado al ViewModel
            _viewModel.FiltrarSocios(criterioSocio);

            // Abre el dropdown
            cbSocio.IsDropDownOpen = true;

            // Restaura el texto
            cbSocio.Text = criterioSocio;

            // Pone el cursor al final del texto
            if (cbSocio.Template.FindName("PART_EditableTextBox", cbSocio) is TextBox textBox)
            {
                textBox.SelectionStart = criterioSocio.Length;
                
                // Suscribir al TextChanged si no está ya suscrito
                textBox.TextChanged -= ComboBox_TextChanged;
                textBox.TextChanged += ComboBox_TextChanged;
            }

            // Si no hay resultados, limpia la selección
            if (_viewModel.SociosCount == 0)
            {
                cbSocio.SelectedIndex = -1;
            }
        }

        // ==================== EVENTOS DE ACTIVIDAD ====================

        // Evento para la selección de la actividad al hacer clic en el ComboBox
        private void CbActividad_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Selecciona todo el texto del ComboBox editable
            if (cbActividad.IsEditable && cbActividad.Template.FindName("PART_EditableTextBox", cbActividad) is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        // Evento para filtrar las actividades al escribir en el ComboBox
        private void CbActividad_KeyUp(object sender, KeyEventArgs e)
        {
            // Obtiene el criterio de filtrado
            string criterioActividad = cbActividad.Text;

            // Delegar el filtrado al ViewModel
            _viewModel.FiltrarActividades(criterioActividad);

            // Abre el dropdown
            cbActividad.IsDropDownOpen = true;

            // Restaura el texto
            cbActividad.Text = criterioActividad;

            // Pone el cursor al final del texto
            if (cbActividad.Template.FindName("PART_EditableTextBox", cbActividad) is TextBox textBox)
            {
                textBox.SelectionStart = criterioActividad.Length;
                
                // Suscribir al TextChanged si no está ya suscrito
                textBox.TextChanged -= ComboBox_TextChanged;
                textBox.TextChanged += ComboBox_TextChanged;
            }

            // Si no hay resultados, limpia la selección
            if (_viewModel.ActividadesCount == 0)
            {
                cbActividad.SelectedIndex = -1;
            }
        }
    }
}



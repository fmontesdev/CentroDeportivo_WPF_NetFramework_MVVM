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
    /// Code-behind para NuevaReservaWindow.xaml.
    /// Ventana modal para crear una nueva reserva con ComboBox filtrables de socios y actividades
    /// </summary>
    public partial class NuevaReservaWindow : Window
    {
        private NuevaReservaViewModel _viewModel;

        /// <summary>
        /// Constructor que inicializa la ventana, configura el ViewModel y suscribe eventos de filtrado
        /// </summary>
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

            // Suscribir eventos TextChanged de los ComboBox al cargar la ventana
            this.Loaded += (s, e) =>
            {
                // Suscribir el evento TextChanged del ComboBox de Socio cuando el template esté listo
                if (cbSocio.Template.FindName("PART_EditableTextBox", cbSocio) is TextBox textBoxSocio)
                {
                    textBoxSocio.TextChanged += ComboBox_TextChanged;
                }

                // Suscribir el evento TextChanged del ComboBox de Actividad cuando el template esté listo
                if (cbActividad.Template.FindName("PART_EditableTextBox", cbActividad) is TextBox textBoxActividad)
                {
                    textBoxActividad.TextChanged += ComboBox_TextChanged;
                }
            };
        }

        /// <summary>
        /// Maneja el cambio de texto en los ComboBox editables y actualiza la visibilidad de placeholders
        /// </summary>
        /// <param name="sender">TextBox interno del ComboBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void ComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;

            if (textBox != null)
            {
                // El TextBox interno del ComboBox tiene nombre "PART_EditableTextBox"
                // Identificamos el ComboBox por el placeholder que le corresponde
                if (cbSocio.Template.FindName("PART_EditableTextBox", cbSocio) == textBox)
                {
                    ActualizarVisibilidadPlaceholder(cbSocio, placeholderSocio);
                }
                else if (cbActividad.Template.FindName("PART_EditableTextBox", cbActividad) == textBox)
                {
                    ActualizarVisibilidadPlaceholder(cbActividad, placeholderActividad);
                }
            }
        }

        /// <summary>
        /// Maneja el cambio de selección en el ComboBox de Socio y actualiza el placeholder
        /// </summary>
        /// <param name="sender">ComboBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void CbSocio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarVisibilidadPlaceholder(cbSocio, placeholderSocio);
        }

        /// <summary>
        /// Maneja el cambio de selección en el ComboBox de Actividad y actualiza el placeholder
        /// </summary>
        /// <param name="sender">ComboBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void CbActividad_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ActualizarVisibilidadPlaceholder(cbActividad, placeholderActividad);
        }

        /// <summary>
        /// Actualiza la visibilidad del placeholder de un ComboBox según su estado.
        /// Oculta el placeholder si hay un elemento seleccionado o texto escrito
        /// </summary>
        /// <param name="comboBox">ComboBox a evaluar</param>
        /// <param name="placeholder">TextBlock del placeholder asociado</param>
        private void ActualizarVisibilidadPlaceholder(ComboBox comboBox, TextBlock placeholder)
        {
            // Ocultar el placeholder si hay un elemento seleccionado o si hay texto en el ComboBox
            bool tieneSeleccion = comboBox.SelectedItem != null;
            bool tieneTexto = false;

            if (comboBox.Template.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
            {
                tieneTexto = !string.IsNullOrEmpty(textBox.Text);
            }

            placeholder.Visibility = (tieneSeleccion || tieneTexto)
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Selecciona todo el texto del ComboBox de Socio al hacer clic para facilitar la edición
        /// </summary>
        /// <param name="sender">ComboBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void CbSocio_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Selecciona todo el texto del ComboBox editable
            if (cbSocio.IsEditable && cbSocio.Template.FindName("PART_EditableTextBox", cbSocio) is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Filtra los socios en tiempo real mientras el usuario escribe en el ComboBox.
        /// Delega la lógica de filtrado al ViewModel
        /// </summary>
        /// <param name="sender">ComboBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
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
            }

            // Si no hay resultados, limpia la selección
            if (_viewModel.SociosCount == 0)
            {
                cbSocio.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Selecciona todo el texto del ComboBox de Actividad al hacer clic para facilitar la edición
        /// </summary>
        /// <param name="sender">ComboBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
        private void CbActividad_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            // Selecciona todo el texto del ComboBox editable
            if (cbActividad.IsEditable && cbActividad.Template.FindName("PART_EditableTextBox", cbActividad) is TextBox textBox)
            {
                textBox.SelectAll();
            }
        }

        /// <summary>
        /// Filtra las actividades en tiempo real mientras el usuario escribe en el ComboBox.
        /// Delega la lógica de filtrado al ViewModel
        /// </summary>
        /// <param name="sender">ComboBox que disparó el evento</param>
        /// <param name="e">Argumentos del evento</param>
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
            }

            // Si no hay resultados, limpia la selección
            if (_viewModel.ActividadesCount == 0)
            {
                cbActividad.SelectedIndex = -1;
            }
        }
    }
}



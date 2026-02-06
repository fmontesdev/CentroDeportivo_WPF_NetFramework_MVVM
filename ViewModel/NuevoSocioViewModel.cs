using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Model;
using ViewModel.Command;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la ventana modal de creación de socios.
    /// Implementa validación y gestión de errores para el formulario de nuevo socio
    /// </summary>
    public class NuevoSocioViewModel : INotifyPropertyChanged
    {
        private readonly SocioService _socioService;
        private string _nombre;
        private string _email;
        private string _errorMessage;

        /// <summary>
        /// Nombre del socio enlazado al TextBox
        /// </summary>
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        /// <summary>
        /// Email del socio enlazado al TextBox
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        /// <summary>
        /// Mensaje de error para mostrar al usuario
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
                OnPropertyChanged(nameof(HasError));
            }
        }

        /// <summary>
        /// Indica si hay un error activo para mostrar en la interfaz
        /// </summary>
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        /// <summary>
        /// Comando para crear el socio y cerrar la ventana
        /// </summary>
        public ICommand CrearCommand { get; }

        /// <summary>
        /// Comando para cerrar la ventana sin guardar
        /// </summary>
        public ICommand CancelarCommand { get; }

        /// <summary>
        /// Constructor que inicializa el servicio y los comandos
        /// </summary>
        public NuevoSocioViewModel()
        {
            _socioService = new SocioService();

            // Inicializar Commands
            CrearCommand = new RelayCommand(CrearSocio);
            CancelarCommand = new RelayCommand(Cancelar);
        }

        /// <summary>
        /// Crea un nuevo socio en la base de datos después de validar el formulario
        /// </summary>
        private async void CrearSocio()
        {
            // Valida el formulario y crea el socio
            if (ValidarFormulario())
            {
                try
                {
                    // Limpiar mensaje de error previo
                    ErrorMessage = string.Empty;

                    // Crear nuevo socio
                    var nuevoSocio = new Socio
                    {
                        Nombre = Nombre.Trim(),
                        Email = Email.Trim(),
                        Activo = true
                    };

                    await _socioService.CrearSocioAsync(nuevoSocio);

                    // Notificar éxito y cerrar ventana
                    CerrarVentanaExito?.Invoke();
                }
                catch (ArgumentException ex)
                {
                    // Errores de validación del servicio
                    ErrorMessage = $"{ex.Message}";
                }
                catch (Exception ex)
                {
                    // Otros errores
                    ErrorMessage = $"Error al guardar el socio: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Valida los campos del formulario de creación de socio
        /// </summary>
        /// <returns>True si todos los datos son válidos, false si hay errores</returns>
        public bool ValidarFormulario()
        {
            // Patrón de email
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            // Validación del Nombre
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                ErrorMessage = "El nombre del socio es obligatorio";
                return false;
            }

            if (int.TryParse(Nombre, out _))
            {
                ErrorMessage = "El nombre del socio no puede ser solo números";
                return false;
            }

            if (Nombre.Trim().Length < 2)
            {
                ErrorMessage = "El nombre debe tener al menos 2 caracteres";
                return false;
            }

            // Validación del Email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "El email del socio es obligatorio";
                return false;
            }

            if (!Regex.IsMatch(Email, patron))
            {
                ErrorMessage = "El email debe tener un formato válido (ejemplo@email.com)";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Cancela la operación y cierra la ventana sin guardar
        /// </summary>
        private void Cancelar()
        {
            CerrarVentanaCancelar?.Invoke();
        }

        /// <summary>
        /// Acción para comunicar con la Vista y cerrar la ventana tras éxito
        /// </summary>
        public Action CerrarVentanaExito { get; set; }
        
        /// <summary>
        /// Acción para comunicar con la Vista y cerrar la ventana tras cancelar
        /// </summary>
        public Action CerrarVentanaCancelar { get; set; }

        /// <summary>
        /// Evento que notifica cambios en las propiedades para actualizar la interfaz
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Método auxiliar para invocar el evento PropertyChanged
        /// </summary>
        /// <param name="propertyName">Nombre de la propiedad que cambió</param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

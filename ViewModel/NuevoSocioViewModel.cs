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
    // ViewModel para la ventana de creación de socios (ventana modal)
    // Implementa INotifyPropertyChanged para notificar cambios a la UI
    // Usa validación con mensajes de error
    public class NuevoSocioViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private readonly SocioService _socioService;
        private string _nombre;
        private string _email;
        private string _errorMessage;

        // Nombre del socio (enlazado al TextBox)
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        // Email del socio (enlazado al TextBox)
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        // Mensaje de error
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

        // Indica si hay un error para mostrar en la UI
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        // Command para crear el socio
        public ICommand CrearCommand { get; }

        // Command para cerrar la ventana sin guardar
        public ICommand CancelarCommand { get; }

        // Constructor
        public NuevoSocioViewModel()
        {
            _socioService = new SocioService();

            // Inicializar Commands
            CrearCommand = new RelayCommand(CrearSocio);
            CancelarCommand = new RelayCommand(Cancelar);
        }

        // Crea el socio en la base de datos
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

        // Valida los campos del formulario
        // Devuelve true si todo es válido, false si hay errores
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

        // Cancela la operación y cierra la ventana
        private void Cancelar()
        {
            CerrarVentanaCancelar?.Invoke();
        }

        // Actions para cerrar la ventana (más simple que eventos)
        // La vista asigna estas acciones al crear el ViewModel
        public Action CerrarVentanaExito { get; set; }
        public Action CerrarVentanaCancelar { get; set; }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

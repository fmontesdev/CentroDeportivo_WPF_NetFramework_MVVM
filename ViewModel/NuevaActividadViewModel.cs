using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Model;
using ViewModel.Command;
using ViewModel.Services;

namespace ViewModel
{
    // ViewModel para la ventana de creación de actividades (ventana modal)
    // Implementa INotifyPropertyChanged para notificar cambios a la UI
    // Usa validación con mensajes de error
    public class NuevaActividadViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private readonly ActividadService _actividadService;
        private string _nombre;
        private string _aforoMaximo;
        private string _errorMessage;

        // Nombre de la actividad (enlazado al TextBox)
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        // Aforo máximo de la actividad (enlazado al TextBox)
        public string AforoMaximo
        {
            get => _aforoMaximo;
            set
            {
                _aforoMaximo = value;
                OnPropertyChanged(nameof(AforoMaximo));
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

        // Command para crear la actividad
        public ICommand CrearCommand { get; }

        // Command para cerrar la ventana sin guardar
        public ICommand CancelarCommand { get; }

        // Constructor
        public NuevaActividadViewModel()
        {
            _actividadService = new ActividadService();

            // Inicializar Commands
            CrearCommand = new RelayCommand(CrearActividad);
            CancelarCommand = new RelayCommand(Cancelar);
        }

        // Crea la actividad en la base de datos
        private async void CrearActividad()
        {
            // Valida el formulario y crea la actividad
            if (ValidarFormulario())
            {
                try
                {
                    // Limpiar mensaje de error previo
                    ErrorMessage = string.Empty;

                    // Crear nueva actividad
                    var nuevaActividad = new Actividad
                    {
                        Nombre = Nombre.Trim(),
                        AforoMaximo = int.Parse(AforoMaximo.Trim())
                    };

                    await _actividadService.CrearActividadAsync(nuevaActividad);

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
                    ErrorMessage = $"Error al guardar la actividad: {ex.Message}";
                }
            }
        }

        // Valida los campos del formulario
        // Devuelve true si todo es válido, false si hay errores
        private bool ValidarFormulario()
        {
            // Validación del Nombre
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                ErrorMessage = "El nombre de la actividad es obligatorio";
                return false;
            }

            if (Nombre.Trim().Length < 3)
            {
                ErrorMessage = "El nombre debe tener al menos 3 caracteres";
                return false;
            }

            // Validación del Aforo Máximo
            if (string.IsNullOrWhiteSpace(AforoMaximo))
            {
                ErrorMessage = "El aforo máximo es obligatorio";
                return false;
            }

            if (!int.TryParse(AforoMaximo, out int aforo))
            {
                ErrorMessage = "El aforo máximo debe ser un número entero";
                return false;
            }

            if (aforo <= 0)
            {
                ErrorMessage = "El aforo máximo debe ser mayor a 0";
                return false;
            }

            if (aforo > 1000)
            {
                ErrorMessage = "El aforo máximo no puede superar las 1000 personas";
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

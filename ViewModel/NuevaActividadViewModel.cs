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
    /// <summary>
    /// ViewModel para la ventana modal de creación de actividades.
    /// Implementa validación y gestión de errores para el formulario de nueva actividad
    /// </summary>
    public class NuevaActividadViewModel : INotifyPropertyChanged
    {
        private readonly ActividadService _actividadService;
        private string _nombre;
        private string _aforoMaximo;
        private string _errorMessage;

        /// <summary>
        /// Nombre de la actividad enlazado al TextBox
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
        /// Aforo máximo de la actividad enlazado al TextBox
        /// </summary>
        public string AforoMaximo
        {
            get => _aforoMaximo;
            set
            {
                _aforoMaximo = value;
                OnPropertyChanged(nameof(AforoMaximo));
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
        /// Comando para crear la actividad y cerrar la ventana
        /// </summary>
        public ICommand CrearCommand { get; }

        /// <summary>
        /// Comando para cerrar la ventana sin guardar
        /// </summary>
        public ICommand CancelarCommand { get; }

        /// <summary>
        /// Constructor que inicializa el servicio y los comandos
        /// </summary>
        public NuevaActividadViewModel()
        {
            _actividadService = new ActividadService();

            // Inicializar Commands
            CrearCommand = new RelayCommand(CrearActividad);
            CancelarCommand = new RelayCommand(Cancelar);
        }

        /// <summary>
        /// Crea una nueva actividad en la base de datos después de validar el formulario
        /// </summary>
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

        /// <summary>
        /// Valida los campos del formulario de creación de actividad
        /// </summary>
        /// <returns>True si todos los datos son válidos, false si hay errores</returns>
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

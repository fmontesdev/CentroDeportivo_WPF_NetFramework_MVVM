using Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ViewModel.Command;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la ventana modal de creación de reservas.
    /// Implementa validación, filtrado de socios/actividades y gestión de errores
    /// </summary>
    public class NuevaReservaViewModel : INotifyPropertyChanged
    {
        private readonly ReservaService _reservaService;
        private readonly SocioService _socioService;
        private readonly ActividadService _actividadService;
        private Socio _socioSeleccionado;
        private Actividad _actividadSeleccionada;
        private DateTime? _fechaReserva;
        private string _errorMessage;

        private List<Socio> _todosLosSocios;
        private List<Actividad> _todasLasActividades;

        /// <summary>
        /// Colección observable de socios para el ComboBox con filtrado
        /// </summary>
        public ObservableCollection<Socio> Socios { get; set; }
        
        /// <summary>
        /// Colección observable de actividades para el ComboBox con filtrado
        /// </summary>
        public ObservableCollection<Actividad> Actividades { get; set; }

        /// <summary>
        /// Número de socios en la colección filtrada
        /// </summary>
        public int SociosCount => Socios?.Count ?? 0;
        
        /// <summary>
        /// Número de actividades en la colección filtrada
        /// </summary>
        public int ActividadesCount => Actividades?.Count ?? 0;

        /// <summary>
        /// Socio seleccionado en el ComboBox
        /// </summary>
        public Socio SocioSeleccionado
        {
            get => _socioSeleccionado;
            set
            {
                _socioSeleccionado = value;
                OnPropertyChanged(nameof(SocioSeleccionado));
            }
        }

        /// <summary>
        /// Actividad seleccionada en el ComboBox
        /// </summary>
        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));
            }
        }

        /// <summary>
        /// Fecha de la reserva enlazada al DatePicker
        /// </summary>
        public DateTime? FechaReserva
        {
            get => _fechaReserva;
            set
            {
                _fechaReserva = value;
                OnPropertyChanged(nameof(FechaReserva));
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
        /// Comando para crear la reserva y cerrar la ventana
        /// </summary>
        public ICommand CrearCommand { get; }

        /// <summary>
        /// Comando para cerrar la ventana sin guardar
        /// </summary>
        public ICommand CancelarCommand { get; }

        /// <summary>
        /// Constructor que inicializa los servicios, comandos y carga los datos iniciales
        /// </summary>
        public NuevaReservaViewModel()
        {
            _reservaService = new ReservaService();
            _socioService = new SocioService();
            _actividadService = new ActividadService();
            
            Socios = new ObservableCollection<Socio>();
            Actividades = new ObservableCollection<Actividad>();

            // Inicializar Commands
            CrearCommand = new RelayCommand(CrearReserva);
            CancelarCommand = new RelayCommand(Cancelar);

            // Establecer fecha por defecto (hoy)
            FechaReserva = DateTime.Today;

            // Cargar datos
            CargarDatos();
        }

        /// <summary>
        /// Carga los socios activos y actividades disponibles desde la base de datos
        /// </summary>
        private async void CargarDatos()
        {
            try
            {
                // Cargar socios activos
                var socios = await _socioService.ObtenerSociosActivosAsync();
                _todosLosSocios = socios; // Guardar lista completa
                
                Socios.Clear();
                foreach (var socio in socios)
                {
                    Socios.Add(socio);
                }

                // Cargar actividades
                var actividades = await _actividadService.ObtenerActividadesAsync();
                _todasLasActividades = actividades; // Guardar lista completa
                
                Actividades.Clear();
                foreach (var actividad in actividades)
                {
                    Actividades.Add(actividad);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar datos: {ex.Message}";
            }
        }

        /// <summary>
        /// Filtra los socios según el criterio de búsqueda.
        /// Método público para que la vista pueda invocar el filtrado
        /// </summary>
        /// <param name="criterio">Texto de búsqueda para filtrar por nombre de socio</param>
        public void FiltrarSocios(string criterio)
        {
            // Si la lista de socios está vacia, no hacer nada
            if (_todosLosSocios != null)
            {
                var listaFiltrada = _todosLosSocios;

                // Si hay criterio, filtrar por nombre (case-insensitive)
                if (!string.IsNullOrEmpty(criterio))
                {
                    listaFiltrada = listaFiltrada
                        .Where(s => s.Nombre.ToUpper().Contains(criterio.Trim().ToUpper()))
                        .ToList();
                }

                // Actualizar la colección observable
                Socios.Clear();
                foreach (var socio in listaFiltrada)
                {
                    Socios.Add(socio);
                }

                // Notificar cambio en el conteo
                OnPropertyChanged(nameof(SociosCount));
            }
        }

        /// <summary>
        /// Filtra las actividades según el criterio de búsqueda.
        /// Método público para que la vista pueda invocar el filtrado
        /// </summary>
        /// <param name="criterio">Texto de búsqueda para filtrar por nombre de actividad</param>
        public void FiltrarActividades(string criterio)
        {
            // Si la lista de actividades está vacia, no hacer nada
            if (_todasLasActividades != null)
            {
                var listaFiltrada = _todasLasActividades;

                // Si hay criterio, filtrar por nombre (case-insensitive)
                if (!string.IsNullOrEmpty(criterio))
                {
                    listaFiltrada = listaFiltrada
                        .Where(a => a.AforoMaximo > 0 && a.Nombre.ToUpper().Contains(criterio.Trim().ToUpper()))
                        .ToList();
                }

                // Actualizar la colección observable
                Actividades.Clear();
                foreach (var actividad in listaFiltrada)
                {
                    Actividades.Add(actividad);
                }

                // Notificar cambio en el conteo
                OnPropertyChanged(nameof(ActividadesCount));
            }
        }

        /// <summary>
        /// Crea una nueva reserva en la base de datos después de validar el formulario
        /// </summary>
        private async void CrearReserva()
        {
            // Valida el formulario y crea la reserva
            if (ValidarFormulario())
            {
                try
                {
                    // Limpiar mensaje de error previo
                    ErrorMessage = string.Empty;

                    // Crear nueva reserva
                    var nuevaReserva = new Reserva
                    {
                        IdSocio = SocioSeleccionado.IdSocio,
                        IdActividad = ActividadSeleccionada.IdActividad,
                        Fecha = FechaReserva.Value
                    };

                    await _reservaService.CrearReservaAsync(nuevaReserva);

                    // Notificar éxito y cerrar ventana
                    CerrarVentanaExito?.Invoke();
                }
                catch (ArgumentException ex)
                {
                    // Errores de validación del servicio
                    ErrorMessage = $"{ex.Message}";
                }
                catch (InvalidOperationException ex)
                {
                    // Errores de negocio (aforo completo)
                    ErrorMessage = $"{ex.Message}";
                }
                catch (Exception ex)
                {
                    // Otros errores
                    ErrorMessage = $"Error al guardar la reserva: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Valida los campos del formulario de creación de reserva
        /// </summary>
        /// <returns>True si todos los datos son válidos, false si hay errores</returns>
        public bool ValidarFormulario()
        {
            // Validación del Socio
            if (SocioSeleccionado == null)
            {
                ErrorMessage = "Debe seleccionar un socio";
                return false;
            }

            // Validación de la Actividad
            if (ActividadSeleccionada == null)
            {
                ErrorMessage = "Debe seleccionar una actividad";
                return false;
            }

            // Validación de la Fecha
            if (FechaReserva == null)
            {
                ErrorMessage = "Debe seleccionar una fecha de reserva";
                return false;
            }

            if (FechaReserva.Value.Date < DateTime.Today)
            {
                ErrorMessage = "La fecha de reserva no puede ser anterior a hoy";
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

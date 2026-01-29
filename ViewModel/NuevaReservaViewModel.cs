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
    // ViewModel para la ventana de creación de reservas (ventana modal)
    // Implementa INotifyPropertyChanged para notificar cambios a la UI
    // Usa validación con mensajes de error
    public class NuevaReservaViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private readonly ReservaService _reservaService;
        private readonly SocioService _socioService;
        private readonly ActividadService _actividadService;
        private Socio _socioSeleccionado;
        private Actividad _actividadSeleccionada;
        private DateTime? _fechaReserva;
        private string _errorMessage;

        // Listas completas para filtrado (privadas, solo accesibles desde métodos del ViewModel)
        private List<Socio> _todosLosSocios;
        private List<Actividad> _todasLasActividades;

        // Colecciones para los ComboBox
        public ObservableCollection<Socio> Socios { get; set; }
        public ObservableCollection<Actividad> Actividades { get; set; }

        // Propiedades adicionales para que la vista pueda consultar el Count sin conocer los tipos
        public int SociosCount => Socios?.Count ?? 0;
        public int ActividadesCount => Actividades?.Count ?? 0;

        // Socio seleccionado (enlazado al ComboBox)
        public Socio SocioSeleccionado
        {
            get => _socioSeleccionado;
            set
            {
                _socioSeleccionado = value;
                OnPropertyChanged(nameof(SocioSeleccionado));
            }
        }

        // Actividad seleccionada (enlazado al ComboBox)
        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));
            }
        }

        // Fecha de reserva (enlazada al DatePicker)
        public DateTime? FechaReserva
        {
            get => _fechaReserva;
            set
            {
                _fechaReserva = value;
                OnPropertyChanged(nameof(FechaReserva));
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

        // Command para crear la reserva
        public ICommand CrearCommand { get; }

        // Command para cerrar la ventana sin guardar
        public ICommand CancelarCommand { get; }

        // Constructor
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

        // Carga los socios y actividades disponibles
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

        // Filtra los socios según el criterio de búsqueda
        // Método público para que la vista pueda invocar el filtrado
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

        // Filtra las actividades según el criterio de búsqueda
        // Método público para que la vista pueda invocar el filtrado
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

        // Crea la reserva en la base de datos
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

        // Valida los campos del formulario
        // Devuelve true si todo es válido, false si hay errores
        private bool ValidarFormulario()
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

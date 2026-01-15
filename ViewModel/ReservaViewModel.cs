using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;
using ViewModel.Command;
using ViewModel.Services;

namespace ViewModel
{
    // ViewModel para la vista principal de gestión de reservas (ReservasView)
    // Maneja la lista de reservas, selección y operaciones CRUD
    public class ReservaViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private readonly ReservaService _reservaService;
        private Reserva _reservaSeleccionada;
        private DateTime? _fechaReserva;
        private string _buscarSocio;
        private string _buscarActividad;
        private string _errorMessage;
        private List<Reserva> _todasLasReservas; // Lista completa para filtrado

        // Colección observable de reservas para el DataGrid
        // Actualiza la UI automáticamente
        public ObservableCollection<Reserva> Reservas { get; set; }

        // Reserva seleccionada en el DataGrid
        // Al seleccionar una reserva, se cargan sus datos en los campos
        public Reserva ReservaSeleccionada
        {
            get => _reservaSeleccionada;
            set
            {
                _reservaSeleccionada = value;
                OnPropertyChanged(nameof(ReservaSeleccionada));
                
                // Cargar datos en los campos cuando se selecciona una reserva
                if (_reservaSeleccionada != null)
                {
                    FechaReserva = _reservaSeleccionada.Fecha;
                    SocioNombre = _reservaSeleccionada.Socio?.Nombre ?? "Sin socio";
                    ActividadNombre = _reservaSeleccionada.Actividad?.Nombre ?? "Sin actividad";
                }
                else
                {
                    LimpiarFormulario();
                }
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

        // Nombre del socio (solo lectura)
        private string _socioNombre;
        public string SocioNombre
        {
            get => _socioNombre;
            set
            {
                _socioNombre = value;
                OnPropertyChanged(nameof(SocioNombre));
            }
        }

        // Nombre de la actividad (solo lectura)
        private string _actividadNombre;
        public string ActividadNombre
        {
            get => _actividadNombre;
            set
            {
                _actividadNombre = value;
                OnPropertyChanged(nameof(ActividadNombre));
            }
        }

        // Filtro de búsqueda por socio
        public string BuscarSocio
        {
            get => _buscarSocio;
            set
            {
                _buscarSocio = value;
                OnPropertyChanged(nameof(BuscarSocio));
                AplicarFiltros();
            }
        }

        // Filtro de búsqueda por actividad
        public string BuscarActividad
        {
            get => _buscarActividad;
            set
            {
                _buscarActividad = value;
                OnPropertyChanged(nameof(BuscarActividad));
                AplicarFiltros();
            }
        }

        // Total de reservas (para mostrar en la UI)
        public int TotalReservas => Reservas?.Count ?? 0;

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

        // Indica si hay un error para mostrar
        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        // Command para crear una nueva reserva (abre ventana modal)
        public ICommand NuevaCommand { get; }

        // Command para editar la reserva seleccionada
        public ICommand EditarCommand { get; }

        // Command para cancelar/eliminar la reserva seleccionada
        public ICommand CancelarCommand { get; }

        // Command para limpiar filtros de búsqueda
        public ICommand LimpiarBusquedaCommand { get; }

        // Constructor
        public ReservaViewModel()
        {
            _reservaService = new ReservaService();
            Reservas = new ObservableCollection<Reserva>();
            _todasLasReservas = new List<Reserva>();

            // Inicializar Commands
            NuevaCommand = new RelayCommand(NuevaReserva);
            EditarCommand = new RelayCommand(EditarReserva);
            CancelarCommand = new RelayCommand(CancelarReserva);
            LimpiarBusquedaCommand = new RelayCommand(LimpiarBusqueda);

            // Valores iniciales
            SocioNombre = "Socio";
            ActividadNombre = "Actividad";

            // Cargar reservas al inicializar
            InicializarAsync();
        }

        // Inicializa la carga de datos de forma asíncrona
        public async void InicializarAsync()
        {
            await CargarReservasAsync();
            SeleccionarPrimero();
        }

        // Carga la lista de reservas desde la base de datos
        private async Task CargarReservasAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                var reservas = await _reservaService.ObtenerReservasAsync();
                _todasLasReservas = reservas;

                Reservas.Clear();
                foreach (var reserva in reservas)
                {
                    Reservas.Add(reserva);
                }

                OnPropertyChanged(nameof(TotalReservas));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar reservas: {ex.Message}";
            }
        }

        // Selecciona la primera reserva de la lista
        private void SeleccionarPrimero()
        {
            if (Reservas.Count > 0)
            {
                ReservaSeleccionada = Reservas[0];
            }
            else
            {
                ReservaSeleccionada = null;
            }
        }

        // Mantiene la selección de la reserva en el índice especificado
        // Si el índice es inválido, selecciona la primera
        private void MantieneSeleccion(int indice)
        {
            if (Reservas.Count == 0)
            {
                ReservaSeleccionada = null;
            }
            else if (indice >= 0 && indice < Reservas.Count)
            {
                ReservaSeleccionada = Reservas[indice];
            }
            else
            {
                SeleccionarPrimero();
            }
        }

        // Selecciona la reserva anterior al índice especificado al eliminar
        // Si se eliminó la primera, selecciona la nueva primera
        // Si no quedan reservas, no selecciona ninguna
        private void SeleccionaAnterior(int indiceEliminado)
        {
            if (Reservas.Count == 0)
            {
                ReservaSeleccionada = null;
            }
            else if (indiceEliminado == 0)
            {
                // Se eliminó la primera, seleccionar la nueva primera
                ReservaSeleccionada = Reservas[0];
            }
            else
            {
                // Seleccionar la anterior, pero asegurarse de no exceder los límites
                int nuevoIndice = indiceEliminado - 1;
                if (nuevoIndice >= Reservas.Count)
                {
                    nuevoIndice = Reservas.Count - 1;
                }
                ReservaSeleccionada = Reservas[nuevoIndice];
            }
        }

        // Aplica los filtros de búsqueda
        private void AplicarFiltros()
        {
            // Verifica que la lista completa no sea nula
            if (_todasLasReservas != null)
            {
                var reservasFiltradas = _todasLasReservas.AsEnumerable();

                // Filtrar por socio
                if (!string.IsNullOrWhiteSpace(BuscarSocio))
                {
                    reservasFiltradas = reservasFiltradas
                        .Where(r => r.Socio != null && r.Socio.Nombre.ToUpper().Contains(BuscarSocio.Trim().ToUpper()))
                        .ToList();
                }

                // Filtrar por actividad
                if (!string.IsNullOrWhiteSpace(BuscarActividad))
                {
                    reservasFiltradas = reservasFiltradas
                        .Where(r => r.Actividad != null && r.Actividad.Nombre.ToUpper().Contains(BuscarActividad.Trim().ToUpper()))
                        .ToList();
                }

                // Actualizar colección
                Reservas.Clear();
                foreach (var reserva in reservasFiltradas)
                {
                    Reservas.Add(reserva);
                }

                OnPropertyChanged(nameof(TotalReservas));
            }
        }

        // Limpia los filtros de búsqueda
        private void LimpiarBusqueda()
        {
            BuscarSocio = string.Empty;
            BuscarActividad = string.Empty;
        }

        // Abre la ventana modal para crear una nueva reserva
        private void NuevaReserva()
        {
            VentanaNuevaReserva?.Invoke();
        }

        // Edita la reserva seleccionada con los datos del DatePicker
        private async void EditarReserva()
        {
            // Verifica que haya una fila seleccionada
            if (ReservaSeleccionada == null)
            {                 
                ErrorMessage = "No hay ninguna reserva seleccionada para editar";
            }
            else if (ValidarFormulario())
            {
                try
                {
                    ErrorMessage = string.Empty;

                    // Guardar el índice de la reserva seleccionada para mantener la selección
                    int indiceReservaActual = Reservas.IndexOf(ReservaSeleccionada);

                    // Actualizar propiedades de la reserva seleccionada
                    ReservaSeleccionada.Fecha = FechaReserva.Value;

                    // Guardar en base de datos
                    await _reservaService.ActualizarReservaAsync(ReservaSeleccionada);
                    
                    // Recargar y mantener la selección de la reserva editada
                    await CargarReservasAsync();
                    MantieneSeleccion(indiceReservaActual);
                }
                catch (ArgumentException ex)
                {
                    ErrorMessage = $"{ex.Message}";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al editar reserva: {ex.Message}";
                }
            }
        }

        // Cancela/elimina la reserva seleccionada después de confirmación
        private void CancelarReserva()
        {
            if (ReservaSeleccionada == null)
            {
                ErrorMessage = "No hay ninguna reserva seleccionada para cancelar";
            }
            else
            {
                ErrorMessage = string.Empty;

                ConfirmarCancelar?.Invoke((
                    ReservaSeleccionada.IdReserva,
                    ReservaSeleccionada.Socio?.Nombre ?? "Desconocido",
                    ReservaSeleccionada.Actividad?.Nombre ?? "Desconocida"
                ));
            }
        }

        // Confirma y ejecuta la cancelación de la reserva
        public async void ConfirmarCancelarReserva(int idReserva)
        {
            try
            {
                // Usar el objeto ReservaSeleccionada que ya tenemos disponible
                if (ReservaSeleccionada == null || ReservaSeleccionada.IdReserva != idReserva)
                {
                    ErrorMessage = "La reserva seleccionada no coincide con la reserva a cancelar";
                }
                else
                {
                    // Guardar el índice de la reserva que vamos a eliminar
                    int indiceReservaEliminada = Reservas.IndexOf(ReservaSeleccionada);

                    await _reservaService.EliminarReservaAsync(ReservaSeleccionada);

                    // Recargar y seleccionar la reserva anterior
                    await CargarReservasAsync();
                    SeleccionaAnterior(indiceReservaEliminada);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cancelar reserva: {ex.Message}";
            }
        }

        // Valida los datos del formulario
        private bool ValidarFormulario()
        {
            // Validación de la Fecha
            if (FechaReserva == null)
            {
                ErrorMessage = "La fecha de reserva es obligatoria";
                return false;
            }

            if (FechaReserva.Value.Date < DateTime.Today)
            {
                ErrorMessage = "La fecha de reserva no puede ser anterior a hoy";
                return false;
            }

            return true;
        }

        // Limpia los campos del formulario
        private void LimpiarFormulario()
        {
            FechaReserva = null;
            SocioNombre = "Socio";
            ActividadNombre = "Actividad";
        }

        // Actions para comunicar con la Vista (más simple que eventos)
        public Action VentanaNuevaReserva { get; set; }
        public Action<(int IdReserva, string NombreSocio, string NombreActividad)> ConfirmarCancelar { get; set; }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

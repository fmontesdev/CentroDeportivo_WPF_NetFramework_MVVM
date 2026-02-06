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
    /// <summary>
    /// ViewModel para la vista principal de gestión de reservas (ReservasView).
    /// Maneja la lista de reservas, selección, filtrado y operaciones CRUD
    /// </summary>
    public class ReservaViewModel : INotifyPropertyChanged
    {
        private readonly ReservaService _reservaService;
        private Reserva _reservaSeleccionada;
        private DateTime? _fechaReserva;
        private string _buscarSocio;
        private string _buscarActividad;
        private string _errorMessage;
        private List<Reserva> _todasLasReservas;

        /// <summary>
        /// Colección observable de reservas para el DataGrid.
        /// Se actualiza automáticamente en la interfaz de usuario
        /// </summary>
        public ObservableCollection<Reserva> Reservas { get; set; }

        /// <summary>
        /// Reserva seleccionada en el DataGrid.
        /// Al seleccionar una reserva, se cargan sus datos en los campos
        /// </summary>
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

        /// <summary>
        /// Fecha de la reserva enlazada al DatePicker de edición
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

        private string _socioNombre;
        /// <summary>
        /// Nombre del socio de la reserva seleccionada (solo lectura)
        /// </summary>
        public string SocioNombre
        {
            get => _socioNombre;
            set
            {
                _socioNombre = value;
                OnPropertyChanged(nameof(SocioNombre));
            }
        }

        private string _actividadNombre;
        /// <summary>
        /// Nombre de la actividad de la reserva seleccionada (solo lectura)
        /// </summary>
        public string ActividadNombre
        {
            get => _actividadNombre;
            set
            {
                _actividadNombre = value;
                OnPropertyChanged(nameof(ActividadNombre));
            }
        }

        /// <summary>
        /// Criterio de búsqueda para filtrar reservas por nombre de socio
        /// </summary>
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

        /// <summary>
        /// Criterio de búsqueda para filtrar reservas por nombre de actividad
        /// </summary>
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

        /// <summary>
        /// Total de reservas en la colección actual (considerando filtros aplicados)
        /// </summary>
        public int TotalReservas => Reservas?.Count ?? 0;

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
        /// Comando para crear una nueva reserva (abre ventana modal)
        /// </summary>
        public ICommand NuevaCommand { get; }

        /// <summary>
        /// Comando para editar la reserva seleccionada
        /// </summary>
        public ICommand EditarCommand { get; }

        /// <summary>
        /// Comando para cancelar/eliminar la reserva seleccionada
        /// </summary>
        public ICommand CancelarCommand { get; }

        /// <summary>
        /// Comando para limpiar los filtros de búsqueda
        /// </summary>
        public ICommand LimpiarBusquedaCommand { get; }

        /// <summary>
        /// Constructor que inicializa los servicios, comandos y carga los datos iniciales
        /// </summary>
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

        /// <summary>
        /// Inicializa la carga de datos de forma asíncrona y selecciona la primera reserva
        /// </summary>
        public async void InicializarAsync()
        {
            await CargarReservasAsync();
            SeleccionarPrimero();
        }

        /// <summary>
        /// Carga la lista de reservas desde la base de datos y actualiza la colección observable
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
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

        /// <summary>
        /// Selecciona la primera reserva de la lista
        /// </summary>
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

        /// <summary>
        /// Mantiene la selección de la reserva en el índice especificado.
        /// Si el índice es inválido, selecciona la primera
        /// </summary>
        /// <param name="indice">Índice de la reserva a seleccionar</param>
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

        /// <summary>
        /// Selecciona la reserva anterior al índice especificado al eliminar.
        /// Si se eliminó la primera, selecciona la nueva primera.
        /// Si no quedan reservas, no selecciona ninguna
        /// </summary>
        /// <param name="indiceEliminado">Índice de la reserva que fue eliminada</param>
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

        /// <summary>
        /// Aplica los filtros de búsqueda por socio y actividad sobre la lista completa de reservas
        /// </summary>
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

        /// <summary>
        /// Limpia los filtros de búsqueda y muestra todas las reservas
        /// </summary>
        private void LimpiarBusqueda()
        {
            BuscarSocio = string.Empty;
            BuscarActividad = string.Empty;
        }

        /// <summary>
        /// Abre la ventana modal para crear una nueva reserva
        /// </summary>
        private void NuevaReserva()
        {
            VentanaNuevaReserva?.Invoke();
        }

        /// <summary>
        /// Edita la reserva seleccionada con los datos del DatePicker,
        /// validando los datos y guardando los cambios en la base de datos
        /// </summary>
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

        /// <summary>
        /// Solicita confirmación para cancelar/eliminar la reserva seleccionada
        /// </summary>
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

        /// <summary>
        /// Confirma y ejecuta la cancelación/eliminación de la reserva de la base de datos
        /// </summary>
        /// <param name="idReserva">Identificador de la reserva a cancelar</param>
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

        /// <summary>
        /// Valida los datos del formulario de edición de reservas
        /// </summary>
        /// <returns>True si los datos son válidos, false en caso contrario</returns>
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

        /// <summary>
        /// Limpia los campos del formulario de edición
        /// </summary>
        private void LimpiarFormulario()
        {
            FechaReserva = null;
            SocioNombre = "Socio";
            ActividadNombre = "Actividad";
        }

        /// <summary>
        /// Acción para comunicar con la Vista y abrir la ventana de nueva reserva
        /// </summary>
        public Action VentanaNuevaReserva { get; set; }
        
        /// <summary>
        /// Acción para comunicar con la Vista y solicitar confirmación de cancelación
        /// </summary>
        public Action<(int IdReserva, string NombreSocio, string NombreActividad)> ConfirmarCancelar { get; set; }

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

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Model;
using Model.DataSets;
using ViewModel.Command;
using ViewModel.Models;
using ViewModel.Services;
using Reports.Windows;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la vista de informes (InformesView).
    /// Maneja la lista de informes disponibles y coordina la generación de cada tipo de informe
    /// </summary>
    public class InformeViewModel : INotifyPropertyChanged
    {
        private readonly InformeService _informeService;
        private readonly ActividadService _actividadService;
        private Informe _informeSeleccionado;
        private Actividad _actividadSeleccionada;

        /// <summary>
        /// Colección observable de informes disponibles para el ListBox
        /// </summary>
        public ObservableCollection<Informe> InformesDisponibles { get; set; }
        
        /// <summary>
        /// Colección observable de actividades para el ComboBox de filtro
        /// </summary>
        public ObservableCollection<Actividad> Actividades { get; set; }

        /// <summary>
        /// Informe seleccionado de la lista
        /// </summary>
        public Informe InformeSeleccionado
        {
            get => _informeSeleccionado;
            set
            {
                _informeSeleccionado = value;
                OnPropertyChanged(nameof(InformeSeleccionado));
                OnPropertyChanged(nameof(TieneInformeSeleccionado));
                OnPropertyChanged(nameof(MostrarFiltroActividad));
            }
        }

        /// <summary>
        /// Actividad seleccionada para filtrar el informe de Reservas por Actividad
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
        /// Indica si hay un informe seleccionado para habilitar el botón de generación
        /// </summary>
        public bool TieneInformeSeleccionado => InformeSeleccionado != null;
        
        /// <summary>
        /// Indica si se debe mostrar el ComboBox de selección de actividad (solo para el informe Reservas por Actividad)
        /// </summary>
        public bool MostrarFiltroActividad => InformeSeleccionado?.Tipo == TipoInforme.ListadoReservasPorActividad;

        /// <summary>
        /// Comando para generar el informe seleccionado
        /// </summary>
        public ICommand GenerarInformeCommand { get; }

        /// <summary>
        /// Constructor que inicializa los servicios, comandos y carga los datos
        /// </summary>
        public InformeViewModel()
        {
            _informeService = new InformeService();
            _actividadService = new ActividadService();

            InformesDisponibles = new ObservableCollection<Informe>();
            Actividades = new ObservableCollection<Actividad>();

            // Inicializar Command
            GenerarInformeCommand = new RelayCommand(GenerarInforme);

            // Cargar datos
            InicializarAsync();
        }

        /// <summary>
        /// Inicializa la carga de datos de forma asíncrona
        /// </summary>
        private async void InicializarAsync()
        {
            CargarInformesDisponibles();
            await CargarActividadesAsync();
        }

        /// <summary>
        /// Carga la lista de informes disponibles en el sistema
        /// </summary>
        private void CargarInformesDisponibles()
        {
            var informes = ListaInformes.ObtenerInformesDisponibles();
            
            foreach (var informe in informes)
            {
                InformesDisponibles.Add(informe);
            }
        }

        /// <summary>
        /// Carga las actividades disponibles para el ComboBox de filtro
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        private async Task CargarActividadesAsync()
        {
            try
            {
                var actividades = await _actividadService.ObtenerActividadesAsync();

                Actividades.Clear();
                foreach (var actividad in actividades)
                {
                    Actividades.Add(actividad);
                }

                // Seleccionar la primera actividad por defecto
                if (Actividades.Count > 0)
                {
                    ActividadSeleccionada = Actividades[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error al cargar las actividades: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Genera y muestra el informe seleccionado según su tipo
        /// </summary>
        private async void GenerarInforme()
        {
            // Validar que haya un informe seleccionado
            if (InformeSeleccionado != null)
            {
                // Si es el informe de reservas por actividad, validar que haya actividad seleccionada
                if (InformeSeleccionado.Tipo == TipoInforme.ListadoReservasPorActividad && ActividadSeleccionada == null)
                {
                    MessageBox.Show(
                        "Debes seleccionar una actividad para generar este informe.",
                        "Actividad no seleccionada",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    try
                    {
                        switch (InformeSeleccionado.Tipo)
                        {
                            case TipoInforme.ListadoSocios:
                                await GenerarListadoSocios();
                                break;

                            case TipoInforme.ListadoReservasPorActividad:
                                await GenerarListadoReservasPorActividad();
                                break;

                            case TipoInforme.HistorialReservas:
                                await GenerarHistorialReservas();
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Mostrar error directamente
                        MessageBox.Show($"Error al generar el informe: {ex.Message}",
                                      "Error",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Error);
                    }
                }
            }
        }

        /// <summary>
        /// Genera el informe de listado de socios y abre la ventana del visor
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        private async Task GenerarListadoSocios()
        {
            var dataSet = await _informeService.GenerarDataSetSociosAsync();
            
            // Abrir ventana con el informe
            var ventana = new ListadoSociosWindow(dataSet);
            ventana.ShowDialog();
        }

        /// <summary>
        /// Genera el informe de reservas por actividad y abre la ventana del visor
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        private async Task GenerarListadoReservasPorActividad()
        {
            var dataSet = await _informeService.GenerarDataSetReservasPorActividadAsync(ActividadSeleccionada.IdActividad);
            
            // Abrir ventana con el informe, pasando el nombre de la actividad como parámetro
            var ventana = new Reports.Windows.ListadoReservasPorActividadWindow(dataSet, ActividadSeleccionada.Nombre);
            ventana.ShowDialog();
        }

        /// <summary>
        /// Genera el informe de historial de reservas por socio y abre la ventana del visor
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        private async Task GenerarHistorialReservas()
        {
            var dataSet = await _informeService.GenerarDataSetHistorialReservasAsync();
            
            // Abrir ventana con el informe
            var ventana = new HistorialReservasWindow(dataSet);
            ventana.ShowDialog();
        }

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

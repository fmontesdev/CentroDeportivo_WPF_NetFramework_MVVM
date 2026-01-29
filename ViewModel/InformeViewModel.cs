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
    // ViewModel para la vista de informes (InformesView)
    // Maneja la lista de informes disponibles y la generación de cada uno
    public class InformeViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private readonly InformeService _informeService;
        private readonly ActividadService _actividadService;
        private Informe _informeSeleccionado;
        private Actividad _actividadSeleccionada;

        // Colecciones observables para la UI
        public ObservableCollection<Informe> InformesDisponibles { get; set; }
        public ObservableCollection<Actividad> Actividades { get; set; }

        // Informe seleccionado de la lista
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

        // Actividad seleccionada (para Informe 2: Reservas por Actividad)
        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));
            }
        }

        // Propiedades calculadas para visibilidad en la UI
        public bool TieneInformeSeleccionado => InformeSeleccionado != null;
        public bool MostrarFiltroActividad => InformeSeleccionado?.Tipo == TipoInforme.ListadoReservasPorActividad;

        // Command para generar el informe seleccionado
        public ICommand GenerarInformeCommand { get; }

        // Constructor
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

        // Inicializa la carga de datos de forma asíncrona
        private async void InicializarAsync()
        {
            CargarInformesDisponibles();
            await CargarActividadesAsync();
        }

        // Carga la lista de informes disponibles en el sistema
        private void CargarInformesDisponibles()
        {
            var informes = ListaInformes.ObtenerInformesDisponibles();
            
            foreach (var informe in informes)
            {
                InformesDisponibles.Add(informe);
            }
        }

        // Carga las actividades para el ComboBox de filtro
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

        // Genera el informe seleccionado
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

        // Genera el informe listado de socios
        private async Task GenerarListadoSocios()
        {
            var dataSet = await _informeService.GenerarDataSetSociosAsync();
            
            // Abrir ventana con el informe
            var ventana = new ListadoSociosWindow(dataSet);
            ventana.ShowDialog();
        }

        // Genera el informe reservas por actividad
        private async Task GenerarListadoReservasPorActividad()
        {
            var dataSet = await _informeService.GenerarDataSetReservasPorActividadAsync(ActividadSeleccionada.IdActividad);
            
            // Abrir ventana con el informe, pasando el nombre de la actividad como parámetro
            var ventana = new Reports.Windows.ListadoReservasPorActividadWindow(dataSet, ActividadSeleccionada.Nombre);
            ventana.ShowDialog();
        }

        // Genera el informe historial de reservas por socio
        private async Task GenerarHistorialReservas()
        {
            var dataSet = await _informeService.GenerarDataSetHistorialReservasAsync();
            
            // Abrir ventana con el informe
            var ventana = new HistorialReservasWindow(dataSet);
            ventana.ShowDialog();
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System;
using System.ComponentModel;
using System.Windows.Input;
using ViewModel.Command;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la ventana principal (MainWindow).
    /// Maneja la navegación entre vistas y coordina la interfaz principal de la aplicación
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        private string _titulo;

        /// <summary>
        /// Título de la ventana principal que cambia según la vista activa
        /// </summary>
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged(nameof(Titulo));
            }
        }

        /// <summary>
        /// Comando para mostrar la vista de reservas
        /// </summary>
        public ICommand MostrarReservasCommand { get; }
        
        /// <summary>
        /// Comando para mostrar la vista de socios
        /// </summary>
        public ICommand MostrarSociosCommand { get; }
        
        /// <summary>
        /// Comando para mostrar la vista de actividades
        /// </summary>
        public ICommand MostrarActividadesCommand { get; }
        
        /// <summary>
        /// Comando para mostrar la vista de informes
        /// </summary>
        public ICommand MostrarInformesCommand { get; }
        
        /// <summary>
        /// Comando para salir de la aplicación
        /// </summary>
        public ICommand SalirCommand { get; }

        /// <summary>
        /// Constructor que inicializa todos los comandos de navegación
        /// </summary>
        public MainViewModel()
        {
            // Inicializar Commands
            MostrarReservasCommand = new RelayCommand(MostrarReservas);
            MostrarSociosCommand = new RelayCommand(MostrarSocios);
            MostrarActividadesCommand = new RelayCommand(MostrarActividades);
            MostrarInformesCommand = new RelayCommand(MostrarInformes);
            SalirCommand = new RelayCommand(Salir);
        }

        /// <summary>
        /// Inicializa la vista por defecto mostrando las reservas
        /// </summary>
        public void InicializarVistaInicial()
        {
            MostrarReservas();
        }

        /// <summary>
        /// Cambia a la vista de reservas
        /// </summary>
        private void MostrarReservas()
        {
            CambiarVista?.Invoke("Reservas");
            Titulo = "Gestión de Reservas";
        }

        /// <summary>
        /// Cambia a la vista de socios
        /// </summary>
        private void MostrarSocios()
        {
            CambiarVista?.Invoke("Socios");
            Titulo = "Gestión de Socios";
        }

        /// <summary>
        /// Cambia a la vista de actividades
        /// </summary>
        private void MostrarActividades()
        {
            CambiarVista?.Invoke("Actividades");
            Titulo = "Gestión de Actividades";
        }

        /// <summary>
        /// Cambia a la vista de informes
        /// </summary>
        private void MostrarInformes()
        {
            CambiarVista?.Invoke("Informes");
            Titulo = "Informes";
        }

        /// <summary>
        /// Solicita cerrar la aplicación
        /// </summary>
        private void Salir()
        {
            SolicitarCerrar?.Invoke();
        }

        /// <summary>
        /// Acción para comunicar con la Vista y cambiar de vista activa
        /// </summary>
        public Action<string> CambiarVista { get; set; }
        
        /// <summary>
        /// Acción para comunicar con la Vista y solicitar el cierre de la aplicación
        /// </summary>
        public Action SolicitarCerrar { get; set; }

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

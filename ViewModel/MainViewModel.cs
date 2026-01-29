using System;
using System.ComponentModel;
using System.Windows.Input;
using ViewModel.Command;

namespace ViewModel
{
    // ViewModel para la ventana principal (MainWindow)
    // Maneja la navegación entre vistas
    public class MainViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private string _titulo;

        // Título de la ventana
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged(nameof(Titulo));
            }
        }

        // Commands para navegación
        public ICommand MostrarReservasCommand { get; }
        public ICommand MostrarSociosCommand { get; }
        public ICommand MostrarActividadesCommand { get; }
        public ICommand MostrarInformesCommand { get; }
        public ICommand SalirCommand { get; }

        // Constructor
        public MainViewModel()
        {
            // Inicializar Commands
            MostrarReservasCommand = new RelayCommand(MostrarReservas);
            MostrarSociosCommand = new RelayCommand(MostrarSocios);
            MostrarActividadesCommand = new RelayCommand(MostrarActividades);
            MostrarInformesCommand = new RelayCommand(MostrarInformes);
            SalirCommand = new RelayCommand(Salir);
        }

        // Inicializa la vista por defecto (llamado desde MainWindow)
        public void InicializarVistaInicial()
        {
            MostrarReservas();
        }

        // Métodos de navegación
        private void MostrarReservas()
        {
            CambiarVista?.Invoke("Reservas");
            Titulo = "Gestión de Reservas";
        }

        private void MostrarSocios()
        {
            CambiarVista?.Invoke("Socios");
            Titulo = "Gestión de Socios";
        }

        private void MostrarActividades()
        {
            CambiarVista?.Invoke("Actividades");
            Titulo = "Gestión de Actividades";
        }

        private void MostrarInformes()
        {
            CambiarVista?.Invoke("Informes");
            Titulo = "Informes";
        }

        private void Salir()
        {
            SolicitarCerrar?.Invoke();
        }

        // Actions para comunicar con la Vista (más simple que eventos)
        public Action<string> CambiarVista { get; set; }
        public Action SolicitarCerrar { get; set; }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

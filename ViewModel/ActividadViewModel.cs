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
    // ViewModel para la vista principal de gestión de actividades (ActividadesView)
    // Maneja la lista de actividades, selección y operaciones CRUD
    public class ActividadViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private readonly ActividadService _actividadService;
        private Actividad _actividadSeleccionada;
        private string _nombre;
        private string _aforoMaximo;
        private string _errorMessage;

        // Colección observable de actividades para el DataGrid
        // Actualiza la UI automáticamente
        public ObservableCollection<Actividad> Actividades { get; set; }

        // Actividad seleccionada en el DataGrid
        // Al seleccionar una actividad, se cargan sus datos en los TextBox
        public Actividad ActividadSeleccionada
        {
            get => _actividadSeleccionada;
            set
            {
                _actividadSeleccionada = value;
                OnPropertyChanged(nameof(ActividadSeleccionada));
                
                // Cargar datos en los TextBox cuando se selecciona una actividad
                if (_actividadSeleccionada != null)
                {
                    Nombre = _actividadSeleccionada.Nombre;
                    AforoMaximo = _actividadSeleccionada.AforoMaximo.ToString();
                }
                else
                {
                    LimpiarFormulario();
                }
            }
        }

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

        // Total de actividades (para mostrar en la UI)
        public int TotalActividades => Actividades?.Count ?? 0;

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

        // Command para crear una nueva actividad (abre ventana modal)
        public ICommand NuevoCommand { get; }

        // Command para editar la actividad seleccionada
        public ICommand EditarCommand { get; }

        // Command para eliminar la actividad seleccionada
        public ICommand EliminarCommand { get; }

        // Constructor
        public ActividadViewModel()
        {
            _actividadService = new ActividadService();
            Actividades = new ObservableCollection<Actividad>();

            // Inicializar Commands
            NuevoCommand = new RelayCommand(NuevaActividad);
            EditarCommand = new RelayCommand(EditarActividad);
            EliminarCommand = new RelayCommand(EliminarActividad);

            // Cargar actividades al inicializar
            InicializarAsync();
        }

        // Inicializa la carga de datos de forma asíncrona
        public async void InicializarAsync()
        {
            await CargarActividadesAsync();
            SeleccionarPrimero();
        }

        // Carga la lista de actividades desde la base de datos
        private async Task CargarActividadesAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                var actividades = await _actividadService.ObtenerActividadesAsync();

                Actividades.Clear();
                foreach (var actividad in actividades)
                {
                    Actividades.Add(actividad);
                }

                OnPropertyChanged(nameof(TotalActividades));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar actividades: {ex.Message}";
            }
        }

        // Selecciona la primera actividad de la lista
        private void SeleccionarPrimero()
        {
            if (Actividades.Count > 0)
            {
                ActividadSeleccionada = Actividades[0];
            }
            else
            {
                ActividadSeleccionada = null;
            }
        }

        // Mantiene la selección de la actividad en el índice especificado
        // Si el índice es inválido, selecciona la primera
        private void MantieneSeleccion(int indice)
        {
            if (Actividades.Count == 0)
            {
                ActividadSeleccionada = null;
            }
            else if (indice >= 0 && indice < Actividades.Count)
            {
                ActividadSeleccionada = Actividades[indice];
            }
            else
            {
                SeleccionarPrimero();
            }
        }

        // Selecciona la actividad anterior al índice especificado al eliminar
        // Si se eliminó la primera, selecciona la nueva primera
        // Si no quedan actividades, no selecciona ninguna
        private void SeleccionaAnterior(int indiceEliminado)
        {
            if (Actividades.Count == 0)
            {
                ActividadSeleccionada = null;
            }
            else if (indiceEliminado == 0)
            {
                // Se eliminó la primera, seleccionar la nueva primera
                ActividadSeleccionada = Actividades[0];
            }
            else
            {
                // Seleccionar la anterior, pero asegurarse de no exceder los límites
                int nuevoIndice = indiceEliminado - 1;
                if (nuevoIndice >= Actividades.Count)
                {
                    nuevoIndice = Actividades.Count - 1;
                }
                ActividadSeleccionada = Actividades[nuevoIndice];
            }
        }

        // Abre la ventana modal para crear una nueva actividad
        private void NuevaActividad()
        {
            VentanaNuevaActividad?.Invoke();
        }

        // Edita la actividad seleccionada con los datos de los TextBox
        private async void EditarActividad()
        {
            // Verifica que haya una fila seleccionada
            if (ActividadSeleccionada == null)
            {                 
                ErrorMessage = "No hay ninguna actividad seleccionada para editar";
            }
            else if (ValidarFormulario())
            {
                try
                {
                    ErrorMessage = string.Empty;

                    // Guardar el índice de la actividad seleccionada para mantener la selección
                    int indiceActividadActual = Actividades.IndexOf(ActividadSeleccionada);

                    // Actualizar propiedades de la actividad seleccionada
                    ActividadSeleccionada.Nombre = Nombre.Trim();
                    ActividadSeleccionada.AforoMaximo = int.Parse(AforoMaximo.Trim());

                    // Guardar en base de datos
                    await _actividadService.ActualizarActividadAsync(ActividadSeleccionada);
                    
                    // Recargar y mantener la selección de la actividad editada
                    await CargarActividadesAsync();
                    MantieneSeleccion(indiceActividadActual);
                }
                catch (ArgumentException ex)
                {
                    ErrorMessage = $"{ex.Message}";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al editar actividad: {ex.Message}";
                }
            }
        }

        // Elimina la actividad seleccionada después de confirmación
        private void EliminarActividad()
        {
            if (ActividadSeleccionada == null)
            {
                ErrorMessage = "No hay ninguna actividad seleccionada para eliminar";
                return;
            }
            else
            {
                ErrorMessage = string.Empty;
                ConfirmarEliminar?.Invoke((ActividadSeleccionada.IdActividad, ActividadSeleccionada.Nombre));
            }
        }

        // Confirma y ejecuta la eliminación de la actividad
        public async void ConfirmarEliminarActividad(int idActividad)
        {
            try
            {
                // Usar el objeto ActividadSeleccionada que ya tenemos disponible
                if (ActividadSeleccionada == null || ActividadSeleccionada.IdActividad != idActividad)
                {
                    ErrorMessage = "La actividad seleccionada no coincide con la actividad a eliminar";
                }
                else
                {
                    // Guardar el índice de la actividad que vamos a eliminar
                    int indiceActividadEliminada = Actividades.IndexOf(ActividadSeleccionada);

                    await _actividadService.EliminarActividadAsync(ActividadSeleccionada);

                    // Recargar y seleccionar la actividad anterior
                    await CargarActividadesAsync();
                    SeleccionaAnterior(indiceActividadEliminada);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej: actividad tiene reservas)
                ErrorMessage = $"{ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al eliminar actividad: {ex.Message}";
            }
        }

        // Valida los datos del formulario
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

        // Limpia los campos del formulario
        private void LimpiarFormulario()
        {
            Nombre = string.Empty;
            AforoMaximo = string.Empty;
        }

        // Actions para comunicar con la Vista (más simple que eventos)
        public Action VentanaNuevaActividad { get; set; }
        public Action<(int IdActividad, string Nombre)> ConfirmarEliminar { get; set; }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

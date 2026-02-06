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
    /// ViewModel para la vista principal de gestión de actividades (ActividadesView).
    /// Maneja la lista de actividades, selección y operaciones CRUD
    /// </summary>
    public class ActividadViewModel : INotifyPropertyChanged
    {
        private readonly ActividadService _actividadService;
        private Actividad _actividadSeleccionada;
        private string _nombre;
        private string _aforoMaximo;
        private string _errorMessage;

        /// <summary>
        /// Colección observable de actividades para el DataGrid.
        /// Se actualiza automáticamente en la interfaz de usuario
        /// </summary>
        public ObservableCollection<Actividad> Actividades { get; set; }

        /// <summary>
        /// Actividad seleccionada en el DataGrid.
        /// Al seleccionar una actividad, se cargan sus datos en los TextBox
        /// </summary>
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

        /// <summary>
        /// Nombre de la actividad enlazado al TextBox de edición
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
        /// Aforo máximo de la actividad enlazado al TextBox de edición
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
        /// Total de actividades registradas en la colección
        /// </summary>
        public int TotalActividades => Actividades?.Count ?? 0;

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
        /// Comando para crear una nueva actividad (abre ventana modal)
        /// </summary>
        public ICommand NuevoCommand { get; }

        /// <summary>
        /// Comando para editar la actividad seleccionada
        /// </summary>
        public ICommand EditarCommand { get; }

        /// <summary>
        /// Comando para eliminar la actividad seleccionada
        /// </summary>
        public ICommand EliminarCommand { get; }

        /// <summary>
        /// Constructor que inicializa el servicio, comandos y carga los datos iniciales
        /// </summary>
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

        /// <summary>
        /// Inicializa la carga de datos de forma asíncrona y selecciona la primera actividad
        /// </summary>
        public async void InicializarAsync()
        {
            await CargarActividadesAsync();
            SeleccionarPrimero();
        }

        /// <summary>
        /// Carga la lista de actividades desde la base de datos y actualiza la colección observable
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
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

        /// <summary>
        /// Selecciona la primera actividad de la lista
        /// </summary>
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

        /// <summary>
        /// Mantiene la selección de la actividad en el índice especificado.
        /// Si el índice es inválido, selecciona la primera
        /// </summary>
        /// <param name="indice">Índice de la actividad a seleccionar</param>
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

        /// <summary>
        /// Selecciona la actividad anterior al índice especificado al eliminar.
        /// Si se eliminó la primera, selecciona la nueva primera.
        /// Si no quedan actividades, no selecciona ninguna
        /// </summary>
        /// <param name="indiceEliminado">Índice de la actividad que fue eliminada</param>
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

        /// <summary>
        /// Abre la ventana modal para crear una nueva actividad
        /// </summary>
        private void NuevaActividad()
        {
            VentanaNuevaActividad?.Invoke();
        }

        /// <summary>
        /// Edita la actividad seleccionada con los datos de los TextBox,
        /// validando los datos y guardando los cambios en la base de datos
        /// </summary>
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

        /// <summary>
        /// Solicita confirmación para eliminar la actividad seleccionada
        /// </summary>
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

        /// <summary>
        /// Confirma y ejecuta la eliminación de la actividad de la base de datos
        /// </summary>
        /// <param name="idActividad">Identificador de la actividad a eliminar</param>
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

        /// <summary>
        /// Valida los datos del formulario de edición de actividades
        /// </summary>
        /// <returns>True si los datos son válidos, false en caso contrario</returns>
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
        /// Limpia los campos del formulario de edición
        /// </summary>
        private void LimpiarFormulario()
        {
            Nombre = string.Empty;
            AforoMaximo = string.Empty;
        }

        /// <summary>
        /// Acción para comunicar con la Vista y abrir la ventana de nueva actividad
        /// </summary>
        public Action VentanaNuevaActividad { get; set; }
        
        /// <summary>
        /// Acción para comunicar con la Vista y solicitar confirmación de eliminación
        /// </summary>
        public Action<(int IdActividad, string Nombre)> ConfirmarEliminar { get; set; }

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

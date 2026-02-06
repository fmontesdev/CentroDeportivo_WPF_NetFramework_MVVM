using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Model;
using ViewModel.Command;
using ViewModel.Services;

namespace ViewModel
{
    /// <summary>
    /// ViewModel para la vista principal de gestión de socios (SociosView).
    /// Maneja la lista de socios, selección y operaciones CRUD
    /// </summary>
    public class SocioViewModel : INotifyPropertyChanged
    {
        private readonly SocioService _socioService;
        private Socio _socioSeleccionado;
        private string _nombre;
        private string _email;
        private bool _activo;
        private string _errorMessage;

        /// <summary>
        /// Colección observable de socios para el DataGrid.
        /// Se actualiza automáticamente en la interfaz de usuario
        /// </summary>
        public ObservableCollection<Socio> Socios { get; set; }

        /// <summary>
        /// Socio seleccionado en el DataGrid.
        /// Al seleccionar un socio, se cargan sus datos en los TextBox
        /// </summary>
        public Socio SocioSeleccionado
        {
            get => _socioSeleccionado;
            set
            {
                _socioSeleccionado = value;
                OnPropertyChanged(nameof(SocioSeleccionado));
                
                // Cargar datos en los TextBox cuando se selecciona un socio
                if (_socioSeleccionado != null)
                {
                    Nombre = _socioSeleccionado.Nombre;
                    Email = _socioSeleccionado.Email;
                    Activo = _socioSeleccionado.Activo;
                }
                else
                {
                    LimpiarFormulario();
                }
            }
        }

        /// <summary>
        /// Nombre del socio enlazado al TextBox de edición
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
        /// Email del socio enlazado al TextBox de edición
        /// </summary>
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        /// <summary>
        /// Estado activo del socio enlazado al CheckBox de edición
        /// </summary>
        public bool Activo
        {
            get => _activo;
            set
            {
                _activo = value;
                OnPropertyChanged(nameof(Activo));
            }
        }

        /// <summary>
        /// Total de socios registrados en la colección
        /// </summary>
        public int TotalSocios => Socios?.Count ?? 0;

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
        /// Comando para crear un nuevo socio (abre ventana modal)
        /// </summary>
        public ICommand NuevoCommand { get; }

        /// <summary>
        /// Comando para editar el socio seleccionado
        /// </summary>
        public ICommand EditarCommand { get; }

        /// <summary>
        /// Comando para eliminar el socio seleccionado
        /// </summary>
        public ICommand EliminarCommand { get; }

        /// <summary>
        /// Constructor que inicializa el servicio, comandos y carga los datos iniciales
        /// </summary>
        public SocioViewModel()
        {
            _socioService = new SocioService();
            Socios = new ObservableCollection<Socio>();

            // Inicializar Commands
            NuevoCommand = new RelayCommand(NuevoSocio);
            EditarCommand = new RelayCommand(EditarSocio);
            EliminarCommand = new RelayCommand(EliminarSocio);

            // Cargar socios al inicializar
            InicializarAsync();
        }

        /// <summary>
        /// Inicializa la carga de datos de forma asíncrona y selecciona el primer socio
        /// </summary>
        public async void InicializarAsync()
        {
            await CargarSociosAsync();
            SeleccionarPrimero();
        }

        /// <summary>
        /// Carga la lista de socios desde la base de datos y actualiza la colección observable
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        private async Task CargarSociosAsync()
        {
            try
            {
                ErrorMessage = string.Empty;

                var socios = await _socioService.ObtenerSociosAsync();

                Socios.Clear();
                foreach (var socio in socios)
                {
                    Socios.Add(socio);
                }

                OnPropertyChanged(nameof(TotalSocios));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al cargar socios: {ex.Message}";
            }
        }

        /// <summary>
        /// Selecciona el primer socio de la lista
        /// </summary>
        private void SeleccionarPrimero()
        {
            if (Socios.Count > 0)
            {
                SocioSeleccionado = Socios[0];
            }
            else
            {
                SocioSeleccionado = null;
            }
        }

        /// <summary>
        /// Mantiene la selección del socio en el índice especificado.
        /// Si el índice es inválido, selecciona el primero
        /// </summary>
        /// <param name="indice">Índice del socio a seleccionar</param>
        private void MantieneSeleccion(int indice)
        {
            if (Socios.Count == 0)
            {
                SocioSeleccionado = null;
            }
            else if (indice >= 0 && indice < Socios.Count)
            {
                SocioSeleccionado = Socios[indice];
            }
            else
            {
                SeleccionarPrimero();
            }
        }

        /// <summary>
        /// Selecciona el socio anterior al índice especificado al eliminar.
        /// Si se eliminó el primero, selecciona el nuevo primero.
        /// Si no quedan socios, no selecciona ninguno
        /// </summary>
        /// <param name="indiceEliminado">Índice del socio que fue eliminado</param>
        private void SeleccionaAnterior(int indiceEliminado)
        {
            if (Socios.Count == 0)
            {
                SocioSeleccionado = null;
            }
            else if (indiceEliminado == 0)
            {
                // Se eliminó el primero, seleccionar el nuevo primero
                SocioSeleccionado = Socios[0];
            }
            else
            {
                // Seleccionar el anterior, pero asegurarse de no exceder los límites
                int nuevoIndice = indiceEliminado - 1;
                if (nuevoIndice >= Socios.Count)
                {
                    nuevoIndice = Socios.Count - 1;
                }
                SocioSeleccionado = Socios[nuevoIndice];
            }
        }

        /// <summary>
        /// Abre la ventana modal para crear un nuevo socio
        /// </summary>
        private void NuevoSocio()
        {
            VentanaNuevoSocio?.Invoke();
        }

        /// <summary>
        /// Edita el socio seleccionado con los datos de los TextBox,
        /// validando los datos y guardando los cambios en la base de datos
        /// </summary>
        private async void EditarSocio()
        {
            // Verifica que haya una fila seleccionada
            if (SocioSeleccionado == null)
            {                 
                ErrorMessage = "No hay ningún socio seleccionado para editar";
            }
            else if (ValidarFormulario())
            {
                try
                {
                    ErrorMessage = string.Empty;

                    // Guardar el índice del socio seleccionado para mantener la selección
                    int indiceSocioActual = Socios.IndexOf(SocioSeleccionado);

                    // Actualizar propiedades del socio seleccionado
                    SocioSeleccionado.Nombre = Nombre.Trim();
                    SocioSeleccionado.Email = Email.Trim();
                    SocioSeleccionado.Activo = Activo;

                    // Guardar en base de datos
                    await _socioService.ActualizarSocioAsync(SocioSeleccionado);
                    
                    // Recargar y mantener la selección del socio editado
                    await CargarSociosAsync();
                    MantieneSeleccion(indiceSocioActual);
                }
                catch (ArgumentException ex)
                {
                    ErrorMessage = $"{ex.Message}";
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"Error al editar socio: {ex.Message}";
                }
            }
        }

        /// <summary>
        /// Solicita confirmación para eliminar el socio seleccionado
        /// </summary>
        private void EliminarSocio()
        {
            if (SocioSeleccionado == null)
            {
                ErrorMessage = "No hay ningún socio seleccionado para eliminar";
                return;
            }
            else
            {
                ErrorMessage = string.Empty;
                ConfirmarEliminar?.Invoke((SocioSeleccionado.IdSocio, SocioSeleccionado.Nombre));
            }
        }

        /// <summary>
        /// Confirma y ejecuta la eliminación del socio de la base de datos
        /// </summary>
        /// <param name="idSocio">Identificador del socio a eliminar</param>
        public async void ConfirmarEliminarSocio(int idSocio)
        {
            try
            {
                // Usar el objeto SocioSeleccionado que ya tenemos disponible
                if (SocioSeleccionado == null || SocioSeleccionado.IdSocio != idSocio)
                {
                    ErrorMessage = "El socio seleccionado no coincide con el socio a eliminar";
                }
                else
                {
                    // Guardar el índice del socio que vamos a eliminar
                    int indiceSocioEliminado = Socios.IndexOf(SocioSeleccionado);

                    await _socioService.EliminarSocioAsync(SocioSeleccionado);

                    // Recargar y seleccionar el socio anterior
                    await CargarSociosAsync();
                    SeleccionaAnterior(indiceSocioEliminado);
                }
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej: socio tiene reservas)
                ErrorMessage = $"{ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error al eliminar socio: {ex.Message}";
            }
        }

        /// <summary>
        /// Valida los datos del formulario de edición de socios
        /// </summary>
        /// <returns>True si los datos son válidos, false en caso contrario</returns>
        private bool ValidarFormulario()
        {
            // Patrón de email
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            // Validación del Nombre
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                ErrorMessage = "El nombre del socio es obligatorio";
                return false;
            }

            if (int.TryParse(Nombre, out _))
            {
                ErrorMessage = "El nombre del socio no puede ser solo números";
                return false;
            }

            if (Nombre.Trim().Length < 2)
            {
                ErrorMessage = "El nombre debe tener al menos 2 caracteres";
                return false;
            }

            // Validación del Email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "El email del socio es obligatorio";
                return false;
            }

            if (!Regex.IsMatch(Email, patron))
            {
                ErrorMessage = "El email debe tener un formato válido (ejemplo@email.com)";
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
            Email = string.Empty;
            Activo = false;
        }

        /// <summary>
        /// Acción para comunicar con la Vista y abrir la ventana de nuevo socio
        /// </summary>
        public Action VentanaNuevoSocio { get; set; }
        
        /// <summary>
        /// Acción para comunicar con la Vista y solicitar confirmación de eliminación
        /// </summary>
        public Action<(int IdSocio, string Nombre)> ConfirmarEliminar { get; set; }

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


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
    // ViewModel para la vista principal de gestión de socios (SociosView)
    // Maneja la lista de socios, selección y operaciones CRUD
    public class SocioViewModel : INotifyPropertyChanged
    {
        // Propiedades privadas
        private readonly SocioService _socioService;
        private Socio _socioSeleccionado;
        private string _nombre;
        private string _email;
        private string _errorMessage;

        // Colección observable de socios para el DataGrid
        // Actualiza la UI automáticamente
        public ObservableCollection<Socio> Socios { get; set; }

        // Socio seleccionado en el DataGrid
        // Al seleccionar un socio, se cargan sus datos en los TextBox
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
                }
                else
                {
                    LimpiarFormulario();
                }
            }
        }

        // Nombre del socio (enlazado al TextBox)
        public string Nombre
        {
            get => _nombre;
            set
            {
                _nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        // Email del socio (enlazado al TextBox)
        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        // Total de socios (para mostrar en la UI)
        public int TotalSocios => Socios?.Count ?? 0;

        // Mensaje de error general
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

        // Command para crear un nuevo socio (abre ventana modal)
        public ICommand NuevoCommand { get; }

        // Command para editar el socio seleccionado
        public ICommand EditarCommand { get; }

        // Command para eliminar el socio seleccionado
        public ICommand EliminarCommand { get; }

        // Constructor
        public SocioViewModel()
        {
            _socioService = new SocioService();
            Socios = new ObservableCollection<Socio>();

            // Inicializar Commands
            NuevoCommand = new RelayCommand(NuevoSocio);
            EditarCommand = new RelayCommand(EditarSocio, PuedeEditar);
            EliminarCommand = new RelayCommand(EliminarSocio, PuedeEliminar);

            // Cargar socios al inicializar
            CargarSocios();
        }

        // Abre la ventana modal para crear un nuevo socio
        private void NuevoSocio()
        {
            ErrorMessage = string.Empty;
            LimpiarFormulario();
            
            // Disparar evento para que la Vista abra la ventana modal
            VentanaNuevoSocio?.Invoke(this, EventArgs.Empty);
        }

        // Edita el socio seleccionado con los datos de los TextBox
        private async void EditarSocio()
        {
            if (SocioSeleccionado == null) return;

            try
            {
                ErrorMessage = string.Empty;

                // Validar formulario
                if (!ValidarFormulario())
                {
                    return;
                }

                // Actualizar propiedades del socio seleccionado
                SocioSeleccionado.Nombre = Nombre.Trim();
                SocioSeleccionado.Email = Email.Trim();

                // Guardar en base de datos
                await _socioService.ActualizarSocioAsync(SocioSeleccionado);

                // Refrescar el DataGrid
                OnPropertyChanged(nameof(Socios));
                
                ErrorMessage = string.Empty;
            }
            catch (ArgumentException ex)
            {
                ErrorMessage = $"⚠️ {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"❌ Error al editar socio: {ex.Message}";
            }
        }

        // Elimina el socio seleccionado después de confirmación
        private void EliminarSocio()
        {
            if (SocioSeleccionado == null) return;

            try
            {
                ErrorMessage = string.Empty;

                // Disparar evento para que la Vista muestre confirmación
                ConfirmarEliminar?.Invoke(this, (SocioSeleccionado.IdSocio, SocioSeleccionado.Nombre));
            }
            catch (Exception ex)
            {
                ErrorMessage = $"❌ Error: {ex.Message}";
            }
        }

        /// <summary>
        /// Determina si se puede editar (debe haber un socio seleccionado y datos válidos)
        /// </summary>
        private bool PuedeEditar()
        {
            return SocioSeleccionado != null && 
                   !string.IsNullOrWhiteSpace(Nombre) && 
                   !string.IsNullOrWhiteSpace(Email);
        }

        /// <summary>
        /// Determina si se puede eliminar (debe haber un socio seleccionado)
        /// </summary>
        private bool PuedeEliminar()
        {
            return SocioSeleccionado != null;
        }

        // Carga la lista de socios desde la base de datos
        private async void CargarSocios()
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
                ErrorMessage = $"❌ Error al cargar socios: {ex.Message}";
            }
        }

        // Valida los datos del formulario
        private bool ValidarFormulario()
        {
            // Patrón de email
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            // Validación del Nombre
            if (string.IsNullOrWhiteSpace(Nombre))
            {
                ErrorMessage = "⚠️ El nombre del socio es obligatorio";
                return false;
            }

            if (int.TryParse(Nombre, out _))
            {
                ErrorMessage = "⚠️ El nombre del socio no puede ser solo números";
                return false;
            }

            if (Nombre.Trim().Length < 2)
            {
                ErrorMessage = "⚠️ El nombre debe tener al menos 2 caracteres";
                return false;
            }

            // Validación del Email
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "⚠️ El email del socio es obligatorio";
                return false;
            }

            if (!Regex.IsMatch(Email, patron))
            {
                ErrorMessage = "⚠️ El email debe tener un formato válido (ejemplo@email.com)";
                return false;
            }

            return true;
        }

        // Limpia los campos del formulario
        private void LimpiarFormulario()
        {
            Nombre = string.Empty;
            Email = string.Empty;
        }

        // Confirma y ejecuta la eliminación del socio
        // La Vista debe llamar a este método después de que el usuario confirme
        public async void ConfirmarEliminarSocio(int idSocio)
        {
            try
            {
                // Buscar el socio en la colección local
                var socio = Socios.FirstOrDefault(s => s.IdSocio == idSocio);
                if (socio == null)
                {
                    ErrorMessage = "No se encontró el socio a eliminar";
                    return;
                }

                await _socioService.EliminarSocioAsync(socio);

                // Eliminar de la colección local
                Socios.Remove(socio);
                SocioSeleccionado = null;
                LimpiarFormulario();

                OnPropertyChanged(nameof(TotalSocios));

                ErrorMessage = string.Empty;
            }
            catch (InvalidOperationException ex)
            {
                // Error de negocio (ej: socio tiene reservas)
                ErrorMessage = $"⚠️ {ex.Message}";
            }
            catch (Exception ex)
            {
                ErrorMessage = $"❌ Error al eliminar socio: {ex.Message}";
            }
        }

        // Recarga la lista después de crear un socio desde la ventana modal
        public void ActualizarListaDespuesDeCrear()
        {
            CargarSocios();
            LimpiarFormulario();
            SocioSeleccionado = null;
        }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Evento para solicitar abrir la ventana de nuevo socio
        public event EventHandler VentanaNuevoSocio;

        // Evento para solicitar confirmación antes de eliminar
        public event EventHandler<(int IdSocio, string Nombre)> ConfirmarEliminar;
    }
}


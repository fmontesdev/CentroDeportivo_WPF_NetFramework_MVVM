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
            EditarCommand = new RelayCommand(EditarSocio);
            EliminarCommand = new RelayCommand(EliminarSocio);

            // Cargar socios al inicializar
            CargarSocios();
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
                ErrorMessage = $"Error al cargar socios: {ex.Message}";
            }
        }

        // Abre la ventana modal para crear un nuevo socio
        private void NuevoSocio()
        {
            VentanaNuevoSocio?.Invoke();
        }

        // Edita el socio seleccionado con los datos de los TextBox
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

                    // Actualizar propiedades del socio seleccionado
                    SocioSeleccionado.Nombre = Nombre.Trim();
                    SocioSeleccionado.Email = Email.Trim();

                    // Guardar en base de datos
                    await _socioService.ActualizarSocioAsync(SocioSeleccionado);
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

        // Elimina el socio seleccionado después de confirmación
        private void EliminarSocio()
        {
            if (SocioSeleccionado == null)
            {
                ErrorMessage = "No hay ningún socio seleccionado para eliminar";
                return;
            }

            ErrorMessage = string.Empty;
            ConfirmarEliminar?.Invoke((SocioSeleccionado.IdSocio, SocioSeleccionado.Nombre));
        }

        // Confirma y ejecuta la eliminación del socio
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

        // Valida los datos del formulario
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

        // Limpia los campos del formulario
        private void LimpiarFormulario()
        {
            Nombre = string.Empty;
            Email = string.Empty;
        }

        // Recarga la lista después de crear un socio desde la ventana modal
        public void ActualizarListaDespuesDeCrear()
        {
            CargarSocios();
            LimpiarFormulario();
            SocioSeleccionado = null;
        }

        // Actions para comunicar con la Vista (más simple que eventos)
        public Action VentanaNuevoSocio { get; set; }
        public Action<(int IdSocio, string Nombre)> ConfirmarEliminar { get; set; }

        // INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


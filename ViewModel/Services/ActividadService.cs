using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Repositorios;

namespace ViewModel.Services
{
    // Servicio que encapsula la lógica de negocio para la gestión de actividades
    // Actúa como intermediario entre el ViewModel y el Repositorio
    public class ActividadService
    {
        private readonly ActividadRepositorio _repo;

        public ActividadService()
        {
            _repo = new ActividadRepositorio();
        }

        // Obtiene todas las actividades de la base de datos
        public async Task<List<Actividad>> ObtenerActividadesAsync()
        {
            return await _repo.SeleccionarAsync();
        }

        // Crea una nueva actividad en la base de datos
        public async Task CrearActividadAsync(Actividad actividad)
        {
            // Validaciones de negocio
            ValidarActividad(actividad);

            await _repo.CrearAsync(actividad);
        }

        // Actualiza una actividad existente
        public async Task ActualizarActividadAsync(Actividad actividad)
        {
            // Validaciones de negocio
            ValidarActividad(actividad);

            await _repo.GuardarAsync();
        }

        // Elimina una actividad de la base de datos
        public async Task EliminarActividadAsync(Actividad actividad)
        {
            if (actividad == null)
                throw new ArgumentNullException(nameof(actividad), "La actividad no puede ser nula");

            // Verificar si tiene reservas activas
            if (actividad.Reserva != null && actividad.Reserva.Any())
            {
                throw new InvalidOperationException(
                    "No se puede eliminar una actividad con reservas asociadas. " +
                    "Primero elimine las reservas.");
            }

            await _repo.EliminarAsync(actividad);
        }

        // Valida los datos de una actividad según las reglas de negocio
        private void ValidarActividad(Actividad actividad)
        {
            if (actividad == null)
                throw new ArgumentNullException(nameof(actividad), "La actividad no puede ser nula");

            if (string.IsNullOrWhiteSpace(actividad.Nombre))
                throw new ArgumentException("El nombre de la actividad no puede estar vacío", nameof(actividad.Nombre));

            if (actividad.AforoMaximo <= 0)
                throw new ArgumentException("El aforo máximo debe ser mayor a 0", nameof(actividad.AforoMaximo));

            if (actividad.AforoMaximo > 1000)
                throw new ArgumentException("El aforo máximo no puede superar las 1000 personas", nameof(actividad.AforoMaximo));
        }
    }
}

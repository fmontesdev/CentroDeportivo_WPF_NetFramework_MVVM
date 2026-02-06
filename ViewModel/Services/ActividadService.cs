using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Repositorios;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio que encapsula la lógica de negocio para la gestión de actividades.
    /// Actúa como intermediario entre el ViewModel y el Repositorio
    /// </summary>
    public class ActividadService
    {
        private readonly ActividadRepositorio _repo;

        /// <summary>
        /// Constructor que inicializa el repositorio de actividades
        /// </summary>
        public ActividadService()
        {
            _repo = new ActividadRepositorio();
        }

        /// <summary>
        /// Obtiene todas las actividades de la base de datos
        /// </summary>
        /// <returns>Lista de todas las actividades registradas</returns>
        public async Task<List<Actividad>> ObtenerActividadesAsync()
        {
            return await _repo.SeleccionarAsync();
        }

        /// <summary>
        /// Crea una nueva actividad en la base de datos después de validar las reglas de negocio
        /// </summary>
        /// <param name="actividad">Actividad a crear</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si la actividad es nula</exception>
        /// <exception cref="ArgumentException">Si los datos de la actividad no son válidos</exception>
        public async Task CrearActividadAsync(Actividad actividad)
        {
            // Validaciones de negocio
            ValidarActividad(actividad);

            await _repo.CrearAsync(actividad);
        }

        /// <summary>
        /// Actualiza los datos de una actividad existente después de validar las reglas de negocio
        /// </summary>
        /// <param name="actividad">Actividad con los datos actualizados</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si la actividad es nula</exception>
        /// <exception cref="ArgumentException">Si los datos de la actividad no son válidos</exception>
        public async Task ActualizarActividadAsync(Actividad actividad)
        {
            // Validaciones de negocio
            ValidarActividad(actividad);

            await _repo.GuardarAsync();
        }

        /// <summary>
        /// Elimina una actividad de la base de datos si no tiene reservas asociadas
        /// </summary>
        /// <param name="actividad">Actividad a eliminar</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si la actividad es nula</exception>
        /// <exception cref="InvalidOperationException">Si la actividad tiene reservas asociadas</exception>
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

        /// <summary>
        /// Valida los datos de una actividad según las reglas de negocio
        /// </summary>
        /// <param name="actividad">Actividad a validar</param>
        /// <exception cref="ArgumentNullException">Si la actividad es nula</exception>
        /// <exception cref="ArgumentException">Si el nombre o aforo no cumplen las validaciones</exception>
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

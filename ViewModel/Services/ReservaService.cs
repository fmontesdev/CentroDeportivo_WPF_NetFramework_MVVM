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
    /// Servicio que encapsula la lógica de negocio para la gestión de reservas.
    /// Actúa como intermediario entre el ViewModel y el Repositorio, y coordina con otros servicios
    /// </summary>
    public class ReservaService
    {
        private readonly ReservaRepositorio _reservaRepo;
        private readonly SocioService _socioService;
        private readonly ActividadService _actividadService;

        /// <summary>
        /// Constructor que inicializa el repositorio de reservas y los servicios relacionados
        /// </summary>
        public ReservaService()
        {
            _reservaRepo = new ReservaRepositorio();
            _socioService = new SocioService();
            _actividadService = new ActividadService();
        }

        /// <summary>
        /// Obtiene todas las reservas de la base de datos
        /// </summary>
        /// <returns>Lista de todas las reservas registradas</returns>
        public async Task<List<Reserva>> ObtenerReservasAsync()
        {
            return await _reservaRepo.SeleccionarAsync();
        }

        /// <summary>
        /// Crea una nueva reserva en la base de datos después de validar las reglas de negocio.
        /// Verifica que el socio esté activo, que la actividad exista, que no haya reservas duplicadas
        /// y que no se exceda el aforo máximo de la actividad
        /// </summary>
        /// <param name="reserva">Reserva a crear</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si la reserva es nula</exception>
        /// <exception cref="ArgumentException">Si los datos de la reserva no son válidos o el socio/actividad no existen</exception>
        /// <exception cref="InvalidOperationException">Si ya existe una reserva duplicada o se excede el aforo</exception>
        public async Task CrearReservaAsync(Reserva reserva)
        {
            // Validaciones de negocio
            ValidarReserva(reserva);

            // Verificar que el socio existe y está activo
            var socios = await _socioService.ObtenerSociosActivosAsync();
            var socio = socios.FirstOrDefault(s => s.IdSocio == reserva.IdSocio);
            if (socio == null)
                throw new ArgumentException("El socio seleccionado no existe o no está activo");

            // Verificar que la actividad existe
            var actividades = await _actividadService.ObtenerActividadesAsync();
            var actividad = actividades.FirstOrDefault(a => a.IdActividad == reserva.IdActividad);
            if (actividad == null)
                throw new ArgumentException("La actividad seleccionada no existe");

            // Obtener todas las reservas para validaciones
            var todasLasReservas = await _reservaRepo.SeleccionarAsync();

            // Verificar que el socio no tiene ya una reserva para la misma actividad en la misma fecha
            var reservaDuplicada = todasLasReservas
                .FirstOrDefault(r => r.IdSocio == reserva.IdSocio 
                                   && r.IdActividad == reserva.IdActividad 
                                   && r.Fecha.Date == reserva.Fecha.Date);

            if (reservaDuplicada != null)
                throw new InvalidOperationException(
                    $"El socio '{socio.Nombre}' ya tiene una reserva " +
                    $"para la actividad '{actividad.Nombre}' en la fecha {reserva.Fecha.Date:dd/MM/yyyy}");

            // Verificar que no exceda el aforo
            var reservasEnFecha = todasLasReservas
                .Where(r => r.IdActividad == reserva.IdActividad && r.Fecha.Date == reserva.Fecha.Date)
                .Count();

            if (reservasEnFecha >= actividad.AforoMaximo)
                throw new InvalidOperationException(
                    $"No se puede crear la reserva. La actividad '{actividad.Nombre}' " +
                    $"ha alcanzado su aforo máximo ({actividad.AforoMaximo}) para esta fecha");

            // Crear la reserva en la base de datos
            await _reservaRepo.CrearAsync(reserva);
        }

        /// <summary>
        /// Actualiza los datos de una reserva existente
        /// </summary>
        /// <param name="reserva">Reserva con los datos actualizados</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si la reserva es nula</exception>
        /// <exception cref="InvalidOperationException">Si la reserva no existe</exception>
        public async Task ActualizarReservaAsync(Reserva reserva)
        {
            if (reserva == null)
                throw new ArgumentNullException(nameof(reserva), "La reserva no puede ser nula");

            // Verificar que la reserva existe
            var reservas = await _reservaRepo.SeleccionarAsync();
            var reservaExistente = reservas.FirstOrDefault(r => r.IdReserva == reserva.IdReserva);
            if (reservaExistente == null)
                throw new InvalidOperationException($"No se puede actualizar. La reserva con ID {reserva.IdReserva} no existe");

            // Actualizar la reserva en la base de datos
            await _reservaRepo.GuardarAsync();
        }

        /// <summary>
        /// Elimina una reserva de la base de datos
        /// </summary>
        /// <param name="reserva">Reserva a eliminar</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si la reserva es nula</exception>
        /// <exception cref="InvalidOperationException">Si la reserva no existe</exception>
        public async Task EliminarReservaAsync(Reserva reserva)
        {
            if (reserva == null)
                throw new ArgumentNullException(nameof(reserva), "La reserva no puede ser nula");

            // Verificar que la reserva existe
            var reservas = await _reservaRepo.SeleccionarAsync();
            var reservaExistente = reservas.FirstOrDefault(r => r.IdReserva == reserva.IdReserva);
            if (reservaExistente == null)
                throw new InvalidOperationException($"No se puede eliminar. La reserva con ID {reserva.IdReserva} no existe");

            await _reservaRepo.EliminarAsync(reserva);
        }

        /// <summary>
        /// Valida los datos básicos de una reserva según las reglas de negocio
        /// </summary>
        /// <param name="reserva">Reserva a validar</param>
        /// <exception cref="ArgumentNullException">Si la reserva es nula</exception>
        /// <exception cref="ArgumentException">Si los datos de la reserva no cumplen las validaciones</exception>
        private void ValidarReserva(Reserva reserva)
        {
            if (reserva == null)
                throw new ArgumentNullException(nameof(reserva), "La reserva no puede ser nula");

            if (reserva.IdSocio <= 0)
                throw new ArgumentException("Debe seleccionar un socio válido", nameof(reserva.IdSocio));

            if (reserva.IdActividad <= 0)
                throw new ArgumentException("Debe seleccionar una actividad válida", nameof(reserva.IdActividad));

            if (reserva.Fecha == default(DateTime))
                throw new ArgumentException("La fecha de reserva no puede estar vacía", nameof(reserva.Fecha));

            if (reserva.Fecha.Date < DateTime.Today)
                throw new ArgumentException("La fecha de reserva no puede ser anterior a hoy", nameof(reserva.Fecha));
        }
    }
}

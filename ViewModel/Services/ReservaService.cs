using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Repositorios;

namespace ViewModel.Services
{
    // Servicio que encapsula la lógica de negocio para la gestión de reservas
    // Actúa como intermediario entre el ViewModel y el Repositorio
    public class ReservaService
    {
        private readonly ReservaRepositorio _reservaRepo;
        private readonly SocioService _socioService;
        private readonly ActividadService _actividadService;

        public ReservaService()
        {
            _reservaRepo = new ReservaRepositorio();
            _socioService = new SocioService();
            _actividadService = new ActividadService();
        }

        // Obtiene todas las reservas de la base de datos
        public async Task<List<Reserva>> ObtenerReservasAsync()
        {
            return await _reservaRepo.SeleccionarAsync();
        }

        // Crea una nueva reserva en la base de datos
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

        // Actualiza una reserva existente
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

        // Elimina una reserva de la base de datos
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

        // Valida los datos de una reserva según las reglas de negocio
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

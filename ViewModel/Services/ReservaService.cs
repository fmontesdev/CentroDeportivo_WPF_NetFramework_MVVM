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
        private readonly ReservaRepositorio _repo;
        private readonly SocioRepositorio _socioRepo;
        private readonly ActividadRepositorio _actividadRepo;

        public ReservaService()
        {
            _repo = new ReservaRepositorio();
            _socioRepo = new SocioRepositorio();
            _actividadRepo = new ActividadRepositorio();
        }

        // Obtiene todas las reservas de la base de datos
        public async Task<List<Reserva>> ObtenerReservasAsync()
        {
            return await _repo.SeleccionarAsync();
        }

        // Obtiene todos los socios activos
        public async Task<List<Socio>> ObtenerSociosAsync()
        {
            var socios = await _socioRepo.SeleccionarAsync();
            return socios.Where(s => s.Activo).ToList();
        }

        // Obtiene todas las actividades
        public async Task<List<Actividad>> ObtenerActividadesAsync()
        {
            return await _actividadRepo.SeleccionarAsync();
        }

        // Crea una nueva reserva en la base de datos
        public async Task CrearReservaAsync(Reserva reserva)
        {
            // Validaciones de negocio
            ValidarReserva(reserva);

            // Verificar que el socio existe y está activo
            var socios = await _socioRepo.SeleccionarAsync();
            var socio = socios.FirstOrDefault(s => s.IdSocio == reserva.IdSocio);
            if (socio == null)
                throw new ArgumentException("El socio seleccionado no existe");
            if (!socio.Activo)
                throw new ArgumentException("El socio seleccionado no está activo");

            // Verificar que la actividad existe
            var actividades = await _actividadRepo.SeleccionarAsync();
            var actividad = actividades.FirstOrDefault(a => a.IdActividad == reserva.IdActividad);
            if (actividad == null)
                throw new ArgumentException("La actividad seleccionada no existe");

            // Verificar que no exceda el aforo
            var reservasActividad = await _repo.SeleccionarAsync();
            var reservasEnFecha = reservasActividad
                .Where(r => r.IdActividad == reserva.IdActividad && r.Fecha.Date == reserva.Fecha.Date)
                .Count();

            if (reservasEnFecha >= actividad.AforoMaximo)
                throw new InvalidOperationException(
                    $"No se puede crear la reserva. La actividad '{actividad.Nombre}' " +
                    $"ha alcanzado su aforo máximo ({actividad.AforoMaximo}) para esta fecha");

            await _repo.CrearAsync(reserva);
        }

        // Actualiza una reserva existente
        public async Task ActualizarReservaAsync(Reserva reserva)
        {
            // Validaciones de negocio
            ValidarReserva(reserva);

            await _repo.GuardarAsync();
        }

        // Elimina una reserva de la base de datos
        public async Task EliminarReservaAsync(Reserva reserva)
        {
            if (reserva == null)
                throw new ArgumentNullException(nameof(reserva), "La reserva no puede ser nula");

            await _repo.EliminarAsync(reserva);
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

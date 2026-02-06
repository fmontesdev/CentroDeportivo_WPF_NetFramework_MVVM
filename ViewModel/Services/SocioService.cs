using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Model;
using Model.Repositorios;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio que encapsula la lógica de negocio para la gestión de socios.
    /// Actúa como intermediario entre el ViewModel y el Repositorio
    /// </summary>
    public class SocioService
    {
        private readonly SocioRepositorio _repo;

        /// <summary>
        /// Constructor que inicializa el repositorio de socios
        /// </summary>
        public SocioService()
        {
            _repo = new SocioRepositorio();
        }

        /// <summary>
        /// Obtiene todos los socios de la base de datos
        /// </summary>
        /// <returns>Lista de todos los socios registrados</returns>
        public async Task<List<Socio>> ObtenerSociosAsync()
        {
            return await _repo.SeleccionarAsync();
        }

        /// <summary>
        /// Obtiene todos los socios activos de la base de datos
        /// </summary>
        /// <returns>Lista de socios con estado activo</returns>
        public async Task<List<Socio>> ObtenerSociosActivosAsync()
        {
            var socios = await _repo.SeleccionarAsync();
            return socios.Where(s => s.Activo).ToList();
        }

        /// <summary>
        /// Crea un nuevo socio en la base de datos después de validar las reglas de negocio
        /// </summary>
        /// <param name="socio">Socio a crear</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si el socio es nulo</exception>
        /// <exception cref="ArgumentException">Si los datos del socio no son válidos</exception>
        public async Task CrearSocioAsync(Socio socio)
        {
            // Validaciones de negocio
            ValidarSocio(socio);

            // Por defecto, un nuevo socio está activo
            socio.Activo = true;

            await _repo.CrearAsync(socio);
        }

        /// <summary>
        /// Actualiza los datos de un socio existente después de validar las reglas de negocio
        /// </summary>
        /// <param name="socio">Socio con los datos actualizados</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si el socio es nulo</exception>
        /// <exception cref="ArgumentException">Si los datos del socio no son válidos</exception>
        public async Task ActualizarSocioAsync(Socio socio)
        {
            // Validaciones de negocio
            ValidarSocio(socio);

            await _repo.GuardarAsync();
        }

        /// <summary>
        /// Elimina un socio de la base de datos si no tiene reservas asociadas
        /// </summary>
        /// <param name="socio">Socio a eliminar</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Si el socio es nulo</exception>
        /// <exception cref="InvalidOperationException">Si el socio tiene reservas asociadas</exception>
        public async Task EliminarSocioAsync(Socio socio)
        {
            if (socio == null)
                throw new ArgumentNullException(nameof(socio), "El socio no puede ser nulo");

            // Verificar si tiene reservas activas
            if (socio.Reserva != null && socio.Reserva.Any())
            {
                throw new InvalidOperationException(
                    "No se puede eliminar un socio con reservas asociadas. " +
                    "Primero elimine las reservas o desactive el socio.");
            }

            await _repo.EliminarAsync(socio);
        }

        /// <summary>
        /// Valida los datos de un socio según las reglas de negocio
        /// </summary>
        /// <param name="socio">Socio a validar</param>
        /// <exception cref="ArgumentNullException">Si el socio es nulo</exception>
        /// <exception cref="ArgumentException">Si el nombre o email no cumplen las validaciones</exception>
        private void ValidarSocio(Socio socio)
        {
            if (socio == null)
                throw new ArgumentNullException(nameof(socio), "El socio no puede ser nulo");

            if (string.IsNullOrWhiteSpace(socio.Nombre))
                throw new ArgumentException("El nombre del socio no puede estar vacío", nameof(socio.Nombre));

            if (string.IsNullOrWhiteSpace(socio.Email))
                throw new ArgumentException("El email del socio no puede estar vacío", nameof(socio.Email));

            if (!EsEmailValido(socio.Email))
                throw new ArgumentException("El formato del email no es válido", nameof(socio.Email));
        }

        /// <summary>
        /// Valida el formato de un email usando expresión regular
        /// </summary>
        /// <param name="email">Email a validar</param>
        /// <returns>True si el email tiene un formato válido, false en caso contrario</returns>
        private bool EsEmailValido(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Patrón de email simple: usuario@dominio.extension
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, patron);
        }
    }
}

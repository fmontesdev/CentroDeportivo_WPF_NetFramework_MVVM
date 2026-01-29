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
    // Servicio que encapsula la lógica de negocio para la gestión de socios
    // Actúa como intermediario entre el ViewModel y el Repositorio
    public class SocioService
    {
        private readonly SocioRepositorio _repo;

        public SocioService()
        {
            _repo = new SocioRepositorio();
        }

        // Obtiene todos los socios de la base de datos
        public async Task<List<Socio>> ObtenerSociosAsync()
        {
            return await _repo.SeleccionarAsync();
        }

        // Obtiene todos los socios activos de la base de datos
        public async Task<List<Socio>> ObtenerSociosActivosAsync()
        {
            var socios = await _repo.SeleccionarAsync();
            return socios.Where(s => s.Activo).ToList();
        }

        // Crea un nuevo socio en la base de datos
        public async Task CrearSocioAsync(Socio socio)
        {
            // Validaciones de negocio
            ValidarSocio(socio);

            // Por defecto, un nuevo socio está activo
            socio.Activo = true;

            await _repo.CrearAsync(socio);
        }

        // Actualiza un socio existente
        public async Task ActualizarSocioAsync(Socio socio)
        {
            // Validaciones de negocio
            ValidarSocio(socio);

            await _repo.GuardarAsync();
        }

        // Elimina un socio de la base de datos
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

        // Valida los datos de un socio según las reglas de negocio
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

        // Valida el formato de un email usando expresión regular
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

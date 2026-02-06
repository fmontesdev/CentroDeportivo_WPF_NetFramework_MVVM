using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio para gestionar las operaciones de acceso a datos de actividades
    /// </summary>
    public class ActividadRepositorio
    {
        /// <summary>
        /// Instancia del contexto de la base de datos
        /// </summary>
        private readonly CentroDeportivoEntities _context = new CentroDeportivoEntities();

        /// <summary>
        /// Obtiene todas las actividades de la base de datos incluyendo sus reservas
        /// </summary>
        /// <returns>Lista de todas las actividades con sus reservas asociadas</returns>
        public async Task<List<Actividad>> SeleccionarAsync()
        {
            return await _context.Actividad.Include("Reserva").ToListAsync();
        }

        /// <summary>
        /// Crea una nueva actividad en la base de datos
        /// </summary>
        /// <param name="viaje">Actividad a crear</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task CrearAsync(Actividad viaje)
        {
            _context.Actividad.Add(viaje);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Guarda los cambios realizados en una actividad
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una actividad de la base de datos
        /// </summary>
        /// <param name="viaje">Actividad a eliminar</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task EliminarAsync(Actividad viaje)
        {
            _context.Actividad.Remove(viaje);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Inicia una transacción en la base de datos
        /// </summary>
        /// <returns>Objeto que representa la transacción iniciada</returns>
        public DbContextTransaction IniciarTransaccion()
        {
            return _context.Database.BeginTransaction();
        }

        /// <summary>
        /// Obtiene el contexto actual de Entity Framework
        /// </summary>
        /// <returns>Contexto de Entity Framework para operaciones dentro de transacciones</returns>
        public CentroDeportivoEntities ObtenerContexto()
        {
            return _context;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repositorios
{
    public class ActividadRepositorio
    {
        // Instancia del contexto de la base de datos
        private readonly CentroDeportivoEntities _context = new CentroDeportivoEntities();

        // Obtiene todas las actividades
        public async Task<List<Actividad>> SeleccionarAsync()
        {
            return await _context.Actividad.Include("Reserva").ToListAsync();
        }

        // Crea una nueva actividad
        public async Task CrearAsync(Actividad viaje)
        {
            _context.Actividad.Add(viaje);
            await _context.SaveChangesAsync();
        }

        // Guarda los cambios realizados en una actividad
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Elimina una actividad
        public async Task EliminarAsync(Actividad viaje)
        {
            _context.Actividad.Remove(viaje);
            await _context.SaveChangesAsync();
        }

        // Inicia una transacción en base de datos
        public DbContextTransaction IniciarTransaccion()
        {
            return _context.Database.BeginTransaction();
        }

        // Obtiene el contexto actual (para operaciones dentro de transacciones)
        public CentroDeportivoEntities ObtenerContexto()
        {
            return _context;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Repositorios
{
    public class ReservaRepositorio
    {
        // Instancia del contexto de la base de datos
        private readonly CentroDeportivoEntities _context = new CentroDeportivoEntities();

        // Obtiene todas las reservas
        public async Task<List<Reserva>> SeleccionarAsync()
        {
            return await _context.Reserva.Include("Socio").Include("Actividad").ToListAsync();
        }

        // Crea un nueva reserva
        public async Task CrearAsync(Reserva reserva)
        {
            _context.Reserva.Add(reserva);
            await _context.SaveChangesAsync();
        }

        // Guarda los cambios realizados en una reserva
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Elimina una reserva
        public async Task EliminarAsync(Reserva reserva)
        {
            _context.Reserva.Remove(reserva);
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

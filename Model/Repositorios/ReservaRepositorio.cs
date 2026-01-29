using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataSets;

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

        // Obtiene el DataSet de reservas por actividad para el informe
        public async Task<dsReservasPorActividad> ObtenerDataSetReservasPorActividadAsync(int idActividad)
        {
            // Consulta con LINQ
            var datos = await _context.Reserva
                .Where(r => r.IdActividad == idActividad)
                .Select(r => new
                {
                    NombreActividad = r.Actividad.Nombre,
                    FechaReserva = r.Fecha,
                    NombreSocio = r.Socio.Nombre,
                    AforoMaximo = r.Actividad.AforoMaximo
                })
                .OrderBy(d => d.FechaReserva)
                .ToListAsync();

            // Crear instancia del DataSet
            var dataSet = new dsReservasPorActividad();

            // Mapear datos al DataSet
            foreach (var dato in datos)
            {
                dataSet._dsReservasPorActividad.AdddsReservasPorActividadRow(
                    dato.NombreActividad,
                    dato.FechaReserva,
                    dato.NombreSocio,
                    dato.AforoMaximo
                );
            }

            return dataSet;
        }

        // Obtiene el DataSet de historial de reservas para el informe
        public async Task<dsReservasHistorial> ObtenerDataSetReservasHistorialAsync()
        {
            // Consulta con LINQ
            var datos = await _context.Reserva
                .Select(r => new
                {
                    NombreSocio = r.Socio.Nombre,
                    NombreActividad = r.Actividad.Nombre,
                    FechaReserva = r.Fecha
                })
                .OrderBy(d => d.NombreSocio)
                .ThenBy(d => d.FechaReserva)
                .ToListAsync();

            // Crear instancia del DataSet
            var dataSet = new dsReservasHistorial();

            // Mapear datos al DataSet
            foreach (var dato in datos)
            {
                dataSet._dsReservasHistorial.AdddsReservasHistorialRow(
                    dato.NombreSocio,
                    dato.NombreActividad,
                    dato.FechaReserva
                );
            }

            return dataSet;
        }
    }
}

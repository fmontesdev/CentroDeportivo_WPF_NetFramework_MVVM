using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataSets;

namespace Model.Repositorios
{
    /// <summary>
    /// Repositorio para gestionar las operaciones de acceso a datos de reservas
    /// </summary>
    public class ReservaRepositorio
    {
        /// <summary>
        /// Instancia del contexto de la base de datos
        /// </summary>
        private readonly CentroDeportivoEntities _context = new CentroDeportivoEntities();

        /// <summary>
        /// Obtiene todas las reservas de la base de datos incluyendo sus socios y actividades
        /// </summary>
        /// <returns>Lista de todas las reservas con sus entidades relacionadas</returns>
        public async Task<List<Reserva>> SeleccionarAsync()
        {
            return await _context.Reserva.Include("Socio").Include("Actividad").ToListAsync();
        }

        /// <summary>
        /// Crea una nueva reserva en la base de datos
        /// </summary>
        /// <param name="reserva">Reserva a crear</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task CrearAsync(Reserva reserva)
        {
            _context.Reserva.Add(reserva);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Guarda los cambios realizados en una reserva
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina una reserva de la base de datos
        /// </summary>
        /// <param name="reserva">Reserva a eliminar</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task EliminarAsync(Reserva reserva)
        {
            _context.Reserva.Remove(reserva);
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

        /// <summary>
        /// Obtiene el DataSet de reservas por actividad optimizado para informes de Crystal Reports
        /// </summary>
        /// <param name="idActividad">Identificador de la actividad para filtrar las reservas</param>
        /// <returns>DataSet tipado con las reservas de la actividad especificada</returns>
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

        /// <summary>
        /// Obtiene el DataSet de historial de reservas optimizado para informes de Crystal Reports
        /// </summary>
        /// <returns>DataSet tipado con el historial completo de reservas ordenado por socio y fecha</returns>
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

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
    /// Repositorio para gestionar las operaciones de acceso a datos de socios
    /// </summary>
    public class SocioRepositorio
    {
        /// <summary>
        /// Instancia del contexto de la base de datos
        /// </summary>
        private readonly CentroDeportivoEntities _context = new CentroDeportivoEntities();

        /// <summary>
        /// Obtiene todos los socios de la base de datos incluyendo sus reservas
        /// </summary>
        /// <returns>Lista de todos los socios con sus reservas asociadas</returns>
        public async Task<List<Socio>> SeleccionarAsync()
        {
            return await _context.Socio.Include("Reserva").ToListAsync();
        }

        /// <summary>
        /// Crea un nuevo socio en la base de datos
        /// </summary>
        /// <param name="entity">Socio a crear</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task CrearAsync(Socio entity)
        {
            _context.Socio.Add(entity);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Guarda los cambios realizados en un socio
        /// </summary>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Elimina un socio de la base de datos
        /// </summary>
        /// <param name="entity">Socio a eliminar</param>
        /// <returns>Tarea que representa la operación asíncrona</returns>
        public async Task EliminarAsync(Socio entity)
        {
            _context.Socio.Remove(entity);
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
        /// Obtiene el DataSet de socios optimizado para informes de Crystal Reports
        /// </summary>
        /// <returns>DataSet tipado con la información de todos los socios</returns>
        public async Task<dsSocios> ObtenerDataSetSociosAsync()
        {
            // Obtener todos los socios
            var socios = await SeleccionarAsync();

            // Crear instancia del DataSet
            var dataSet = new dsSocios();

            // Mapear entidades al DataSet
            foreach (var socio in socios)
            {
                dataSet._dsSocios.AdddsSociosRow(
                    socio.IdSocio,
                    socio.Nombre,
                    socio.Email,
                    socio.Activo ? "Activo" : "Inactivo"  // Convierte booleano a texto
                );
            }

            return dataSet;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.DataSets;

namespace Model.Repositorios
{
    public class SocioRepositorio
    {
        // Instancia del contexto de la base de datos
        private readonly CentroDeportivoEntities _context = new CentroDeportivoEntities();

        //Obtiene todos los socios
        public async Task<List<Socio>> SeleccionarAsync()
        {
            return await _context.Socio.Include("Reserva").ToListAsync();
        }

        // Crea un nuevo socio
        public async Task CrearAsync(Socio entity)
        {
            _context.Socio.Add(entity);
            await _context.SaveChangesAsync();
        }

        // Guarda los cambios realizados en un socio
        public async Task GuardarAsync()
        {
            await _context.SaveChangesAsync();
        }

        // Elimina un socio
        public async Task EliminarAsync(Socio entity)
        {
            _context.Socio.Remove(entity);
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

        // Obtiene el DataSet de socios para el informe
        // Consulta optimizada para Crystal Reports
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

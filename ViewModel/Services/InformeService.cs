using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Model.DataSets;
using Model.Repositorios;

namespace ViewModel.Services
{
    /// <summary>
    /// Servicio para la generación de informes con Crystal Reports.
    /// Coordina la obtención de datos desde los repositorios y su transformación a DataSets
    /// </summary>
    public class InformeService
    {
        private readonly SocioRepositorio _socioRepo;
        private readonly ReservaRepositorio _reservaRepo;

        /// <summary>
        /// Constructor que inicializa los repositorios necesarios
        /// </summary>
        public InformeService()
        {
            _socioRepo = new SocioRepositorio();
            _reservaRepo = new ReservaRepositorio();
        }

        /// <summary>
        /// Genera el DataSet para el informe de listado de socios
        /// </summary>
        /// <returns>DataSet tipado con todos los socios de la base de datos</returns>
        public async Task<dsSocios> GenerarDataSetSociosAsync()
        {
            return await _socioRepo.ObtenerDataSetSociosAsync();
        }

        /// <summary>
        /// Genera el DataSet para el informe de reservas por actividad
        /// </summary>
        /// <param name="idActividad">Identificador de la actividad para filtrar las reservas</param>
        /// <returns>DataSet tipado con las reservas de la actividad especificada</returns>
        public async Task<dsReservasPorActividad> GenerarDataSetReservasPorActividadAsync(int idActividad)
        {
            return await _reservaRepo.ObtenerDataSetReservasPorActividadAsync(idActividad);
        }

        /// <summary>
        /// Genera el DataSet para el informe de historial de reservas agrupado por socio
        /// </summary>
        /// <returns>DataSet tipado con el historial completo de reservas ordenado cronológicamente por socio</returns>
        public async Task<dsReservasHistorial> GenerarDataSetHistorialReservasAsync()
        {
            return await _reservaRepo.ObtenerDataSetReservasHistorialAsync();
        }
    }
}

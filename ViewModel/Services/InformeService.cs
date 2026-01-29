using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Model.DataSets;
using Model.Repositorios;

namespace ViewModel.Services
{
    // Servicio para la generación de informes
    public class InformeService
    {
        private readonly SocioRepositorio _socioRepo;
        private readonly ReservaRepositorio _reservaRepo;

        public InformeService()
        {
            _socioRepo = new SocioRepositorio();
            _reservaRepo = new ReservaRepositorio();
        }

        // Genera el DataSet para el informe listado de socios
        // Obtiene todos los socios de la base de datos
        public async Task<dsSocios> GenerarDataSetSociosAsync()
        {
            return await _socioRepo.ObtenerDataSetSociosAsync();
        }

        // Genera el DataSet para el informe reservas por actividad
        // Filtra las reservas por el IdActividad proporcionado
        public async Task<dsReservasPorActividad> GenerarDataSetReservasPorActividadAsync(int idActividad)
        {
            return await _reservaRepo.ObtenerDataSetReservasPorActividadAsync(idActividad);
        }

        // Genera el DataSet para el informe historial de reservas por socio
        // Obtiene todas las reservas agrupadas por socio y ordenadas cronológicamente
        public async Task<dsReservasHistorial> GenerarDataSetHistorialReservasAsync()
        {
            return await _reservaRepo.ObtenerDataSetReservasHistorialAsync();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Models
{
    // Enumeración que representa los tipos de informes disponibles
    public enum TipoInforme
    {
        // Informe maestro de socios (listado completo)
        ListadoSocios,

        // Informe de reservas filtrado por actividad
        ListadoReservasPorActividad,

        // Informe de historial de reservas agrupado por socio
        HistorialReservas
    }
}

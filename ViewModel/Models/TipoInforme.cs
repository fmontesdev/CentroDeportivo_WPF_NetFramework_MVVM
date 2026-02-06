using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Models
{
    /// <summary>
    /// Enumeración que define los tipos de informes disponibles en el sistema
    /// </summary>
    public enum TipoInforme
    {
        /// <summary>
        /// Informe maestro de socios (listado completo con estado)
        /// </summary>
        ListadoSocios,

        /// <summary>
        /// Informe de reservas filtrado por actividad con cálculo de ocupación
        /// </summary>
        ListadoReservasPorActividad,

        /// <summary>
        /// Informe de historial de reservas agrupado por socio y ordenado cronológicamente
        /// </summary>
        HistorialReservas
    }
}

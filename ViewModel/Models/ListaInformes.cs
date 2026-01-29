using System.Collections.Generic;

namespace ViewModel.Models
{
    // Clase que contiene la lista de informes disponibles en el sistema
    public static class ListaInformes
    {
        // Obtiene la lista de todos los informes disponibles
        public static List<Informe> ObtenerInformesDisponibles()
        {
            return new List<Informe>
            {
                new Informe
                {
                    Tipo = TipoInforme.ListadoSocios,
                    Titulo = "📊 Listado de Socios",
                    Descripcion = "Listado completo de todos los socios registrados con su estado. Los socios inactivos aparecen en color rojo."
                },
                new Informe
                {
                    Tipo = TipoInforme.ListadoReservasPorActividad,
                    Titulo = "🏃 Listado de Reservas por Actividad",
                    Descripcion = "Hoja de asistencia para una actividad específica con cálculo de ocupación. Selecciona una actividad para ver la lista de socios que han reservado y el porcentaje de ocupación."
                },
                new Informe
                {
                    Tipo = TipoInforme.HistorialReservas,
                    Titulo = "📅 Historial de Reservas",
                    Descripcion = "Historial completo de reservas agrupadas por socio y ordenadas cronológicamente."
                }
            };
        }
    }
}

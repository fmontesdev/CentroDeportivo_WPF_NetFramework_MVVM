using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel.Models
{
    // Información de un informe disponible
    public class Informe
    {
        // Tipo de informe
        public TipoInforme Tipo { get; set; }

        // Título del informe
        public string Titulo { get; set; }

        // Descripción del informe
        public string Descripcion { get; set; }
    }
}

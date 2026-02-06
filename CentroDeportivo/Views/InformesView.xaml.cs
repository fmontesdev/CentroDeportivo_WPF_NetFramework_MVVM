using System.Windows.Controls;
using ViewModel;

namespace CentroDeportivo.Views
{
    /// <summary>
    /// Code-behind para InformesView.xaml.
    /// Vista para la generación y visualización de informes del centro deportivo
    /// </summary>
    public partial class InformesView : UserControl
    {
        /// <summary>
        /// Constructor que inicializa la vista y configura el ViewModel.
        /// El ViewModel maneja toda la lógica de generación de informes
        /// </summary>
        public InformesView()
        {
            InitializeComponent();

            // Crear ViewModel y asignar DataContext
            // El ViewModel maneja toda la lógica (incluida la apertura de ventanas)
            this.DataContext = new InformeViewModel();
        }
    }
}

using System.Windows.Controls;
using ViewModel;

namespace CentroDeportivo.Views
{
    // Code-behind para InformesView.xaml
    public partial class InformesView : UserControl
    {
        public InformesView()
        {
            InitializeComponent();

            // Crear ViewModel y asignar DataContext
            // El ViewModel maneja toda la lógica (incluida la apertura de ventanas)
            this.DataContext = new InformeViewModel();
        }
    }
}

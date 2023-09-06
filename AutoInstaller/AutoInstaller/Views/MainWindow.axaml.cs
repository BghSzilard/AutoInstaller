using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AutoInstaller.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var linearGradientBrush = new LinearGradientBrush
            {
	            GradientStops = new GradientStops
	            {
		            new GradientStop(Color.Parse("#006487"), 0.0),
		            new GradientStop(Color.Parse("#0579a2"), 1.0),
	            },
	            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
	            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            };

            // Set the window's background to the linear gradient brush
            Background = linearGradientBrush;
        }
    }
}
using System.Windows;
using System.Windows.Controls;

namespace AnalysisEffectOfError
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ApproximationPointControl _approximationPointControl;

        private readonly ApproximationСontinuousControl _approximationСontinuousControl;

        public MainWindow()
        {
            InitializeComponent();

            _approximationPointControl = new ApproximationPointControl();
            _approximationСontinuousControl = new ApproximationСontinuousControl();

            AddControlOnPanel(_approximationPointControl);
        }

        private void ApproximationPointButtonClick(object sender, RoutedEventArgs e)
        {
            AddControlOnPanel(_approximationPointControl);
        }

        private void ApproximationContinuousButtonClick(object sender, RoutedEventArgs e)
        {
            AddControlOnPanel(_approximationСontinuousControl);
        }

        private int AddControlOnPanel(Control controlToAdd)
        {
            MainPanel.Children.Clear();
            int positionIndex = MainPanel.Children.Add(controlToAdd);

            return positionIndex;
        }
    }
}

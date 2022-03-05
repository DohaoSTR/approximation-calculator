using AnalysisEffectOfError.Misc;
using Approximation;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using ZedGraph;

namespace AnalysisEffectOfError
{
    /// <summary>
    /// Interaction logic for ApproximationPointControl.xaml
    /// </summary>
    public partial class ApproximationPointControl : UserControl
    {
        private readonly PointPairList _points = new PointPairList();

        private readonly GraphPane _graphPane;

        public ApproximationPointControl()
        {
            InitializeComponent();

            _graphPane = GraphControl.GraphPane;

            _graphPane.XAxis.MajorGrid.DashOn = 1;
            _graphPane.XAxis.MajorGrid.DashOff = 1;

            _graphPane.YAxis.MajorGrid.DashOn = 1;
            _graphPane.YAxis.MajorGrid.DashOff = 1;

            _graphPane.YAxis.MinorGrid.DashOn = 1;
            _graphPane.YAxis.MinorGrid.DashOff = 1;

            _graphPane.XAxis.MinorGrid.DashOn = 1;
            _graphPane.XAxis.MinorGrid.DashOff = 1;


            _graphPane.YAxis.MajorGrid.IsZeroLine = true;
            _graphPane.XAxis.MajorGrid.IsZeroLine = true;

            _graphPane.XAxis.MajorGrid.IsVisible = true;
            _graphPane.YAxis.MajorGrid.IsVisible = true;

            _graphPane.YAxis.MinorGrid.IsVisible = true;
            _graphPane.XAxis.MinorGrid.IsVisible = true;

            _graphPane.Title.Text = " ";
        }

        private void DrawGraph(PointPairList points, Color color)
        {
            if (color.Name == "Yellow")
            {
                _graphPane.CurveList.Clear();
                LineItem myCurve = _graphPane.AddCurve("", points, Color.Red, SymbolType.None);
                myCurve.Line.Width = 4;
                LineItem p = _graphPane.AddCurve("", _points, Color.Blue, SymbolType.Circle);
                p.Symbol.Fill.Color = Color.Blue;
                p.Symbol.Fill.Type = FillType.Solid;
                p.Symbol.Size = 10;
                p.Line.IsVisible = false;
                GraphControl.AxisChange();
                GraphControl.Invalidate();
            }
            else
            {
                LineItem myCurve = _graphPane.AddCurve("", points, color, SymbolType.None);
                myCurve.Line.Width = 4;
                LineItem p = _graphPane.AddCurve("", _points, Color.Blue, SymbolType.Circle);
                p.Symbol.Fill.Color = Color.Blue;
                p.Symbol.Fill.Type = FillType.Solid;
                p.Symbol.Size = 10;
                p.Line.IsVisible = false;
                GraphControl.AxisChange();
                GraphControl.Invalidate();
            }
        }

        private void AddPointButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _points.Add(new PointPair(double.Parse(ValueTextBox.Text), double.Parse(KeyTextBox.Text)));

                ListBoxItem listBoxItem = new ListBoxItem
                {
                    Content = string.Format($"{ValueTextBox.Text} ; {KeyTextBox.Text}")
                };
                listBoxItem.Selected += CoordinateListBoxSelectionChanged;

                PointListBox.Items.Add(listBoxItem);
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите координаты!");
            }
        }

        private void BuildGraphButtonClick(object sender, RoutedEventArgs e)
        {
            DateTime timeStart = DateTime.Now;

            if (_points.Count < 2)
            {
                MessageBox.Show("Необходимо минимум две координаты!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(StepTextBox.Text))
            {
                MessageBox.Show("Введите шаг!");
                return;
            }
            else if (MethodComboBox.SelectedIndex == 5 &&
                string.IsNullOrWhiteSpace(PowerTextBox.Text))
            {
                MessageBox.Show("Введите степень!");
                return;
            }
            else
            {
                PointPairList result = new PointPairList();

                ApproximationPoint approximationPoint = new ApproximationPoint(_points.ConvertToIEnumerable());

                switch (MethodComboBox.SelectedIndex)
                {
                    case 0: { result = result.ConvertToPointPairList(approximationPoint.MethodLinearInterpolation(Convert.ToDouble(StepTextBox.Text))); } break;
                    case 1: { result = result.ConvertToPointPairList(approximationPoint.MethodSquareInterpolation(Convert.ToDouble(StepTextBox.Text))); } break;
                    case 2: { result = result.ConvertToPointPairList(approximationPoint.MethodCubicInterpolation(Convert.ToDouble(StepTextBox.Text))); } break;
                    case 3: { result = result.ConvertToPointPairList(approximationPoint.LagrandePolynomial(Convert.ToDouble(StepTextBox.Text))); } break;
                    case 4: { result = result.ConvertToPointPairList(approximationPoint.NewtonPolynomial(Convert.ToDouble(StepTextBox.Text))); } break;
                    case 5: { result = result.ConvertToPointPairList(approximationPoint.LeastSquareMethod(Convert.ToDouble(StepTextBox.Text), int.Parse(PowerTextBox.Text))); } break;
                    default:
                        break;
                }

                DrawGraph(result, Color.Yellow);

                DateTime dateTime = new DateTime();
                SpeedTextBlock.Text = "Время работы: " + dateTime.GetTimeDifference(timeStart);

                Deviation deviation = new Deviation(PointPairListExtension.ConvertToIEnumerable(result));
                AbsSquareTextBlock.Text = "Абсолютное отклонение: " + deviation.AbsMethod().ToString("N2");
                RootSquareTextBlock.Text = "Среднеквадратичное отклонение: " + deviation.RootSquareMethod().ToString("N2");

                AbsSquareTextBlock.Visibility = Visibility.Visible;

                RootSquareTextBlock.Visibility = Visibility.Visible;

                SpeedTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void CoordinateListBoxSelectionChanged(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem
            {
                Header = "Удалить"
            };
            menuItem.Click += RemovePointButtonClick;

            contextMenu.Items.Add(menuItem);

            contextMenu.IsOpen = true;
        }

        private void RemovePointButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _points.RemoveAt(PointListBox.SelectedIndex - 1);
                PointListBox.Items.Remove(PointListBox.SelectedItem);
            }
            catch (ArgumentOutOfRangeException)
            {
                MessageBox.Show("Выберите координаты!");
            }
        }

        private void MethodBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MethodComboBox.SelectedIndex == 5)
            {
                PowerTextBox.Visibility = Visibility.Visible;
                PowerTextBlock.Visibility = Visibility.Visible;
            }
            else if (PowerTextBox != null)
            {
                PowerTextBlock.Visibility = Visibility.Collapsed;
                PowerTextBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}

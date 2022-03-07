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
    /// Interaction logic for ApproximationСontinuousControl.xaml
    /// </summary>
    public partial class ApproximationСontinuousControl : UserControl
    {
        private readonly PointPairList _points = new PointPairList();

        private readonly GraphPane _graphPane;

        public ApproximationСontinuousControl()
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

        private void BuildGraphButtonClick(object sender, RoutedEventArgs e)
        {
            DateTime timeStart = DateTime.Now;

            if (string.IsNullOrWhiteSpace(StartIntervalTextBox.Text) || string.IsNullOrWhiteSpace(EndIntervalTextBox.Text))
            {
                MessageBox.Show("Введите интервал!");
                return;
            }

            if (Convert.ToDouble(StartIntervalTextBox.Text) >= Convert.ToDouble(EndIntervalTextBox.Text))
            {
                MessageBox.Show("Начало интервала должно быть больше его конца!");
                return;
            }

            if (string.IsNullOrWhiteSpace(StepTextBox.Text))
            {
                MessageBox.Show("Введите шаг!");
                return;
            }

            if (MethodComboBox.SelectedIndex == 5 && string.IsNullOrWhiteSpace(PowerTextBox.Text))
            {
                MessageBox.Show("Введите степень!");
                return;
            }

            if (MethodComboBox.SelectedIndex == 0 && string.IsNullOrWhiteSpace(LocationTextBox.Text))
            {
                MessageBox.Show("Введите окрестность!");
                return;
            }
            else
            {
                PointPairList result = new PointPairList();

                Function function = new Function(FunctionTextBox.Text);

                Interval interval = new Interval(double.Parse(StartIntervalTextBox.Text), double.Parse(EndIntervalTextBox.Text));
                double step = Convert.ToDouble(StepTextBox.Text);
                int power = Convert.ToInt32(PowerTextBox.Text);

                ApproximationСontinuous approximationContinuous = new ApproximationСontinuous(function, interval, power, step);

                switch (MethodComboBox.SelectedIndex)
                {
                    case 0:
                        {
                            result = result.ConvertToPointPairList(approximationContinuous.TaylorSeriesMethod(double.Parse(LocationTextBox.Text)));
                        }
                        break;
                    case 1:
                        {
                            result = result.ConvertToPointPairList(approximationContinuous.ChebyshevPolynomial());
                        }
                        break;
                    case 2:
                        {
                            result = result.ConvertToPointPairList(approximationContinuous.FourierSeriesMethod());
                        }
                        break;
                    default:
                        break;
                }

                DrawGraph(result, Color.Yellow);

                ApproximationError approximationError = new ApproximationError(PointPairListExtension.ConvertToIEnumerable(result));

                DateTime dateTime = new DateTime();
                SpeedTextBlock.Text = "Время работы: " + dateTime.GetTimeDifference(timeStart) + " секунд.";
                AbsoluteDeviationTextBlock.Text = "Абсолютное отклонение: " + approximationError.AbsoluteDeviation().ToString("N2");
                RootSquareDeviationTextBlock.Text = "Среднеквадратичное отклонение: " + approximationError.RootSquareDeviation().ToString("N2");

                MeanAbsoluteDeviationTextBlock.Text = "Среднее абсолютное отклонение: " + approximationError.MeanAbsoluteDeviation().ToString("N2");
                XSquareTextBlock.Text = "Коэффициент минимум хи-квадрат: " + approximationError.XSquare(function).ToString("N2");
                MaxLikelihoodTextBlock.Text = "Коэффициент максимального правдоподобия: " + approximationError.MaxLikelihood(function).ToString("N2");

                AverageRelativeApproximationErrorTextBlock.Text = "Относительная ошибка аппроксимации: " + approximationError.AverageRelativeApproximationError(function).ToString("N2");
                ApproximationQualityTextBlock.Text = "Качество аппроксимации: " + approximationError.ApproximationQuality(function).ToString("N2");

                ErrorPanel.Visibility = Visibility.Visible;
            }
        }

        private void MethodBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MethodComboBox.SelectedIndex == 0 && LocationTextBox != null)
            {
                LocationTextBlock.Visibility = Visibility.Visible;
                LocationTextBox.Visibility = Visibility.Visible;
            }
            else if (LocationTextBox != null)
            {
                LocationTextBlock.Visibility = Visibility.Collapsed;
                LocationTextBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}

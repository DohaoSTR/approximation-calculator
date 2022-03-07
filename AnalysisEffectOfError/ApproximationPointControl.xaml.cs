using AnalysisEffectOfError.Misc;
using Approximation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using ZedGraph;
using Point = Approximation.Point;

namespace AnalysisEffectOfError
{
    /// <summary>
    /// Interaction logic for ApproximationPointControl.xaml
    /// </summary>
    public partial class ApproximationPointControl : UserControl
    {
        private readonly GraphPane _graphPane;

        private readonly DiscreteFunction _function;

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

            _function = new DiscreteFunction();
        }

        private void DrawGraph(PointPairList points, Color color)
        {
            IEnumerable<Point> funcPoints = _function.GetPoints();

            PointPairList pointPairs = new PointPairList();
            pointPairs = pointPairs.ConvertToPointPairList(funcPoints);

            if (color.Name == "Yellow")
            {
                _graphPane.CurveList.Clear();
                LineItem myCurve = _graphPane.AddCurve("", points, Color.Red, SymbolType.None);
                myCurve.Line.Width = 4;
                LineItem p = _graphPane.AddCurve("", pointPairs, Color.Blue, SymbolType.Circle);
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
                LineItem p = _graphPane.AddCurve("", pointPairs, Color.Blue, SymbolType.Circle);
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
                _function.Add(new Point(double.Parse(ValueTextBox.Text), double.Parse(KeyTextBox.Text)));

                ListBoxItem listBoxItem = new ListBoxItem
                {
                    Content = string.Format($"{ValueTextBox.Text} ; {KeyTextBox.Text}")
                };
                listBoxItem.Selected += CoordinateListBoxSelectionChanged;

                PointListBox.Items.Add(listBoxItem);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.ParamName);
            }
            catch (FormatException)
            {
                MessageBox.Show("Введите координаты!");
            }
        }

        private void BuildGraphButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StepTextBox.Text))
            {
                MessageBox.Show("Введите шаг!");
                return;
            }

            if (MethodComboBox.SelectedIndex == 5 &&
                string.IsNullOrWhiteSpace(PowerTextBox.Text))
            {
                MessageBox.Show("Введите степень!");
                return;
            }

            try
            {
                DateTime timeStart = DateTime.Now;

                PointPairList result = new PointPairList();

                double step = Convert.ToDouble(StepTextBox.Text);

                ApproximationPoint approximationPoint = new ApproximationPoint(_function, step);

                switch (MethodComboBox.SelectedIndex)
                {
                    case 0: { result = result.ConvertToPointPairList(approximationPoint.MethodLinearInterpolation()); } break;
                    case 1: { result = result.ConvertToPointPairList(approximationPoint.MethodSquareInterpolation()); } break;
                    case 2: { result = result.ConvertToPointPairList(approximationPoint.MethodCubicInterpolation()); } break;
                    case 3: { result = result.ConvertToPointPairList(approximationPoint.LagrandePolynomial()); } break;
                    case 4: { result = result.ConvertToPointPairList(approximationPoint.NewtonPolynomial()); } break;
                    case 5: { result = result.ConvertToPointPairList(approximationPoint.LeastSquareMethod(int.Parse(PowerTextBox.Text))); } break;
                    default:
                        break;
                }

                DrawGraph(result, Color.Yellow);

                ApproximationError approximationError = new ApproximationError(PointPairListExtension.ConvertToIEnumerable(result));
                Function function = new Function("x");

                DateTime dateTime = new DateTime();
                SpeedTextBlock.Text = "Время работы: " + dateTime.GetTimeDifference(timeStart) + " секунд.";
                AbsoluteDeviationTextBlock.Text = "Абсолютное отклонение: " + approximationError.AbsoluteDeviation().ToString("N2");
                RootSquareDeviationTextBlock.Text = "Среднеквадратичное отклонение: " + approximationError.RootSquareDeviation().ToString("N2");

                MeanAbsoluteDeviationTextBlock.Text = "Среднее абсолютное отклонение: " + approximationError.MeanAbsoluteDeviation().ToString("N2");
                XSquareTextBlock.Text = "Коэффициент минимум хи-квадрат: " + approximationError.XSquare(function).ToString("N2");
                MaxLikelihoodTextBlock.Text = "Коэффициент максимального правдоподобия: " + approximationError.MaxLikelihood(function).ToString("N2");

                AverageRelativeApproximationErrorTextBlock.Text = "Относительная ошибка аппроксимации: " + approximationError.AverageRelativeApproximationError(function).ToString("N2");
                ApproximationQualityTextBlock.Text = "Качество аппроксимации: " + approximationError.ApproximationQuality(function).ToString("N2");

                if (MethodComboBox.SelectedIndex == 5)
                {
                    RateLeastSquareMethodTextBlock.Visibility = Visibility.Visible;
                    RateLeastSquareMethodTextBlock.Text = "Оценка МНК: " + approximationError.ApproximationQuality(function).ToString("N2");
                }
                else
                {
                    RateLeastSquareMethodTextBlock.Visibility = Visibility.Collapsed;
                }

                ErrorPanel.Visibility = Visibility.Visible;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show(ex.ParamName);
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
                _function.RemoveAt(PointListBox.SelectedIndex - 1);

                PointListBox.Items.RemoveAt(PointListBox.SelectedIndex);
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

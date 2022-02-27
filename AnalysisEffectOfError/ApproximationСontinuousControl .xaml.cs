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
                LineItem myCurve = _graphPane.AddCurve("", points, Color.Blue, SymbolType.None);
                myCurve.Line.Width = 4;
                LineItem p = _graphPane.AddCurve("", _points, Color.Red, SymbolType.Circle);
                p.Symbol.Fill.Color = Color.Red;
                p.Symbol.Fill.Type = FillType.Solid;
                p.Symbol.Size = 5;
                p.Line.IsVisible = false;
                GraphControl.AxisChange();
                GraphControl.Invalidate();
            }
            else
            {
                LineItem myCurve = _graphPane.AddCurve("", points, color, SymbolType.None);
                myCurve.Line.Width = 4;
                LineItem p = _graphPane.AddCurve("", _points, Color.Red, SymbolType.Circle);
                p.Symbol.Fill.Color = Color.Red;
                p.Symbol.Fill.Type = FillType.Solid;
                p.Symbol.Size = 10;
                p.Line.IsVisible = false;
                GraphControl.AxisChange();
                GraphControl.Invalidate();
            }
        }

        private void BuildGraphButtonClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(StartIntervalTextBox.Text) || string.IsNullOrWhiteSpace(EndIntervalTextBox.Text))
            {
                MessageBox.Show("Введите интервал!");
                return;
            }
            else if (Convert.ToDouble(StartIntervalTextBox.Text) >= Convert.ToDouble(EndIntervalTextBox.Text))
            {
                MessageBox.Show("Начало интервала должно быть больше его конца!");
                return;
            }
            else if (string.IsNullOrWhiteSpace(StepTextBox.Text))
            {
                MessageBox.Show("Введите шаг!");
                return;
            }
            else if (MethodComboBox.SelectedIndex == 5 && string.IsNullOrWhiteSpace(PowerTextBox.Text))
            {
                MessageBox.Show("Введите степень!");
                return;
            }
            else if (MethodComboBox.SelectedIndex == 0 && string.IsNullOrWhiteSpace(LocationTextBox.Text))
            {
                MessageBox.Show("Введите окрестность!");
                return;
            }
            else
            {
                PointPairList result = new PointPairList();

                Function function = new Function(FunctionTextBox.Text);
                ApproximationСontinuous approximationContinuous = new ApproximationСontinuous(function);

                Interval interval = new Interval(double.Parse(StartIntervalTextBox.Text), double.Parse(EndIntervalTextBox.Text));

                switch (MethodComboBox.SelectedIndex)
                {
                    case 0:
                        {
                            result = result.ConvertToPointPairList(approximationContinuous.TaylorSeriesMethod(double.Parse(LocationTextBox.Text),
                                int.Parse(PowerTextBox.Text), Convert.ToDouble(StepTextBox.Text), interval));
                        }
                        break;
                    case 1:
                        {
                            result = result.ConvertToPointPairList(approximationContinuous.ChebyshevPolynomial(int.Parse(PowerTextBox.Text),
                                Convert.ToDouble(StepTextBox.Text), interval));
                        }
                        break;
                    case 2:
                        {
                            result = result.ConvertToPointPairList(approximationContinuous.FourierSeriesMethod(int.Parse(PowerTextBox.Text),
                                Convert.ToDouble(StepTextBox.Text), interval));
                        }
                        break;
                    default:
                        break;
                }

                DrawGraph(result, Color.Yellow);
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
                LocationTextBlock.Visibility = Visibility.Hidden;
                LocationTextBox.Visibility = Visibility.Hidden;
            }
        }
    }
}

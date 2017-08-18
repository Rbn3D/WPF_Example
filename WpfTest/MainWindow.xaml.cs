using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LiveCharts.Definitions.Series;
using LiveCharts.Helpers;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using MahApps.Metro.Controls;
using System.Windows.Threading;

namespace WpfTest
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        List<Currency> Currencies { get; set; }
        private Currency[] mArrayCurrencies; // stored for performance

        private Random random = new Random();

        public SeriesCollection Series { get; private set; }

        private DateTime InitDate = DateTime.Now;

        private DispatcherTimer chartTimer;

        public MainWindow()
        {
            InitializeComponent();

            InitializeCurrencies();

            PopulateIntialChartData();

            // Bind Echange rates combos
            ExchangeFrom.ItemsSource = Currencies;
            ExchangeTo.ItemsSource = Currencies;

            // Timer to do custom UI animations
            DispatcherTimer uiTimer = new DispatcherTimer(DispatcherPriority.Render);
            uiTimer.Interval = TimeSpan.FromSeconds(1d / 60d); // 60 times per second
            uiTimer.Tick += UiTimerTick;            
            uiTimer.Start();

            chartTimer = new DispatcherTimer(DispatcherPriority.Normal);
            chartTimer.Interval = TimeSpan.FromSeconds(1d);
            chartTimer.Tick += ChartTimerTick;
            chartTimer.Start();
        }

        private void UiTimerTick(object sender, EventArgs e)
        {

        }

        private void ChartTimerTick(object sender, EventArgs e)
        {
            UpdateChart();
        }

        private void UpdateChart()
        {
            var r = random;

            for (var i = 0; i < mArrayCurrencies.Length; i++)
            {
                var curr = mArrayCurrencies[i];

                curr.CurrentTrend += (r.NextDouble() < .8 ? 1 : -1) * r.Next(-10, 10);
                var newVal = curr.CurrentTrend;

                curr.ChartSerie.Values.Add(newVal);
            }
        }

        private void PopulateIntialChartData()
        {
            mArrayCurrencies = Currencies.ToArray();

            this.currencyList.ItemsSource = Currencies;

            Series = new SeriesCollection();
            var r = random;

            for (var i = 0; i < mArrayCurrencies.Length; i++)
            {
                var curr = mArrayCurrencies[i];
                var values = new double[100];

                var initialElements = 100;

                for (var j = 0; j < initialElements; j++)
                {
                    curr.CurrentTrend += (r.NextDouble() < .8 ? 1 : -1) * r.Next(-10, 10);
                    values[j] = curr.CurrentTrend;
                }

                var fillBr = new SolidColorBrush(mArrayCurrencies[i].Color);
                fillBr.Opacity = 0.0f; // Transparent

                var serie = new LineSeries
                {
                    Values = values.AsChartValues(),
                    Fill = fillBr,
                    StrokeThickness = .8,
                    Stroke = new SolidColorBrush(mArrayCurrencies[i].Color),
                    PointGeometry = null
                };
                Series.Add(serie);
                curr.ChartSerie = serie;
            }

            currencyChart.Series = Series;
            currencyChart.AxisX.Add(new Axis()
            {
                LabelFormatter = value => {
                        DateTime clone = InitDate.AddSeconds(value);
                        return clone.ToString("HH:mm:ss");
                    },
                Separator = new LiveCharts.Wpf.Separator()
            });
        }

        private void InitializeCurrencies()
        {
            List<Currency> mCurrencies = new List<Currency>();

            mCurrencies.Add(new Currency() { Name = "EUR", Color = Colors.Blue });
            mCurrencies.Add(new Currency() { Name = "USD", Color = Colors.Green });
            mCurrencies.Add(new Currency() { Name = "BTC", Color = Colors.Orange });
            mCurrencies.Add(new Currency() { Name = "ETR", Color = Colors.LightBlue });

            Currencies = mCurrencies;
        }

        private void ToggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            chartTimer.Start();
        }

        private void ToggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            chartTimer.Stop();
        }
    }

    public class Currency
    {
        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public Color Color { get; set; }

        public double CurrentTrend { get; set; } = 0d;

        public Series ChartSerie { get; set; }
    }
}

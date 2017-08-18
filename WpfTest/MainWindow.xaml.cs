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

namespace WpfTest
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        List<Currency> Currencies { get; set; }

        public Func<double, string> DateFormatter
        {
            get
            {
                return value => new DateTime((long)(value * TimeSpan.FromDays(1).Ticks)).ToString("d");
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            List<Currency> mCurrencies = new List<Currency>();

            mCurrencies.Add(new Currency() { Name = "EUR", Color = Colors.Blue });
            mCurrencies.Add(new Currency() { Name = "USD", Color = Colors.Green });
            mCurrencies.Add(new Currency() { Name = "BTC", Color = Colors.Orange });
            mCurrencies.Add(new Currency() { Name = "ETR", Color = Colors.LightBlue });

            Currencies = mCurrencies;
            var arrayCurrencies = mCurrencies.ToArray();

            this.currencyList.ItemsSource = mCurrencies;

            var Series = new SeriesCollection();
            var r = new Random();

            for (var i = 0; i < arrayCurrencies.Length; i++)
            {
                var trend = 0d;
                var values = new double[100];

                for (var j = 0; j < 100; j++)
                {
                    trend += (r.NextDouble() < .8 ? 1 : -1) * r.Next(-10, 10);
                    values[j] = trend;
                }

                var fillBr = new SolidColorBrush(arrayCurrencies[i].Color);
                fillBr.Opacity = 0.0f; // Transparent

                var series = new LineSeries
                {
                    Values = values.AsChartValues(),
                    Fill = fillBr,
                    StrokeThickness = .8,
                    Stroke = new SolidColorBrush(arrayCurrencies[i].Color),
                    PointGeometry = null
                };
                Series.Add(series);
            }

            currencyChart.Series = Series;

            // Build Echange rates combos
            ExchangeFrom.ItemsSource = Currencies;
            ExchangeTo.ItemsSource = Currencies;
        }
    }

    public class Currency
    {
        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public Color Color { get; set; }
    }
}

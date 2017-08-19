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
using static WpfTest.Currency;
using static WpfTest.Currency.CurrencySymbol;

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
            double currentMaxWidth = ExchangeCalcContainer.ActualWidth - FromUIRect.Margin.Left - 10d;

            updateExhangeRect(ExchangeFrom, FromUIRect, currentMaxWidth);
            updateExhangeRect(ExchangeTo, ToUIRect, currentMaxWidth);
        }

        private void updateExhangeRect(ComboBox combo, Rectangle previewRect, double currentMaxWidth)
        {
            double desiredWidth = 0d;
            double minWidth = 0d;

            double MinTrend = Currencies.OrderBy(cr => cr.CurrentTrend).FirstOrDefault().CurrentTrend;
            double MaxTrend = Currencies.OrderByDescending(cr => cr.CurrentTrend).FirstOrDefault().CurrentTrend;

            if (combo.SelectedItem != null)
            {
                minWidth = 2d;

                var item = (Currency)combo.SelectedItem;
                previewRect.Fill = ColorToBrushConverter.Convert(item.Color);

                desiredWidth = MathR.InverseLerp(MinTrend, MaxTrend, item.CurrentTrend); // Map to a 0 - 1 value
                desiredWidth *= currentMaxWidth;
            }

            previewRect.Width = MathR.Lerp(Math.Max(minWidth, previewRect.ActualWidth), desiredWidth, 0.2f);
        }

        private void ChartTimerTick(object sender, EventArgs e)
        {
            UpdateChart();
            UpdateEchangeConversion();
        }

        private void UpdateEchangeConversion()
        {
            if(ExchangeFrom.SelectedValue == null || ExchangeTo.SelectedValue == null || ExchangeDecimalTextBox.Value == null)
            {
                // Hide results
                if(GridExchangeResults != null)
                    GridExchangeResults.Visibility = Visibility.Hidden;

                return;
            }
            else
            {
                Currency cFrom  = (Currency) ExchangeFrom.SelectedValue;
                Currency cTo    = (Currency) ExchangeTo.SelectedValue;

                decimal quantity = (decimal)ExchangeDecimalTextBox.Value;

                double exchangeRate = cFrom.CurrentTrend / cTo.CurrentTrend;

                var finalExchangeRate = exchangeRate * (double)quantity;

                ToExResultName.Content = cFrom.Name;
                ToExResultName.Foreground = ColorToBrushConverter.Convert(cFrom.Color); 
                FromExResultName.Content = cTo.Name;
                FromExResultName.Foreground = ColorToBrushConverter.Convert(cTo.Color);
                ExRateResult.Content = exchangeRate.ToString();

                ExchangeResult.Content = String.Format("{0} = {1}", cFrom.Format((double)quantity), cTo.Format(finalExchangeRate));

                // Show results
                if (GridExchangeResults != null)
                    GridExchangeResults.Visibility = Visibility.Visible;
            }
        }

        private void UpdateChart()
        {
            var r = random;

            for (var i = 0; i < mArrayCurrencies.Length; i++)
            {
                var curr = mArrayCurrencies[i];

                curr.CurrentTrend += ((double)r.Next(-curr.Volatility, curr.Volatility)) + r.NextDouble();

                if (curr.CurrentTrend < 0d) // Never allow negative currency trend
                {
                    curr.CurrentTrend = ((double)r.Next(0, 5)) + r.NextDouble();
                }
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
                    curr.CurrentTrend += ((double)r.Next(-curr.Volatility, curr.Volatility)) + r.NextDouble();

                    if(curr.CurrentTrend < 0d) // Never allow negative currency trend
                    {
                        curr.CurrentTrend = ((double)r.Next(0, 5)) + r.NextDouble();
                    }

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

            AxisX.LabelFormatter = value =>
            {
                DateTime clone = InitDate.AddSeconds(value);
                return clone.ToString("HH:mm:ss");
            };
            AxisX.Separator = new LiveCharts.Wpf.Separator();
        }

        private void InitializeCurrencies()
        {
            List<Currency> mCurrencies = new List<Currency>();

            var symbolEUR = new CurrencySymbol() { Symbol = "€" };
            var symbolUSD = new CurrencySymbol() { Symbol = "$", Location = SymbolLocation.Left };
            var symbolBTC = new CurrencySymbol() { Symbol = "BTC" };
            var symbolETR = new CurrencySymbol() { Symbol = "ETR" };

            double trendEUR = ((double)random.Next(40, 60)) + random.NextDouble();
            double trendUSD = ((double)random.Next(40, 60)) + random.NextDouble();
            double trendBTC = ((double)random.Next(300, 400)) + random.NextDouble();
            double trendETR = ((double)random.Next(200, 300)) + random.NextDouble();

            mCurrencies.Add(new Currency() { Name = "EUR", Color = Colors.Blue, CurrentTrend = trendEUR, Volatility = 80, Symbol = symbolEUR });
            mCurrencies.Add(new Currency() { Name = "USD", Color = Colors.Green, CurrentTrend = trendUSD, Volatility = 70, Symbol = symbolUSD });
            mCurrencies.Add(new Currency() { Name = "BTC", Color = Colors.Orange, CurrentTrend = trendBTC, Volatility = 165, Symbol = symbolBTC });
            mCurrencies.Add(new Currency() { Name = "ETR", Color = Colors.LightBlue, CurrentTrend = trendETR, Volatility = 150, Symbol = symbolETR });

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

        private void CurrencyChart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AutoScrollCheckbox.IsChecked = false;
        }

        private void CurrencyChart_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            AutoScrollCheckbox.IsChecked = false;
        }

        private void AutoScrollCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            // Restore automatic zoom / autoScroll
            currencyChart.AxisX[0].MinValue = double.NaN;
            currencyChart.AxisX[0].MaxValue = double.NaN;
            currencyChart.AxisY[0].MinValue = double.NaN;
            currencyChart.AxisY[0].MaxValue = double.NaN;
        }

        private void AutoScrollCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Disable automatic zoom / autoScroll
            currencyChart.AxisX[0].MinValue = currencyChart.AxisX[0].ActualMinValue;
            currencyChart.AxisX[0].MaxValue = currencyChart.AxisX[0].ActualMaxValue;
            currencyChart.AxisY[0].MinValue = currencyChart.AxisY[0].ActualMinValue;
            currencyChart.AxisY[0].MaxValue = currencyChart.AxisY[0].ActualMaxValue;
        }

        private void UpdateCurrenciesEnabledState(object sender, RoutedEventArgs e)
        {
            foreach(var curr in Currencies)
            {
                curr.ChartSerie.Visibility = curr.Enabled ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void ExchangeComboSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateEchangeConversion();
        }

        private void ExchangeDecimalValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            UpdateEchangeConversion();
        }
    }

    public class Currency
    {
        public string Name { get; set; }

        public bool Enabled { get; set; } = true;

        public Color Color { get; set; }

        public double CurrentTrend { get; set; } = 0d;

        public int Volatility { get; set; } = 50;

        public Series ChartSerie { get; set; }

        public CurrencySymbol Symbol { get; set; }

        public string Format(double ammount)
        {
            if(Symbol.Location == SymbolLocation.Left)
            {
                return String.Format("{0} {1:0.00}", Symbol.Symbol, ammount);
            }
            else
            {
                return String.Format("{0:0.00} {1}", ammount, Symbol.Symbol);
            }
        }

        public class CurrencySymbol
        {
            public enum SymbolLocation
            {
                Left, Right
            }
            public String Symbol { get; set; }
            public SymbolLocation Location { get; set; } = SymbolLocation.Right;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace WpfTest.UserControls
{
    /// <summary>
    /// Lógica de interacción para ExchangeRateWatcher.xaml
    /// </summary>
    public partial class ExchangeRateWatcher : UserControl
    {
        public Currency BaseCurrency { get { return (Currency)GetValue(BaseCurrencyProperty); } set { SetValue(BaseCurrencyProperty, value); } }
        public Currency TargetCurrency { get { return (Currency)GetValue(TargetCurrencyProperty); } set { SetValue(TargetCurrencyProperty, value); } }

        public string BaseCurrencyName { get { return BaseCurrency == null ? "" : BaseCurrency.Name; } }
        public string TargetCurrencyName { get { return TargetCurrency == null ? "" : TargetCurrency.Name; } }

        protected double mExchangeRateValue = 0d;
        protected double mCurrentExchangeRateValue = 0d;

        protected const string ARROW_UP_UNICODE = "\u25B2";
        protected const string ARROW_DOWN_UNICODE = "\u25BC";

        public static Color GreenColor { get { return Colors.Green; } }
        public static Color OrangeColor { get { return Colors.Orange; } }
        public static Color RedColor { get { return Colors.Red; } }

        protected double currentTrendFactor = 0.5d;

        public ExchangeRateWatcher()
        {
            InitializeComponent();

            // Timer to do custom UI animations
            DispatcherTimer uiTimer = new DispatcherTimer(DispatcherPriority.Render);
            uiTimer.Interval = TimeSpan.FromSeconds(1d / 60d); // 60 times per second
            uiTimer.Tick += UiTimerTick;
            uiTimer.Start();
        }

        private void UiTimerTick(object sender, EventArgs e)
        {
            if(BaseCurrency == null || TargetCurrency == null) {
                return;
            }

            UpdateUI();
        }

        private void UpdateUI()
        {
            CurrenciesLabel.Content = String.Format("{0} / {1}", BaseCurrencyName, TargetCurrencyName);

            double exchangeRate = BaseCurrency.CurrentTrend / TargetCurrency.CurrentTrend;
            ValueLabel.Content = String.Format("{0:0.00}", exchangeRate);

            currentTrendFactor = MathR.InverseLerp(2d, 0d, exchangeRate);

            Color col;
            // interpolate between 3 colors
            if(currentTrendFactor >= 0.5d)
            {
                var factor = MathR.InverseLerp(0.5d, 1.0d, currentTrendFactor);
                col = MathR.Lerp(OrangeColor, RedColor, factor);

                ArrowTextBlock.Content = ARROW_DOWN_UNICODE;
            }
            else
            {
                var factor = MathR.InverseLerp(0d, 0.5d, currentTrendFactor);
                col = MathR.Lerp(GreenColor, OrangeColor, factor);

                ArrowTextBlock.Content = ARROW_UP_UNICODE;
            }

            var brush = ColorToBrushConverter.Convert(col);

            ArrowTextBlock.Foreground = brush;
            CurrenciesLabel.Foreground = brush;
            ValueLabel.Foreground = brush;
        }

        public static readonly DependencyProperty BaseCurrencyProperty =
            DependencyProperty.Register("BaseCurrency", typeof(Currency),
              typeof(ExchangeRateWatcher));

        public static readonly DependencyProperty TargetCurrencyProperty =
            DependencyProperty.Register("TargetCurrency", typeof(Currency),
              typeof(ExchangeRateWatcher));
    }
}

using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static WpfTest.Currency.CurrencySymbol;

namespace WpfTest
{
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
            if (Symbol.Location == SymbolLocation.Left)
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

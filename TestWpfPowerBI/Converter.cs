using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace TestWpfPowerBI
{
    public class RelativeWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values[0] == System.Windows.DependencyProperty.UnsetValue) return 0.0;
            return ((Double)values[0] * (Double)values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MaxMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ((Double)values[0] > (Double)values[1]) return values[0];
            return values[1];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DataBindingDebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }

        public object ConvertBack(object value, Type targetType,
            object parameter, CultureInfo culture)
        {
            Debugger.Break();
            return value;
        }
    }

    class MinusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double.TryParse((string)parameter, out double dblParam);
            return (double)value - (double)dblParam;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    public class GreaterThanConverter : MarkupExtension, IValueConverter
    {
        //  The only public constructor is one that requires a double argument.
        //  Because of that, the XAML editor will put a blue squiggly on it if 
        //  the argument is missing in the XAML. 
        public GreaterThanConverter(int opnd)
        {
            Operand = opnd;
        }

        /// <summary>
        /// Converter returns true if value is greater than this.
        /// 
        /// Don't let this be public, because it's required to be initialized 
        /// via the constructor. 
        /// </summary>
        protected int Operand { get; set; }

        //  When the XAML is parsed, each markup extension is instantiated 
        //  and the parser asks it to provide its value. Here, the value is 
        //  us. 
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int.TryParse(value.ToString(), out int i);
            return i > Operand;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

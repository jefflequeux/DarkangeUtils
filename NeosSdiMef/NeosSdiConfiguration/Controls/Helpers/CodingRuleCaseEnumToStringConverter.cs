using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace NeosSdiConfiguration.Controls.Helpers
{
    public class CodingRuleCaseEnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (CodingRuleCaseEnum)Enum.Parse(typeof(CodingRuleCaseEnum), value.ToString(), true);
        }
    }

}

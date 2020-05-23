using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Maui.Presentation.WPF
{
	internal class MauiToWPFDimensionConverter : IValueConverter
	{
		private MauiToWPFDimensionConverter()
		{
		}

		internal static MauiToWPFDimensionConverter Instance { get; } = new MauiToWPFDimensionConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is double d) || d == -1)
				return double.NaN;
			return d;		
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

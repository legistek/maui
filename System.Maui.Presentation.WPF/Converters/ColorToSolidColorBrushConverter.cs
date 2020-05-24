using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Maui.Presentation.WPF
{
	internal class ColorToSolidColorBrushConverter : IValueConverter
	{
		internal static IValueConverter Instance { get; } = new ColorToSolidColorBrushConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is System.Maui.Color c))
				return null;
			return new SolidColorBrush(System.Windows.Media.Color.FromArgb(
				(byte)(c.A * 255),
				(byte)(c.R * 255),
				(byte)(c.G * 255),
				(byte)(c.B * 255)));
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

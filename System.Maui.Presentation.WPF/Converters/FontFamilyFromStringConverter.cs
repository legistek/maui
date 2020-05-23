using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace System.Maui.Presentation.WPF
{
	internal class FontFamilyFromStringConverter : IValueConverter
	{
		private FontFamilyFromStringConverter()
		{
		}

		internal static IValueConverter Instance { get; } = new FontFamilyFromStringConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is string fontName))
				return null;
			return new FontFamily(fontName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is FontFamily fam))
				return null;
			return fam.FamilyNames.First();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Maui.Presentation.WPF
{
	class ThicknessConverter : IValueConverter
	{
		private ThicknessConverter()
		{
		}

		internal static ThicknessConverter Instance { get; } = new ThicknessConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is System.Maui.Thickness t))
				return null;
			return new System.Windows.Thickness(t.Left, t.Top, t.Right, t.Bottom);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

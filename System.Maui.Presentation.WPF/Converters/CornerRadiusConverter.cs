using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Maui.Presentation.WPF
{
	internal class CornerRadiusConverter : IValueConverter
	{
		private CornerRadiusConverter()
		{
		}

		internal static CornerRadiusConverter Instance { get; } = new CornerRadiusConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is System.Maui.CornerRadius cr))
				return null;
			return new System.Windows.CornerRadius(
				cr.TopLeft, 
				cr.TopRight, 
				cr.BottomRight, 
				cr.BottomLeft);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

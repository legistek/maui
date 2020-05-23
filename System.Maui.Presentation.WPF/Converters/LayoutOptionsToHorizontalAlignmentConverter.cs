using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace System.Maui.Presentation.WPF
{
	internal class LayoutOptionsToHorizontalAlignmentConverter : IValueConverter
	{
		private LayoutOptionsToHorizontalAlignmentConverter()
		{
		}

		internal static LayoutOptionsToHorizontalAlignmentConverter Instance { get; } = 
			new LayoutOptionsToHorizontalAlignmentConverter();

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is LayoutOptions l))
				return null;

			switch (l.Alignment)
			{
				case LayoutAlignment.Start:
					return HorizontalAlignment.Left;
				case LayoutAlignment.Center:
					return HorizontalAlignment.Center;
				case LayoutAlignment.End:
					return HorizontalAlignment.Right;
				case LayoutAlignment.Fill:
					return HorizontalAlignment.Stretch;
				default:
					return HorizontalAlignment.Stretch;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}

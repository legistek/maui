using System;
using System.Collections.Generic;
using System.Text;

namespace System.Maui.Presentation.Controls
{
	public class TextBlock : FrameworkElement
	{
		#region string FontFamily dependency property
		public static BindableProperty FontFamilyProperty = BindableProperty.Create(
			"FontFamily",
			typeof(string),
			typeof(TextBlock),
			"Times New Roman");
		public string FontFamily
		{
			get
			{
				return (string)GetValue(FontFamilyProperty);
			}
			set
			{
				SetValue(FontFamilyProperty, value);
			}
		}
		public static string GetFontFamily(BindableObject bo)
		{
			return (string)bo.GetValue(FontFamilyProperty);
		}
		public static void SetFontFamily(BindableObject bo, string value)
		{
			bo.SetValue(FontFamilyProperty, value);
		}
		#endregion

		#region double FontSize dependency property
		public static BindableProperty FontSizeProperty = BindableProperty.Create(
			"FontSize",
			typeof(double),
			typeof(TextBlock),
			12.0d);
		public double FontSize
		{
			get
			{
				return (double)GetValue(FontSizeProperty);
			}
			set
			{
				SetValue(FontSizeProperty, value);
			}
		}

		public static double GetFontSize(BindableObject bo)
		{
			return (double)bo.GetValue(FontSizeProperty);
		}
		public static void SetFontSize(BindableObject bo, double value)
		{
			bo.SetValue(FontSizeProperty, value);
		}
		#endregion

		#region string Text dependency property
		public static BindableProperty TextProperty = BindableProperty.Create(
			"Text",
			typeof(string),
			typeof(TextBlock),
			null);
		public string Text
		{
			get
			{
				return (string)GetValue(TextProperty);
			}
			set
			{
				SetValue(TextProperty, value);
			}
		}
		#endregion

		#region Color Foreground dependency property
		public static BindableProperty ForegroundProperty = BindableProperty.Create(
			"Foreground",
			typeof(Color),
			typeof(TextBlock),
			Color.Black);
		public Color Foreground
		{
			get
			{
				return (Color)GetValue(ForegroundProperty);
			}
			set
			{
				SetValue(ForegroundProperty, value);
			}
		}
		#endregion

		protected override SizeRequest MeasureOverride(double widthConstraint, double heightConstraint)
		{
			var sz = Device.PlatformServices.GetNativeSize(this, widthConstraint, heightConstraint);
			return sz;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace System.Maui.Presentation.Controls
{
	public class Border : Decorator
	{
		#region Color BorderBrush dependency property
		public static BindableProperty BorderBrushProperty = BindableProperty.Create(
			nameof(BorderBrush),
			typeof(Color),
			typeof(Border),
			null);
		public Color BorderBrush
		{
			get
			{
				return (Color)GetValue(BorderBrushProperty);
			}
			set
			{
				SetValue(BorderBrushProperty, value);
			}
		}
		#endregion

		#region Thickness BorderThickness dependency property
		public static BindableProperty BorderThicknessProperty = BindableProperty.Create(
			nameof(BorderThickness),
			typeof(Thickness),
			typeof(Border),
			default(Thickness));
		public Thickness BorderThickness
		{
			get
			{
				return (Thickness)GetValue(BorderThicknessProperty);
			}
			set
			{
				SetValue(BorderThicknessProperty, value);
			}
		}
		#endregion

		#region CornerRadius CornerRadius dependency property
		public static readonly BindableProperty CornerRadiusProperty = BindableProperty.Create(
			nameof(CornerRadius),
			typeof(CornerRadius),
			typeof(Border),
			default(CornerRadius));
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)GetValue(CornerRadiusProperty);
			}
			set
			{
				SetValue(CornerRadiusProperty, value);
			}
		}
		#endregion

		protected override SizeRequest MeasureOverride(double widthConstraint, double heightConstraint)
		{
			var baseSize = base.MeasureOverride(widthConstraint, heightConstraint).Request;
			return new SizeRequest(new Size(baseSize.Width + this.BorderThickness.HorizontalThickness,
					baseSize.Height + this.BorderThickness.VerticalThickness));
		}
	}
}
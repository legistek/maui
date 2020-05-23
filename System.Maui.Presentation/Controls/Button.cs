using System;
using System.Collections.Generic;
using System.Text;

using System.Maui.Presentation.Controls.Primitives;

namespace System.Maui.Presentation.Controls
{
    public class Button : ButtonBase
    {
		#region Color ActionColor dependency property
		public static BindableProperty ActionColorProperty = BindableProperty.Create(
			"ActionColor",
			typeof(Color),
			typeof(Button),
			null);
		public Color ActionColor
		{
			get
			{
				return (Color)GetValue(ActionColorProperty);
			}
			set
			{
				SetValue(ActionColorProperty, value);
			}
		}
		#endregion
	}
}

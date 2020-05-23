using System;
using System.Collections.Generic;
using System.Maui.Platform.WPF;
using System.Text;
using System.Windows;

namespace System.Maui.Presentation.WPF
{
	public static class Platform
	{
		internal static readonly DependencyProperty RendererDProperty = DependencyProperty.RegisterAttached(
			"Renderer",
			typeof(IVisualElementRenderer),
			typeof(Platform),
			null);

		public static void Initialize()
		{
			Framework.Initialize();
		}
	}
}

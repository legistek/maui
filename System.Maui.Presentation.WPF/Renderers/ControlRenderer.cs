using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using XControl = System.Maui.Presentation.Controls.Control;
using System.Maui.Platform.WPF;
using System.Maui.Presentation.Controls;

[assembly: System.Maui.Platform.WPF.ExportRenderer(
	typeof(XControl),
	typeof(System.Maui.Presentation.WPF.ControlRenderer))]

namespace System.Maui.Presentation.WPF
{
	public class ControlRenderer : PlatformRenderer<XControl, System.Windows.Controls.Grid>
	{
		static ControlRenderer()
		{
		}

		protected override void OnElementChanged(ElementChangedEventArgs<XControl> e)
		{
			base.OnElementChanged(e);
			var child = this.Element.Children.FirstOrDefault() as VisualElement;
			if (child != null)
			{
				var childRenderer = System.Maui.Platform.WPF.Platform.CreateRenderer(child);
				System.Maui.Platform.WPF.Platform.SetRenderer(child, childRenderer);
				this.Control.Children.Add(childRenderer.GetNativeElement());
			}
		}

		protected override void Appearing()
		{
			base.Appearing();
			Element.Layout(new Rectangle(
				0,
				0,
				Control.ActualWidth,
				Control.ActualHeight));
		}
	}
}

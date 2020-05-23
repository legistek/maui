using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using WBorder = System.Windows.Controls.Border;
using XBorder = System.Maui.Presentation.Controls.Border;
using System.Maui.Platform.WPF;
using System.Maui.Presentation.Controls;

[assembly: System.Maui.Platform.WPF.ExportRenderer(
	typeof(XBorder),
	typeof(System.Maui.Presentation.WPF.BorderRenderer))]

namespace System.Maui.Presentation.WPF
{
	public class BorderRenderer : PlatformRenderer<XBorder, WBorder>
	{
		static BorderRenderer()
		{
			BindProperty(nameof(XBorder.BorderBrush), WBorder.BorderBrushProperty, ColorToSolidColorBrushConverter.Instance);
			BindProperty(nameof(XBorder.CornerRadius), WBorder.CornerRadiusProperty, CornerRadiusConverter.Instance);
			BindProperty(nameof(XBorder.BorderThickness), WBorder.BorderThicknessProperty, ThicknessConverter.Instance);
			BindProperty(nameof(XBorder.BackgroundColor), WBorder.BackgroundProperty, ColorToSolidColorBrushConverter.Instance);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<XBorder> e)
		{
			base.OnElementChanged(e);
			if ( this.Element.Child != null )
			{
				var childRenderer = System.Maui.Platform.WPF.Platform.CreateRenderer(this.Element.Child);
				System.Maui.Platform.WPF.Platform.SetRenderer(this.Element.Child, childRenderer);
				this.Control.Child = childRenderer.GetNativeElement();				
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
using System;
using System.Maui.Platform.WPF;
using XTextBlock = System.Maui.Presentation.Controls.TextBlock;
using WTextBlock = System.Windows.Controls.TextBlock;

[assembly: ExportRenderer(
	typeof(XTextBlock),
	typeof(System.Maui.Presentation.WPF.TextBlockRenderer))]

namespace System.Maui.Presentation.WPF
{	
	public class TextBlockRenderer : PlatformRenderer<XTextBlock, WTextBlock>
	{
		static TextBlockRenderer()
		{
			BindProperty(nameof(XTextBlock.Text), WTextBlock.TextProperty);
			BindProperty(nameof(XTextBlock.FontFamily), WTextBlock.FontFamilyProperty, FontFamilyFromStringConverter.Instance);
			BindProperty(nameof(XTextBlock.Foreground), WTextBlock.ForegroundProperty, ColorToSolidColorBrushConverter.Instance);
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
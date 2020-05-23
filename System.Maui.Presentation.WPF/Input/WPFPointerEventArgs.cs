using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Maui.Presentation.Input;

namespace System.Maui.Presentation.WPF
{
	internal class WPFPointerEventArgs : PointerEventArgs
	{
		MouseEventArgs _e;

		internal static WPFPointerEventArgs ToFrameworkArgs(			
			System.Windows.RoutedEventArgs e,
			RoutedEvent re)
		{
			return new WPFPointerEventArgs(e as MouseEventArgs)
			{
				RoutedEvent = re
			};
		}

		private WPFPointerEventArgs(MouseEventArgs e)
		{
			_e = e;
		}

		public override Point GetPoint(View relativeTo)
		{
			System.Windows.UIElement nativeElement = null;
			Element xformsElement = relativeTo;
			while (relativeTo != null && nativeElement == null)
			{
				var renderer = System.Maui.Platform.WPF.Platform.GetRenderer(relativeTo);
				nativeElement = renderer.GetNativeElement();										
				if (nativeElement == null)
					xformsElement = xformsElement.Parent;
			}

			if (nativeElement != null)
			{
				var pt = _e.GetPosition(nativeElement);
				return new Point(pt.X, pt.Y);
			}

			return default(Point);
		}

		public override void OnEventHandled()
		{
			base.OnEventHandled();
			_e.Handled = true;
			if ( this.PointerCaptureRequested)
				Mouse.Capture(_e.Source as IInputElement);
			else if (this.PointerReleaseRequested)
				(_e.Source as IInputElement)?.ReleaseMouseCapture();
		}
	}
}

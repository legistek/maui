using System;
using System.Collections.Generic;
using System.Text;

namespace System.Maui.Presentation.Input
{
	public class PointerEventArgs : RoutedEventArgs
	{
		public bool PointerReleaseRequested { get; set; }

		public bool PointerCaptureRequested { get; set; }

		public virtual Point GetPoint(View relativeTo)
		{
			return default;
		}
	}
}

using System;
using System.Collections.Generic;
using System.Text;

namespace System.Maui.Presentation.Controls
{
    public class VirtualizingPanel : Panel
    {
		/// <summary>
		///     Attached property for use on the ItemsControl that is the host for the items being
		///     presented by this panel. Use this property to turn virtualization on/off when grouping.
		/// </summary>
		public static readonly BindableProperty IsVirtualizingWhenGroupingProperty =
			BindableProperty.CreateAttached(
				"IsVirtualizingWhenGrouping",
				typeof(bool),
				typeof(VirtualizingPanel),
				false);

		/// <summary>
		///     Retrieves the value for <see cref="IsVirtualizingWhenGroupingProperty" />.
		/// </summary>
		/// <param name="element">The object on which to query the value.</param>
		/// <returns>True if virtualizing, false otherwise.</returns>
		public static bool GetIsVirtualizingWhenGrouping(BindableObject element)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}

			return (bool)element.GetValue(IsVirtualizingWhenGroupingProperty);
		}

		/// <summary>
		///     Sets the value for <see cref="IsVirtualizingWhenGroupingProperty" />.
		/// </summary>
		/// <param name="element">The element on which to set the value.</param>
		/// <param name="value">True if virtualizing, false otherwise.</param>
		public static void SetIsVirtualizingWhenGrouping(BindableObject element, bool value)
		{
			if (element == null)
			{
				throw new ArgumentNullException("element");
			}

			element.SetValue(IsVirtualizingWhenGroupingProperty, value);
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			throw new NotImplementedException();
		}
	}
}

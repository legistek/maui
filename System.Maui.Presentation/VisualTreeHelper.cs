using System;
using System.Collections.Generic;
using System.Text;
using System.Maui.Presentation.Internals;

namespace System.Maui.Presentation
{
	public static class VisualTreeHelper
	{
		internal static ITemplateParent TemplatedParent(this Layout layout)
		{
			return VisualTreeHelper.FindParent(
					layout,
					elem =>
						elem is ITemplateParent) as ITemplateParent;
		}

		public static Element FindChildElement(Layout layout, Func<Element, bool> criteria)
		{
			foreach (var child in layout.Children)
			{
				if (criteria(child))
					return child;
				else if (child is Layout l && l.Children.Count > 0)
				{
					Element hit = FindChildElement(l, criteria);
					if (hit != null)
						return hit;
				}
			}
			return null;
		}

		public static Element FindParent(Element elem, Func<Element, bool> criteria)
		{
			var parent = elem.RealParent;
			while (parent != null)
			{
				if (criteria(parent))
					return parent;
				parent = parent.RealParent;
			}
			return null;
		}

		public static VisualElement GetParent(VisualElement elem)
		{
			return elem.Parent as VisualElement;
		}

		internal static BindableObject GetParentInternal(BindableObject elem)
		{
			return (elem as Element)?.RealParent;
		}
	}
}

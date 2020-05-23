using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace System.Maui.Presentation.Controls
{
	[ContentProperty(nameof(Child))]
	public class Decorator : FrameworkElement
	{
		public View Child
		{
			get
			{
				return this.Children.FirstOrDefault() as View;
			}
			set
			{
				this.InternalChildren.Clear();
				this.InternalChildren.Add(value);
				(this.Parent as Layout)?.ForceLayout();
			}
		}

		protected override void LayoutChildren(
			double x,
			double y,
			double width,
			double height)
		{
			base.LayoutChildren(x, y, width, height);
			foreach (var child in Children)
				LayoutChildIntoBoundingRegion(
					child as VisualElement,
					new Rectangle(x, y, width, height));
		}

		protected override SizeRequest MeasureOverride(double widthConstraint, double heightConstraint)
		{
			if (this.Child != null)
				return this.Child.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
			return base.OnMeasure(widthConstraint, heightConstraint);
		}
	}
}
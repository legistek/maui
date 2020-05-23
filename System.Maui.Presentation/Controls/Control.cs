using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.Windows.Input;
using System.Maui.Presentation.Input;
using System.Maui.Presentation.Internals;

namespace System.Maui.Presentation.Controls
{
	public class Control : FrameworkElement, IControlTemplated, ITemplateParent
	{
		#region ControlTemplate Template property
		public static BindableProperty TemplateProperty = BindableProperty.Create(
			"Template",
			typeof(ControlTemplate),
			typeof(Control),
			null);
		public ControlTemplate Template
		{
			get
			{
				return (ControlTemplate)GetValue(TemplateProperty);
			}
			set
			{
				SetValue(TemplateProperty, value);
			}
		}
		#endregion

		#region double FontSize property
		public static BindableProperty FontSizeProperty = BindableProperty.Create(
			nameof(FontSize),
			typeof(double),
			typeof(Control),
			12.0);
		public double FontSize
		{
			get
			{
				return (double)GetValue(FontSizeProperty);
			}
			set
			{
				SetValue(FontSizeProperty, value);
			}
		}
		#endregion

		#region string FontFamily property
		public static BindableProperty FontFamilyProperty = BindableProperty.Create(
			nameof(FontFamily),
			typeof(string),
			typeof(Control),
			null);
		public string FontFamily
		{
			get
			{
				return (string)GetValue(FontFamilyProperty);
			}
			set
			{
				SetValue(FontFamilyProperty, value);
			}
		}
		#endregion

		#region Color Background property
		public static BindableProperty BackgroundProperty = BindableProperty.Create(
			"Background",
			typeof(Color),
			typeof(Control),
			null,
			propertyChanged: OnBackgroundChanged);
		public Color Background
		{
			get
			{
				return (Color)GetValue(BackgroundProperty);
			}
			set
			{
				SetValue(BackgroundProperty, value);
			}
		}
		private static void OnBackgroundChanged(BindableObject obj, object oldValue, object newValue)
		{
			Debug.WriteLine($"Control background changed to {(newValue as Color?)?.ToString()}");
		}
		#endregion

		#region Color Foreground property
		public static BindableProperty ForegroundProperty = BindableProperty.Create(
			"Foreground",
			typeof(Color),
			typeof(Control),
			null);
		public Color Foreground
		{
			get
			{
				return (Color)GetValue(ForegroundProperty);
			}
			set
			{
				SetValue(ForegroundProperty, value);
			}
		}
		#endregion

		#region HorizontalAlignment HorizontalContentAlignment property
		public static BindableProperty HorizontalContentAlignmentProperty = BindableProperty.Create(
			"HorizontalContentAlignment",
			typeof(LayoutAlignment),
			typeof(Control),
			LayoutAlignment.Fill);
		public LayoutAlignment HorizontalContentAlignment
		{
			get
			{
				return (LayoutAlignment)GetValue(HorizontalContentAlignmentProperty);
			}
			set
			{
				SetValue(HorizontalContentAlignmentProperty, value);
			}
		}
		#endregion

		#region VerticalAlignment VerticalContentAlignment property
		public static BindableProperty VerticalContentAlignmentProperty = BindableProperty.Create(
			"VerticalContentAlignment",
			typeof(LayoutAlignment),
			typeof(Control),
			LayoutAlignment.Fill);
		public LayoutAlignment VerticalContentAlignment
		{
			get
			{
				return (LayoutAlignment)GetValue(VerticalContentAlignmentProperty);
			}
			set
			{
				SetValue(VerticalContentAlignmentProperty, value);
			}
		}
		#endregion

		#region Color BorderBrush property
		public static BindableProperty BorderBrushProperty = BindableProperty.Create(
			"BorderBrush",
			typeof(Color),
			typeof(Control),
			null);
		public Color BorderBrush
		{
			get
			{
				return (Color)GetValue(BorderBrushProperty);
			}
			set
			{
				SetValue(BorderBrushProperty, value);
			}
		}
		#endregion

		#region Thickness BorderThickness property
		public static BindableProperty BorderThicknessProperty = BindableProperty.Create(
			"BorderThickness",
			typeof(Thickness),
			typeof(Control),
			default(Thickness));
		public Thickness BorderThickness
		{
			get
			{
				return (Thickness)GetValue(BorderThicknessProperty);
			}
			set
			{
				SetValue(BorderThicknessProperty, value);
			}
		}
		#endregion

		#region CornerRadius CornerRadius property
		public static BindableProperty CornerRadiusProperty = BindableProperty.Create(
			"CornerRadius",
			typeof(CornerRadius),
			typeof(Control),
			default(CornerRadius));
		public CornerRadius CornerRadius
		{
			get
			{
				return (CornerRadius)GetValue(CornerRadiusProperty);
			}
			set
			{
				SetValue(CornerRadiusProperty, value);
			}
		}
		#endregion

		#region bool IsPointerOver property
		public static BindableProperty IsPointerOverProperty = BindableProperty.Create(
			"IsPointerOver",
			typeof(bool),
			typeof(Control),
			false);
		public bool IsPointerOver
		{
			get
			{
				return (bool)GetValue(IsPointerOverProperty);
			}
			set
			{
				SetValue(IsPointerOverProperty, value);
			}
		}
		#endregion

		#region bool IsPointerDown property
		public static BindableProperty IsPointerDownProperty = BindableProperty.Create(
			"IsPointerDown",
			typeof(bool),
			typeof(Control),
			false);
		public bool IsPointerDown
		{
			get
			{
				return (bool)GetValue(IsPointerDownProperty);
			}
			set
			{
				SetValue(IsPointerDownProperty, value);
			}
		}
		#endregion

		#region ICommand PointerPressedCommand property
		public static BindableProperty PointerPressedCommandProperty = BindableProperty.Create(
			"PointerPressedCommand",
			typeof(ICommand),
			typeof(Control),
			null);
		public ICommand PointerPressedCommand
		{
			get
			{
				return (ICommand)GetValue(PointerPressedCommandProperty);
			}
			set
			{
				SetValue(PointerPressedCommandProperty, value);
			}
		}
		#endregion

		#region ICommand TappedCommand property
		public static BindableProperty TappedCommandProperty = BindableProperty.Create(
			"TappedCommand",
			typeof(ICommand),
			typeof(Control),
			null);
		public ICommand TappedCommand
		{
			get
			{
				return (ICommand)GetValue(TappedCommandProperty);
			}
			set
			{
				SetValue(TappedCommandProperty, value);
			}
		}
		#endregion

		#region ICommand DoubleTappedCommand property
		public static BindableProperty DoubleTappedCommandProperty = BindableProperty.Create(
			"DoubleTappedCommand",
			typeof(ICommand),
			typeof(Control),
			null);
		public ICommand DoubleTappedCommand
		{
			get
			{
				return (ICommand)GetValue(DoubleTappedCommandProperty);
			}
			set
			{
				SetValue(DoubleTappedCommandProperty, value);
			}
		}
		#endregion

		#region ICommand PointerReleasedCommand property
		public static BindableProperty PointerReleasedCommandProperty = BindableProperty.Create(
			"PointerReleasedCommand",
			typeof(ICommand),
			typeof(Control),
			null);
		public ICommand PointerReleasedCommand
		{
			get
			{
				return (ICommand)GetValue(PointerReleasedCommandProperty);
			}
			set
			{
				SetValue(PointerReleasedCommandProperty, value);
			}
		}
		#endregion

		public virtual void OnTapped(object sender, PointerEventArgs e)
		{
			this.TappedCommand?.Execute(e);
		}

		public virtual void OnDoubleTapped(object sender, PointerEventArgs e)
		{
			this.DoubleTappedCommand?.Execute(e);
		}

		public override void OnPointerPressed(PointerEventArgs e)
		{
			this.IsPointerDown = true;
			this.PointerPressedCommand?.Execute(e);
		}

		public override void OnPointerReleased(PointerEventArgs e)
		{
			this.IsPointerDown = false;
			this.PointerReleasedCommand?.Execute(e);
		}

		public override void OnPointerEntered(PointerEventArgs e)
		{
			this.IsPointerOver = true;
			this.PointerReleasedCommand?.Execute(e);
		}

		public override void OnPointerExited(PointerEventArgs e)
		{
			this.IsPointerOver = false;
			this.PointerReleasedCommand?.Execute(e);
		}

		protected override void OnParentSet()
		{
			base.OnParentSet();
			ApplyTemplate();
		}

		protected override void OnApplyTemplate()
		{
		}

		protected override SizeRequest MeasureOverride(double widthConstraint, double heightConstraint)
		{
			double widthRequest = WidthRequest;
			double heightRequest = HeightRequest;
			var childRequest = new SizeRequest();

			if ((widthRequest == -1 || heightRequest == -1) && InternalChild != null)
			{
				childRequest = InternalChild.Measure(widthConstraint, heightConstraint, MeasureFlags.IncludeMargins);
			}

			return new SizeRequest
			{
				Request = new Size
				{ 
					Width = widthRequest != -1 ? widthRequest : childRequest.Request.Width, 
					Height = heightRequest != -1 ? heightRequest : childRequest.Request.Height 
				},
				Minimum = childRequest.Minimum
			};
		}

		internal View InternalChild => this.Children?.FirstOrDefault() as View;

		private static void ApplyTemplateToChildren(IEnumerable<Element> children)
		{
			if (children == null)
				return;
			foreach (var child in children)
			{
				if (child is Control fe)
					fe.ApplyTemplate();
				else if (child is Layout l && l.Children?.Count > 0)
					ApplyTemplateToChildren(l.Children);
			}
		}

		#region IControlTemplated Implementation

		ControlTemplate IControlTemplated.ControlTemplate
		{
			get => this.Template;
			set => this.Template = value;
		}

		void IControlTemplated.OnApplyTemplate()
		{
			OnApplyTemplate();
			ApplyTemplateToChildren(this.Children);
		}

		IList<Element> IControlTemplated.InternalChildren => InternalChildren;

		Element IControlTemplated.TemplateRoot { get; set; }

		void IControlTemplated.OnControlTemplateChanged(
			ControlTemplate oldValue,
			ControlTemplate newValue)
		{
		}

		#endregion
	}
}
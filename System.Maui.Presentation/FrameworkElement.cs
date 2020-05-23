using System;
using System.Collections.Generic;
using System.Maui.Markup;
using System.Maui.Presentation.Input;
using System.Maui.Presentation.Internals;
using System.Windows;

namespace System.Maui.Presentation
{
	public abstract class FrameworkElement : Layout
	{
		internal delegate void PointerEventHandler(object sender, PointerEventArgs e);

		static FrameworkElement()
		{
			EventManager.RegisterClassHandler(
				typeof(FrameworkElement),
				PointerPressedEvent,
				new PointerEventHandler((sender, e) =>
					(sender as FrameworkElement)?.OnPointerPressed(e as PointerEventArgs)));
			EventManager.RegisterClassHandler(
				typeof(FrameworkElement),
				PointerReleasedEvent,
				new PointerEventHandler((sender, e) =>
					(sender as FrameworkElement)?.OnPointerReleased(e as PointerEventArgs)));
			EventManager.RegisterClassHandler(
				typeof(FrameworkElement),
				PointerEnteredEvent,
				new PointerEventHandler((sender, e) =>
					(sender as FrameworkElement)?.OnPointerEntered(e as PointerEventArgs)));
			EventManager.RegisterClassHandler(
				typeof(FrameworkElement),
				PointerExitedEvent,
				new PointerEventHandler((sender, e) =>
					(sender as FrameworkElement)?.OnPointerExited(e as PointerEventArgs)));
		}

		public FrameworkElement()
		{			
		}

		public static readonly RoutedEvent PointerPressedEvent = EventManager.RegisterRoutedEvent(
			"PointerPressed",
			RoutingStrategy.Direct,
			typeof(PointerEventHandler),
			typeof(FrameworkElement));
		public static readonly RoutedEvent PointerReleasedEvent = EventManager.RegisterRoutedEvent(
			"PointerReleased",
			RoutingStrategy.Direct,
			typeof(PointerEventHandler),
			typeof(FrameworkElement));
		public static readonly RoutedEvent PointerEnteredEvent = EventManager.RegisterRoutedEvent(
			"PointerEntered",
			RoutingStrategy.Direct,
			typeof(PointerEventHandler),
			typeof(FrameworkElement));
		public static readonly RoutedEvent PointerExitedEvent = EventManager.RegisterRoutedEvent(
			"PointerExited",
			RoutingStrategy.Direct,
			typeof(PointerEventHandler),
			typeof(FrameworkElement));

		public bool IsInitialized { get; internal set; }

		public Size DesiredSize { get; private set; }

		#region object DataContext dependency property		
		public object DataContext
		{
			get
			{
				return (object)GetValue(BindingContextProperty);
			}
			set
			{
				SetValue(BindingContextProperty, value);
			}
		}
		#endregion

		DependencyObjectType _dType;
		/// <summary>Returns the DType that represents the CLR type of this instance</summary>
		public DependencyObjectType DependencyObjectType
		{
			get
			{
				if (_dType == null)
				{
					// Specialized type identification
					_dType = DependencyObjectType.FromSystemTypeInternal(GetType());
				}

				// Do not call VerifyAccess because this method is trivial.
				return _dType;
			}
		}

		public void RaiseEvent(RoutedEventArgs e)
		{
			var list = GlobalEventManager.GetDTypedClassListeners(
				this.DependencyObjectType,
				e.RoutedEvent);
			if (list == null)
				return;
			foreach (var handler in list.Handlers)
			{
				handler.InvokeHandler(this, e);
			}
			if (e.Handled)
				e.OnEventHandled();
		}

		public void ApplyTemplate()
		{
			OnApplyTemplate();
		}

		public virtual void OnPointerPressed(PointerEventArgs e)
		{
		}

		public virtual void OnPointerReleased(PointerEventArgs e)
		{
		}

		public virtual void OnPointerEntered(PointerEventArgs e)
		{
		}

		public virtual void OnPointerExited(PointerEventArgs e)
		{
		}

		protected virtual void OnApplyTemplate()
		{
		}

		protected override void OnParentSet()
		{
			base.OnParentSet();
			this.IsInitialized = true;
			OnPreApplyTemplate();
			ApplyTemplate();
			OnAncestorChangedPrivate();
		}

		protected override sealed SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
		{
			var sz = this.MeasureOverride(widthConstraint, heightConstraint);
			this.DesiredSize = sz.Request;
			return sz;
		}

		protected virtual SizeRequest MeasureOverride(double widthConstraint, double heightConstraint)
		{
			return base.OnMeasure(widthConstraint, heightConstraint);
		}

		/// <summary>
		/// This virtual is called by FE.ApplyTemplate before it does work to generate the template tree.
		/// </summary>
		/// <remarks>
		/// This virtual is overridden for the following three reasons
		/// 1. By ContentPresenter/ItemsPresenter to choose the template to be applied in this case.
		/// 2. By RowPresenter/ColumnHeaderPresenter/InkCanvas to build custom visual trees
		/// 3. By ScrollViewer/TickBar/ToolBarPanel/Track to hookup bindings to their TemplateParent
		/// </remarks>
		internal virtual void OnPreApplyTemplate()
		{
		}

		internal virtual void OnAncestorChanged()
		{
		}

		internal bool HasNonDefaultValue(BindableProperty prop)
		{
			return this.GetValue(prop) != prop.DefaultValue;
		}

		ITemplateParent _templatedParent;
		internal ITemplateParent TemplatedParent
		{
			get => _templatedParent ?? (_templatedParent =
				VisualTreeHelper.FindParent(
					this,
					elem =>
						elem is ITemplateParent) as ITemplateParent);
		}

		protected object GetTemplateChild(string name) =>
			this is IControlTemplated c ? TemplateUtilities.GetTemplateChild(c, name) : null;

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			for (var i = 0; i < LogicalChildrenInternal.Count; i++)
			{
				Element element = LogicalChildrenInternal[i];
				var child = element as View;
				if (child != null)
					LayoutChildIntoBoundingRegion(child, new Rectangle(x, y, width, height));
			}
		}

		private void OnAncestorChangedPrivate()
		{
			_templatedParent = null;
			OnAncestorChanged();
			if (this.Children == null)
				return;
			foreach (var child in this.Children)
			{
				if (child is FrameworkElement fe)
					fe.OnAncestorChangedPrivate();
			}
		}
	}
}

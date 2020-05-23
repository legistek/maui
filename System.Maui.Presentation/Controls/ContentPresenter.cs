using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Maui;
using System.Maui.Presentation.Internals;
using System.Maui.Presentation.MS.Internal;

using DependencyObject = System.Maui.BindableObject;

namespace System.Maui.Presentation.Controls
{
	public class ContentPresenter : FrameworkElement, ITemplateParent
	{
		public ContentPresenter() : base()
		{
		}

		#region ContentStringFormat Property

		/// <summary>
		///     The DependencyProperty for the ContentStringFormat property.
		///     Flags:              None
		///     Default Value:      null
		/// </summary>
		public static readonly BindableProperty ContentStringFormatProperty =
				BindableProperty.Create(
						"ContentStringFormat",
						typeof(string),
						typeof(ContentPresenter),
						propertyChanged: (d, oldValue, newValue) =>
						{
							ContentPresenter ctrl = (ContentPresenter)d;
							ctrl.OnContentStringFormatChanged((string)oldValue, (string)newValue);
						});

		/// <summary>
		///     ContentStringFormat is the format used to display the content of
		///     the control as a string.  This arises only when no template is
		///     available.
		/// </summary>
		public String ContentStringFormat
		{
			get { return (String)GetValue(ContentStringFormatProperty); }
			set { SetValue(ContentStringFormatProperty, value); }
		}

		/// <summary>
		///     This method is invoked when the ContentStringFormat property changes.
		/// </summary>
		/// <param name="oldContentStringFormat">The old value of the ContentStringFormat property.</param>
		/// <param name="newContentStringFormat">The new value of the ContentStringFormat property.</param>
		protected virtual void OnContentStringFormatChanged(String oldContentStringFormat, String newContentStringFormat)
		{
			// force on-demand regeneration of the formatting templates for XML and String content
			XMLFormattingTemplateField.ClearValue(this);
			StringFormattingTemplateField.ClearValue(this);
			AccessTextFormattingTemplateField.ClearValue(this);
		}

		#endregion

		#region object Content dependency property
		public static BindableProperty ContentProperty = BindableProperty.Create(
			"Content",
			typeof(object),
			typeof(ContentPresenter),
			propertyChanged: OnContentChanged);
		public object Content
		{
			get
			{
				return (object)GetValue(ContentProperty);
			}
			set
			{
				SetValue(ContentProperty, value);
			}
		}
		private static void OnContentChanged(BindableObject obj, object oldValue, object newValue)
		{
			ContentPresenter cp = obj as ContentPresenter;
			if (!cp.IsInitialized)
				return;
			cp?.ApplyDataTemplate(newValue, cp.ContentTemplate);
		}
		#endregion

		#region DataTemplate ContentTemplate dependency property
		public static BindableProperty ContentTemplateProperty = BindableProperty.Create(
			"ContentTemplate",
			typeof(DataTemplate),
			typeof(ContentPresenter),
			propertyChanged: OnContentTemplateChanged);
		public DataTemplate ContentTemplate
		{
			get
			{
				return (DataTemplate)GetValue(ContentTemplateProperty);
			}
			set
			{
				SetValue(ContentTemplateProperty, value);
			}
		}
		private static void OnContentTemplateChanged(BindableObject obj, object oldValue, object newValue)
		{
			ContentPresenter cp = obj as ContentPresenter;
			if (!cp.IsInitialized)
				return;
			cp?.ApplyDataTemplate(cp.Content, newValue as DataTemplate);
		}
		#endregion

		#region DataTemplateSelector ContentTemplateSelector dependency property
		public static BindableProperty ContentTemplateSelectorProperty = BindableProperty.Create(
			"ContentTemplateSelector",
			typeof(DataTemplateSelector),
			typeof(ContentPresenter),
			null);
		public DataTemplateSelector ContentTemplateSelector
		{
			get
			{
				return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty);
			}
			set
			{
				SetValue(ContentTemplateSelectorProperty, value);
			}
		}
		#endregion

		internal View InternalChild
		{
			get
			{
				return this.Children?.FirstOrDefault() as View;
			}
			set
			{
				this.InternalChildren.Clear();
				if (value != null)
					this.InternalChildren.Add(value);
			}
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ApplyDataTemplate(this.Content, this.ContentTemplate);
		}

		private void ApplyDataTemplate(object content, DataTemplate contentTemplate)
		{
			if (content is View v)
			{
				this.InternalChild = v;
				this.InvalidateMeasure();
				this.InvalidateLayout();
				return;
			}
			else if (content is string s && contentTemplate == null)
			{
				TextBlock tb = new TextBlock();
				tb.Margin = new Thickness(0);
				tb.SetBinding(
					TextBlock.TextProperty,
					new Binding(nameof(Content), mode: BindingMode.OneWay, source: this));
				this.InternalChild = tb;
			}
			else
			{
				if (content == null || contentTemplate == null)
				{
					this.InternalChild = null;
				}
				else
				{
					View view = contentTemplate.CreateContent() as View;
					this.InternalChild = view;
					this.InternalChild.BindingContext = content;
				}
			}

			this.InvalidateMeasure();
			this.InvalidateLayout();
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			Helper.ArrangeElementWithSingleChild(this, x, y, width, height);
		}

		protected override SizeRequest MeasureOverride(double widthConstraint, double heightConstraint)
		{
			if (this.InternalChild != null)
			{
				var sz = this.InternalChild.Measure(
					widthConstraint,
					widthConstraint,
					MeasureFlags.IncludeMargins);
				return sz;
			}
			return new SizeRequest();
		}

		/// <summary>
		/// Prepare to display the item.
		/// </summary>
		internal void PrepareContentPresenter(object item,
								DataTemplate itemTemplate,
								DataTemplateSelector itemTemplateSelector,
								string stringFormat)
		{
			if (item != this)
			{
				// copy templates from parent ItemsControl
				if (_contentIsItem || !HasNonDefaultValue(ContentProperty))
				{
					Content = item;
					_contentIsItem = true;
				}
				if (itemTemplate != null)
					SetValue(ContentTemplateProperty, itemTemplate);
				if (itemTemplateSelector != null)
					SetValue(ContentTemplateSelectorProperty, itemTemplateSelector);
				if (stringFormat != null)
					SetValue(ContentStringFormatProperty, stringFormat);
			}
		}

		/// <summary>
		/// Undo the effect of PrepareContentPresenter.
		/// </summary>
		internal void ClearContentPresenter(object item)
		{
			if (item != this)
			{
				if (_contentIsItem)
				{
					Content = Compatibility.DisconnectedItem;
				}
			}
		}

		private bool _contentIsItem;

		private static readonly UncommonField<DataTemplate> XMLFormattingTemplateField = new UncommonField<DataTemplate>();
		private static readonly UncommonField<DataTemplate> StringFormattingTemplateField = new UncommonField<DataTemplate>();
		private static readonly UncommonField<DataTemplate> AccessTextFormattingTemplateField = new UncommonField<DataTemplate>();
	}
}

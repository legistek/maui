using System;
using System.Collections.Generic;
using System.Text;

using System.Maui.Presentation.Internals;

namespace System.Maui.Presentation.Controls
{
	[ContentProperty(nameof(Content))]
	public class ContentControl : Control
	{
		public ContentControl() : base()
		{
		}

		#region string ContentStringFormat Property

		/// <summary>
		///     The DependencyProperty for the ContentStringFormat property.
		///     Flags:              None
		///     Default Value:      null
		/// </summary>
		public static readonly BindableProperty ContentStringFormatProperty =
				BindableProperty.Create(
						"ContentStringFormat",
						typeof(String),
						typeof(ContentControl),
						propertyChanged: (d, oldValue, newValue) =>
						{
							ContentControl ctrl = (ContentControl)d;
							ctrl.OnContentStringFormatChanged((String)oldValue, (String)newValue);
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
		}

		#endregion

		#region object Content dependency property
		public static BindableProperty ContentProperty = BindableProperty.Create(
			"Content",
			typeof(object),
			typeof(ContentControl),
			null,
			propertyChanged: (e, oldVal, newVal) =>
			{
				(e as ContentControl)?.InvalidateMeasure();
				(e as ContentControl)?.InvalidateLayout();
			});
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
		#endregion

		#region DataTemplate ContentTemplate dependency property
		public static BindableProperty ContentTemplateProperty = BindableProperty.Create(
			"ContentTemplate",
			typeof(DataTemplate),
			typeof(ContentControl),
			null);
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
		#endregion

		#region DataTemplateSelector ContentTemplateSelector dependency property
		public static BindableProperty ContentTemplateSelectorProperty = BindableProperty.Create(
			"ContentTemplateSelector",
			typeof(DataTemplateSelector),
			typeof(ContentControl),
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

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			var presenter = VisualTreeHelper.FindChildElement(
				this,
				elem => elem is Maui.ContentPresenter) as Maui.ContentPresenter;
			if (presenter != null && presenter.Content == null)
			{
				presenter.SetBinding(
					Maui.ContentPresenter.ContentProperty,
					new Binding(nameof(this.Content), source: this));
			}
		}

		/// <summary>
		/// Prepare to display the item.
		/// </summary>
		internal void PrepareContentControl(
			object item,
			DataTemplate itemTemplate,
			DataTemplateSelector itemTemplateSelector,
			string itemStringFormat)
		{
			if (item != this)
			{
				// don't treat Content as a logical child
				ContentIsNotLogical = true;

				// copy styles from the ItemsControl
				if (ContentIsItem || !HasNonDefaultValue(ContentProperty))
				{
					Content = item;
					ContentIsItem = true;
				}
				if (itemTemplate != null)
					SetValue(ContentTemplateProperty, itemTemplate);
				if (itemTemplateSelector != null)
					SetValue(ContentTemplateSelectorProperty, itemTemplateSelector);
				if (itemStringFormat != null)
					SetValue(ContentStringFormatProperty, itemStringFormat);
			}
			else
			{
				ContentIsNotLogical = false;
			}
		}

		/// <summary>
		/// Undo the effect of PrepareContentControl.
		/// </summary>
		internal void ClearContentControl(object item)
		{
			if (item != this)
			{
				if (ContentIsItem)
				{
					Content = Compatibility.DisconnectedItem;
				}
			}
		}

		/// <summary>
		///    Indicates whether Content should be a logical child or not.
		/// </summary>
		internal bool ContentIsNotLogical
		{
			get;
			set;
		}

		/// <summary>
		///    Indicates whether Content is a data item
		/// </summary>
		internal bool ContentIsItem
		{
			get;
			set;
		}
	}
}

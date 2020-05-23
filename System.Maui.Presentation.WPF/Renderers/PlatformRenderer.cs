using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Maui.Platform.WPF;
using System.Maui.Presentation.WPF;

using WFE = System.Windows.FrameworkElement;


namespace System.Maui.Presentation.WPF
{
	public class PlatformRenderer<TElement, TNativeElement> : ViewRenderer<TElement, TNativeElement>
		where TElement : FrameworkElement
		where TNativeElement : WFE, new()
	{
		static Dictionary<string, PlatformPropertyBinding> _propertyBindings =
			new Dictionary<string, PlatformPropertyBinding>();

		static PlatformRenderer()
		{
			BindProperty(nameof(VisualElement.IsEnabled), UIElement.IsEnabledProperty);
			BindProperty(nameof(Frame.Height), WFE.HeightProperty, MauiToWPFDimensionConverter.Instance);
			BindProperty(nameof(Frame.Width), WFE.WidthProperty, MauiToWPFDimensionConverter.Instance);

			BindEvent(FrameworkElement.PointerPressedEvent, WFE.MouseDownEvent, WPFPointerEventArgs.ToFrameworkArgs);
			BindEvent(FrameworkElement.PointerReleasedEvent, WFE.MouseUpEvent, WPFPointerEventArgs.ToFrameworkArgs);
			BindEvent(FrameworkElement.PointerEnteredEvent, WFE.MouseEnterEvent, WPFPointerEventArgs.ToFrameworkArgs);
			BindEvent(FrameworkElement.PointerExitedEvent, WFE.MouseLeaveEvent, WPFPointerEventArgs.ToFrameworkArgs);
		}

		internal static void BindEvent(
			RoutedEvent frameworKEvent,
			System.Windows.RoutedEvent nativeEvent,
			Func<System.Windows.RoutedEventArgs, RoutedEvent, RoutedEventArgs> argConverter)
		{
			var eb = new EventBinding
			{
				FrameworkEvent = frameworKEvent,
				NativeEvent = nativeEvent,
				Convrter = argConverter
			};
			System.Windows.EventManager.RegisterClassHandler(
				typeof(TNativeElement),
				nativeEvent,
				new System.Windows.RoutedEventHandler(eb.OnEvent));
		}

		internal static void BindProperty(
			string visualElementProperty,
			DependencyProperty dp,
			IValueConverter converter = null)
		{
			_propertyBindings.Add(visualElementProperty, new PlatformPropertyBinding
			{
				DP = dp,
				Converter = converter,
				SourceProperty = visualElementProperty,
			});
		}

		protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_propertyBindings.TryGetValue(e.PropertyName, out PlatformPropertyBinding binding))
				binding.UpdateValue(this.Control, this.Element);
		}

		protected override void OnElementChanged(ElementChangedEventArgs<TElement> e)
		{
			if (e.NewElement != null)
			{
				if (Control == null) // construct and SetNativeControl and suscribe control event
				{
					var native = new TNativeElement();
					SetNativeControl(native);
					native.SetValue(Platform.RendererDProperty, this);
				}
				InitializeBindings();
			}
			base.OnElementChanged(e);
		}

		private void InitializeBindings()
		{
			foreach (var binding in _propertyBindings)
				binding.Value.UpdateValue(this.Control, this.Element);
		}

		private struct EventBinding
		{
			public RoutedEvent FrameworkEvent;
			public System.Windows.RoutedEvent NativeEvent;
			public Func<System.Windows.RoutedEventArgs, RoutedEvent, RoutedEventArgs> Convrter;

			public void OnEvent(object sender, System.Windows.RoutedEventArgs e)
			{
				// get native element's renderer
				if (!(sender is System.Windows.UIElement uie))
					return;

				IVisualElementRenderer renderer = uie.GetValue(Platform.RendererDProperty) as IVisualElementRenderer;
				if (renderer == null)
					return;

				if (!(renderer.Element is FrameworkElement fe))
					return;

				RoutedEventArgs args = this.Convrter?.Invoke(e, FrameworkEvent);
				if (args == null)
					return;
				fe.RaiseEvent(args);
			}
		}

		private struct PlatformPropertyBinding
		{
			public DependencyProperty DP;
			public IValueConverter Converter;
			public PropertyInfo ElementProperty;
			public string SourceProperty;

			public void UpdateValue(DependencyObject d, VisualElement src)
			{
				if (ElementProperty == null)
					ElementProperty = src.GetType().GetProperty(SourceProperty);

				var val = ElementProperty.GetValue(src);
				if (this.Converter != null)
					val = Converter.Convert(
						val,
						DP.PropertyType,
						null,
						System.Globalization.CultureInfo.CurrentCulture);
				d.SetValue(DP, val);
			}
		}
	}
}
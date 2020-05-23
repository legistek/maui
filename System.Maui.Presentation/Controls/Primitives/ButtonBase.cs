using System;
using System.Windows.Input;
using System.Maui.Presentation.Input;

namespace System.Maui.Presentation.Controls.Primitives
{
	public class ButtonBase : ContentControl
	{
		#region ICommand Command dependency property
		public static BindableProperty CommandProperty = BindableProperty.Create(
			nameof(Command),
			typeof(ICommand),
			typeof(ButtonBase),
			null,
			propertyChanged: (e, oldVal, newVal) =>
			{
			});
		public ICommand Command
		{
			get
			{
				return (ICommand)GetValue(CommandProperty);
			}
			set
			{
				SetValue(CommandProperty, value);
			}
		}
		#endregion

		#region object CommandParameter dependency property
		public static BindableProperty CommandParameterProperty = BindableProperty.Create(
			nameof(CommandParameter),
			typeof(object),
			typeof(ButtonBase),
			null);
		public object CommandParameter
		{
			get
			{
				return (object)GetValue(CommandParameterProperty);
			}
			set
			{
				SetValue(CommandParameterProperty, value);
			}
		}
		#endregion

		#region ButtonState ButtonState dependency property
		public static BindableProperty ButtonStateProperty = BindableProperty.Create(
			nameof(ButtonState),
			typeof(ButtonState),
			typeof(ButtonBase),
			ButtonState.Normal);
		public ButtonState ButtonState
		{
			get
			{
				return (ButtonState)GetValue(ButtonStateProperty);
			}
			set
			{
				SetValue(ButtonStateProperty, value);
			}
		}
		#endregion

		protected virtual void OnClick()
		{
			this.Command?.Execute(CommandParameter);
		}

		public override void OnPointerEntered(PointerEventArgs e)
		{
			base.OnPointerEntered(e);
			this.UpdateButtonState();
		}

		public override void OnPointerExited(PointerEventArgs e)
		{
			base.OnPointerExited(e);
			this.UpdateButtonState();
		}

		public override void OnPointerPressed(PointerEventArgs e)
		{
			base.OnPointerPressed(e);
			this.UpdateButtonState();
			e.Handled = true;
			e.PointerCaptureRequested = true;
		}

		public override void OnPointerReleased(PointerEventArgs e)
		{
			bool click = this.ButtonState == ButtonState.PressedOver;
			base.OnPointerReleased(e);
			this.UpdateButtonState();
			if (click)
				this.OnClick();
			e.Handled = true;
			e.PointerReleaseRequested = true;
		}

		private void UpdateButtonState()
		{
			if (this.IsPointerDown && this.IsPointerOver)
				this.ButtonState = ButtonState.PressedOver;
			else if (this.IsPointerDown && !this.IsPointerOver)
				this.ButtonState = ButtonState.PressedOutside;
			else if (!this.IsPointerDown && this.IsPointerOver)
				this.ButtonState = ButtonState.Over;
			else
				this.ButtonState = ButtonState.Normal;
		}
	}

	public enum ButtonState
	{
		Normal,
		Over,
		PressedOver,
		PressedOutside
	}
}
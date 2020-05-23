using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

using System.Maui.Presentation.Controls.Primitives;
using System.Collections.Specialized;

namespace System.Maui.Presentation.Controls
{
	[ContentProperty("Children")]
	public abstract class Panel : Layout<View>
	{
		public Panel()
		{
		}
		
		#region bool IsItemsHost dependency property
		public static BindableProperty IsItemsHostProperty = BindableProperty.Create(
			"IsItemsHost",
			typeof(bool),
			typeof(Panel),
			false,
			propertyChanged: (obj, oldVal, newVal)=>
			{
				(obj as Panel)?.OnIsItemsHostChanged((bool)oldVal, (bool)newVal);
			});
		public bool IsItemsHost
		{
			get
			{
				return (bool)GetValue(IsItemsHostProperty);
			}
			set
			{
				SetValue(IsItemsHostProperty, value);
			}
		}
		#endregion

		/// <summary>
		///     This method is invoked when the IsItemsHost property changes.
		/// </summary>
		/// <param name="oldIsItemsHost">The old value of the IsItemsHost property.</param>
		/// <param name="newIsItemsHost">The new value of the IsItemsHost property.</param>
		protected virtual void OnIsItemsHostChanged(bool oldIsItemsHost, bool newIsItemsHost)
		{			
			// GetItemsOwner will check IsItemsHost first, so we don't have
			// to check that IsItemsHost == true before calling it.
			BindableObject parent = ItemsControl.GetItemsOwnerInternal(this, out _);
			ItemsControl itemsControl = parent as ItemsControl;
			Layout oldItemsHost = null;

			if (itemsControl != null)
			{
				// ItemsHost should be the "root" element which has
				// IsItemsHost = true on it.  In the case of grouping,
				// IsItemsHost is true on all panels which are generating
				// content.  Thus, we care only about the panel which
				// is generating content for the ItemsControl.
				IItemContainerGenerator generator = itemsControl.ItemContainerGenerator as IItemContainerGenerator;
				if (generator != null && generator == generator.GetItemContainerGeneratorForPanel(this))
				{
					oldItemsHost = itemsControl.ItemsHost;
					itemsControl.ItemsHost = this;
				}
			}
			else
			{
				GroupItem groupItem = parent as GroupItem;
				if (groupItem != null)
				{
					IItemContainerGenerator generator = groupItem.Generator as IItemContainerGenerator;
					if (generator != null && generator == generator.GetItemContainerGeneratorForPanel(this))
					{
						oldItemsHost = groupItem.ItemsHost;
						groupItem.ItemsHost = this;
					}
				}
			}

			if (oldItemsHost != null && oldItemsHost != this)
			{
				// when changing ItemsHost panels, disconnect the old one
				// oldItemsHost.VerifyBoundState();
			}

			VerifyBoundState();
		}

		ItemContainerGenerator _itemContainerGenerator;
		/// <summary>
		///     The generator associated with this panel.
		/// </summary>
		internal IItemContainerGenerator Generator
		{
			get
			{
				return _itemContainerGenerator;
			}
		}

		internal void EnsureGenerator()
		{
			Debug.Assert(
				IsItemsHost, 
				"Should be invoked only on an ItemsHost panel");

			if (_itemContainerGenerator == null)
			{
				// First access on an items presenter panel
				ConnectToGenerator();

				// Children of this panel should not have their logical parent reset
				EnsureEmptyChildren(/* logicalParent = */ null);

				GenerateChildren();
			}
		}

		internal virtual void OnClearChildrenInternal()
		{
		}

		// This method returns a bool to indicate if or not the panel 
		// layout is affected by this collection change
		internal virtual bool OnItemsChangedInternal(object sender, ItemsChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					AddChildren(args.Position, args.ItemCount);
					break;
				case NotifyCollectionChangedAction.Remove:
					RemoveChildren(args.Position, args.ItemUICount);
					break;
				case NotifyCollectionChangedAction.Replace:
					ReplaceChildren(args.Position, args.ItemCount, args.ItemUICount);
					break;
				case NotifyCollectionChangedAction.Move:
					MoveChildren(args.OldPosition, args.Position, args.ItemUICount);
					break;
				case NotifyCollectionChangedAction.Reset:
					ResetChildren();
					break;
			}

			return true;
		}

		internal virtual void GenerateChildren()
		{
			// This method is typically called during layout, which suspends the dispatcher.
			// Firing an assert causes an exception "Dispatcher processing has been suspended, but messages are still being processed."
			// Besides, the asserted condition can actually arise in practice, and the
			// code responds harmlessly.
			//Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while being asked to generate children.");

			IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
			if (generator != null)
			{
				using (generator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
				{
					View child;
					while ((child = generator.GenerateNext() as View) != null)
					{
						this.Children.Add(child);
						generator.PrepareItemContainer(child);
					}
				}
			}
		}

		private bool VerifyBoundState()
		{
			// If the panel becomes "unbound" while attached to a generator, this
			// method detaches it and makes it really behave like "unbound."  This
			// can happen because of a style change, a theme change, etc. It returns
			// the correct "bound" state, after the dust has settled.
			//
			// This is really a workaround for a more general problem that the panel
			// needs to release resources (an event handler) when it is "out of the tree."
			// Currently, there is no good notification for when this happens.

			bool isItemsHost = (ItemsControl.GetItemsOwnerInternal(this, out _) != null);

			if (isItemsHost)
			{
				if (_itemContainerGenerator == null)
				{
					// Transitioning from being unbound to bound
					ClearChildren();
				}

				return (_itemContainerGenerator != null);
			}
			else
			{
				if (_itemContainerGenerator != null)
				{
					// Transitioning from being bound to unbound
					DisconnectFromGenerator();
					ClearChildren();
				}

				return false;
			}
		}

		private void ConnectToGenerator()
		{
			Debug.Assert(
				_itemContainerGenerator == null, 
				"Attempted to connect to a generator when Panel._itemContainerGenerator is non-null.");

			ItemsControl itemsOwner = ItemsControl.GetItemsOwner(this);
			if (itemsOwner == null)
			{
				// This can happen if IsItemsHost=true, but the panel is not nested in an ItemsControl
				throw new InvalidOperationException("ItemsControl not found");
			}

			IItemContainerGenerator itemsOwnerGenerator = itemsOwner.ItemContainerGenerator;
			if (itemsOwnerGenerator != null)
			{
				_itemContainerGenerator = itemsOwnerGenerator.GetItemContainerGeneratorForPanel(this);
				if (_itemContainerGenerator != null)
				{
					_itemContainerGenerator.ItemsChanged += new ItemsChangedEventHandler(OnItemsChanged);
					((IItemContainerGenerator)_itemContainerGenerator).RemoveAll();
				}
			}
		}

		private void DisconnectFromGenerator()
		{
			Debug.Assert(
				_itemContainerGenerator != null, 
				"Attempted to disconnect from a generator when Panel._itemContainerGenerator is null.");

			_itemContainerGenerator.ItemsChanged -= new ItemsChangedEventHandler(OnItemsChanged);
			((IItemContainerGenerator)_itemContainerGenerator).RemoveAll();
			_itemContainerGenerator = null;
		}

		private void ClearChildren()
		{
			if (_itemContainerGenerator != null)
			{
				((IItemContainerGenerator)_itemContainerGenerator).RemoveAll();
			}

			if ( this.Children?.Count > 0)
			{
				this.Children?.Clear();
				OnClearChildrenInternal();
			}
		}

		private void OnItemsChanged(object sender, ItemsChangedEventArgs args)
		{
			if (VerifyBoundState())
			{
				Debug.Assert(_itemContainerGenerator != null, 
					"Encountered a null _itemContainerGenerator while receiving an ItemsChanged from a generator.");

				bool affectsLayout = OnItemsChangedInternal(sender, args);

				if (affectsLayout)
				{
					InvalidateMeasure();
				}
			}
		}

		private void MoveChildren(
			GeneratorPosition fromPos, 
			GeneratorPosition toPos, 
			int containerCount)
		{
			if (fromPos == toPos)
				return;

			Debug.Assert(
				_itemContainerGenerator != null, 
				"Encountered a null _itemContainerGenerator while receiving an Move action from a generator.");

			IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
			int toIndex = generator.IndexFromGeneratorPosition(toPos);

			View[] elements = new View[containerCount];

			for (int i = 0; i < containerCount; i++)
				elements[i] = this.Children[fromPos.Index + i];

			while (containerCount-- > 0)
				this.Children.RemoveAt(fromPos.Index);

			for (int i = 0; i < containerCount; i++)
			{
				this.Children.Insert(toIndex + i, elements[i]);
			}
		}

		private void ResetChildren()
		{
			EnsureEmptyChildren(null);
			GenerateChildren();
		}

		private void ReplaceChildren(GeneratorPosition pos, int itemCount, int containerCount)
		{
			Debug.Assert(
				itemCount == containerCount, 
				"Panel expects Replace to affect only realized containers");
			Debug.Assert(
				_itemContainerGenerator != null, 
				"Encountered a null _itemContainerGenerator while receiving an Replace action from a generator.");

			IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
			using (generator.StartAt(pos, GeneratorDirection.Forward, true))
			{
				for (int i = 0; i < itemCount; i++)
				{
					bool isNewlyRealized;
					View e = generator.GenerateNext(out isNewlyRealized) as View;

					Debug.Assert(e != null && !isNewlyRealized, "Panel expects Replace to affect only realized containers");
					if (e != null && !isNewlyRealized)
					{
						this.Children[pos.Index + 1] = e;
						generator.PrepareItemContainer(e);
					}
					else
					{
						// TODO -> _itemContainerGenerator.Verify();
					}
				}
			}
		}

		private void EnsureEmptyChildren(FrameworkElement logicalParent)
		{
			ClearChildren();
		}

		private void RemoveChildren(GeneratorPosition pos, int containerCount)
		{
			while (containerCount-- > 0)
			{
				this.Children.RemoveAt(pos.Index);
			}
		}

		private void AddChildren(GeneratorPosition pos, int itemCount)
		{
			Debug.Assert(
				_itemContainerGenerator != null, 
				"Encountered a null _itemContainerGenerator while receiving an Add action from a generator.");

			IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
			using (generator.StartAt(pos, GeneratorDirection.Forward))
			{
				for (int i = 0; i < itemCount; i++)
				{
					View e = generator.GenerateNext() as View;
					if (e != null)
					{
						this.Children.Insert(pos.Index + 1 + i, e);
						generator.PrepareItemContainer(e);
					}
					else
					{
						// TODO -> _itemContainerGenerator.Verify();
					}
				}
			}
		}		
	}
}

using System;
using System.Diagnostics;
using System.Windows.Input;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Collections.ObjectModel;

using System.Maui.Presentation.Internals;
using System.Maui.Presentation.MS.Internal;
using System.Maui.Presentation.MS.Internal.Controls;
using System.Maui.Presentation.Controls.Primitives;
using DependencyObject = System.Maui.BindableObject;

namespace System.Maui.Presentation.Controls
{
	[ContentProperty(nameof(Items))]
	public class ItemsControl : Control, IGeneratorHost
	{
		ObservableCollection<object> _items;

		#region bool IsItemsHost attached dependency property
		public static BindableProperty IsItemsHostProperty = BindableProperty.CreateAttached(
			"IsItemsHost",
			typeof(bool),
			typeof(ItemsControl),
			false);
		public static void SetIsItemsHost(BindableObject obj, bool value)
		{
			obj.SetValue(IsItemsHostProperty, value);
		}
		public static bool GetIsItemsHost(BindableObject obj)
		{
			return (bool)obj.GetValue(IsItemsHostProperty);
		}
		#endregion

		#region ItemTemplateSelector Property
		/// <summary>
		///     The DependencyProperty for the ItemTemplateSelector property.
		///     Flags:              none
		///     Default Value:      null
		/// </summary>
		public static readonly BindableProperty ItemTemplateSelectorProperty =
			BindableProperty.Create(
				"ItemTemplateSelector",
				typeof(DataTemplateSelector),
				typeof(ItemsControl),
				propertyChanged: (obj, oldValue, newValue) =>
				{
					(obj as ItemsControl)?.OnItemTemplateSelectorChanged(
						oldValue as DataTemplateSelector,
						newValue as DataTemplateSelector);
				});

		/// <summary>
		///     ItemTemplateSelector allows the application writer to provide custom logic
		///     for choosing the template used to display each item.
		/// </summary>
		/// <remarks>
		///     This property is ignored if <seealso cref="ItemTemplate"/> is set.
		/// </remarks>
		public DataTemplateSelector ItemTemplateSelector
		{
			get { return (DataTemplateSelector)GetValue(ItemTemplateSelectorProperty); }
			set { SetValue(ItemTemplateSelectorProperty, value); }
		}

		/// <summary>
		///     This method is invoked when the ItemTemplateSelector property changes.
		/// </summary>
		/// <param name="oldItemTemplateSelector">The old value of the ItemTemplateSelector property.</param>
		/// <param name="newItemTemplateSelector">The new value of the ItemTemplateSelector property.</param>
		protected virtual void OnItemTemplateSelectorChanged(
			DataTemplateSelector oldItemTemplateSelector,
			DataTemplateSelector newItemTemplateSelector)
		{
			CheckTemplateSource();

			if ((_itemContainerGenerator != null) && (ItemTemplate == null))
			{
				_itemContainerGenerator.Refresh();
			}
		}
		#endregion

		#region AlternationCount Property

		/// <summary>
		///     The DependencyProperty for the AlternationCount property.
		///     Flags:              none
		///     Default Value:      0
		/// </summary>
		public static readonly BindableProperty AlternationCountProperty =
				BindableProperty.Create(
						"AlternationCount",
						typeof(int),
						typeof(ItemsControl),
						propertyChanged: (d, oldValue, newValue) =>
						{
							ItemsControl ctrl = (ItemsControl)d;

							int oldAlternationCount = (int)oldValue;
							int newAlternationCount = (int)newValue;

							ctrl.OnAlternationCountChanged(oldAlternationCount, newAlternationCount);
						});

		/// <summary>
		///     AlternationCount controls the range of values assigned to the
		///     AlternationIndex property attached to each generated container.  The
		///     default value 0 means "do not set AlternationIndex".  A positive
		///     value means "assign AlternationIndex in the range [0, AlternationCount)
		///     so that adjacent containers receive different values".
		/// </summary>
		/// <remarks>
		///     By referring to AlternationIndex in a trigger or binding (typically
		///     in the ItemContainerStyle), you can make the appearance of items
		///     depend on their position in the display.  For example, you can make
		///     the background color of the items in ListBox alternate between
		///     blue and white.
		/// </remarks>
		public int AlternationCount
		{
			get { return (int)GetValue(AlternationCountProperty); }
			set { SetValue(AlternationCountProperty, value); }
		}

		/// <summary>
		///     This method is invoked when the AlternationCount property changes.
		/// </summary>
		/// <param name="oldAlternationCount">The old value of the AlternationCount property.</param>
		/// <param name="newAlternationCount">The new value of the AlternationCount property.</param>
		protected virtual void OnAlternationCountChanged(int oldAlternationCount, int newAlternationCount)
		{
			ItemContainerGenerator.ChangeAlternationCount();
		}

		#endregion

		#region AlternationIndex Property

		private static readonly BindablePropertyKey AlternationIndexPropertyKey =
			BindableProperty.CreateReadOnly(
				"AlternationIndex",
				typeof(int),
				typeof(ItemsControl),
				0);

		/// <summary>
		/// AlternationIndex is set on containers generated for an ItemsControl, when
		/// the ItemsControl's AlternationCount property is positive.  The AlternationIndex
		/// lies in the range [0, AlternationCount), and adjacent containers always get
		/// assigned different values.
		/// </summary>
		public static readonly BindableProperty AlternationIndexProperty =
					AlternationIndexPropertyKey.BindableProperty;

		/// <summary>
		/// Static getter for the AlternationIndex attached property.
		/// </summary>
		public static int GetAlternationIndex(DependencyObject element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			return (int)element.GetValue(AlternationIndexProperty);
		}

		// internal setter for AlternationIndex.  This property is not settable by
		// an app, only by internal code
		internal static void SetAlternationIndex(DependencyObject d, int value)
		{
			d.SetValue(AlternationIndexPropertyKey, value);
		}

		// internal clearer for AlternationIndex.  This property is not settable by
		// an app, only by internal code
		internal static void ClearAlternationIndex(DependencyObject d)
		{
			d.ClearValue(AlternationIndexPropertyKey);
		}

		#endregion

		#region ItemContainerStyleSelector Property

		/// <summary>
		///     The DependencyProperty for the ItemContainerStyleSelector property.
		///     Flags:              none
		///     Default Value:      null
		/// </summary>
		public static readonly BindableProperty ItemContainerStyleSelectorProperty =
				BindableProperty.Create(
						"ItemContainerStyleSelector",
						typeof(StyleSelector),
						typeof(ItemsControl),
						propertyChanged: (obj, oldValue, newValue) =>
						{
							(obj as ItemsControl)?.OnItemContainerStyleSelectorChanged(
								oldValue as StyleSelector,
								newValue as StyleSelector);
						});

		/// <summary>
		///     ItemContainerStyleSelector allows the application writer to provide custom logic
		///     to choose the style to apply to each generated container element.
		/// </summary>
		/// <remarks>
		///     This property is ignored if <seealso cref="ItemContainerStyle"/> is set.
		/// </remarks>
		public StyleSelector ItemContainerStyleSelector
		{
			get { return (StyleSelector)GetValue(ItemContainerStyleSelectorProperty); }
			set { SetValue(ItemContainerStyleSelectorProperty, value); }
		}

		/// <summary>
		///     This method is invoked when the ItemContainerStyleSelector property changes.
		/// </summary>
		/// <param name="oldItemContainerStyleSelector">The old value of the ItemContainerStyleSelector property.</param>
		/// <param name="newItemContainerStyleSelector">The new value of the ItemContainerStyleSelector property.</param>
		protected virtual void OnItemContainerStyleSelectorChanged(
			StyleSelector oldItemContainerStyleSelector, 
			StyleSelector newItemContainerStyleSelector)
		{
			//Helper.CheckStyleAndStyleSelector(
			//	"ItemContainer", 
			//	ItemContainerStyleProperty, 
			//	ItemContainerStyleSelectorProperty, 
			//	this);

			if ((_itemContainerGenerator != null) && (ItemContainerStyle == null))
			{
				_itemContainerGenerator.Refresh();
			}
		}

		#endregion

		#region ItemStringFormat Property

		/// <summary>
		///     The DependencyProperty for the ItemStringFormat property.
		///     Flags:              None
		///     Default Value:      null
		/// </summary>
		public static readonly BindableProperty ItemStringFormatProperty =
			BindableProperty.Create(
				"ItemStringFormat",
				typeof(String),
				typeof(ItemsControl),
				propertyChanged: (d, oldValue, newValue) =>
				{
					ItemsControl ctrl = (ItemsControl)d;
					ctrl.OnItemStringFormatChanged((string)oldValue, (string)newValue);
					//ctrl.UpdateDisplayMemberTemplateSelector();
				});

		/// <summary>
		///     ItemStringFormat is the format used to display an item (or a
		///     property of an item, as declared by DisplayMemberPath) as a string.
		///     This arises only when no template is available.
		/// </summary>
		public String ItemStringFormat
		{
			get { return (String)GetValue(ItemStringFormatProperty); }
			set { SetValue(ItemStringFormatProperty, value); }
		}

		/// <summary>
		///     This method is invoked when the ItemStringFormat property changes.
		/// </summary>
		/// <param name="oldItemStringFormat">The old value of the ItemStringFormat property.</param>
		/// <param name="newItemStringFormat">The new value of the ItemStringFormat property.</param>
		protected virtual void OnItemStringFormatChanged(String oldItemStringFormat, String newItemStringFormat)
		{
		}

		#endregion

		#region Style ItemContainerStyle dependency property
		public static BindableProperty ItemContainerStyleProperty = BindableProperty.Create(
			"ItemContainerStyle",
			typeof(Style),
			typeof(ItemsControl),
			null);
		public Style ItemContainerStyle
		{
			get
			{
				return (Style)GetValue(ItemContainerStyleProperty);
			}
			set
			{
				SetValue(ItemContainerStyleProperty, value);
			}
		}
		#endregion

		ItemContainerGenerator _itemContainerGenerator;
		public ItemContainerGenerator ItemContainerGenerator
		{
			get
			{
				if ( _itemContainerGenerator == null )
				{				
					_itemContainerGenerator = new ItemContainerGenerator(this)
					{
						ItemsInternal = (this as IGeneratorHost).View
					};
					_itemContainerGenerator.ItemsChanged += OnItemsChanged;
				}
				return _itemContainerGenerator;
			}
		}

		#region DataTemplate ItemsPanel Property

		public static readonly BindableProperty ItemsPanelProperty = BindableProperty.Create(
			nameof(ItemsPanel),
			typeof(ControlTemplate),
			typeof(ItemsControl),
			null);
		public DataTemplate ItemsPanel
		{
			get
			{
				return (DataTemplate)GetValue(ItemsPanelProperty);
			}
			set
			{
				SetValue(ItemsPanelProperty, value);
			}
		}

		#endregion

		#region DataTemplate ItemTemplate dependency property
		public static BindableProperty ItemTemplateProperty = BindableProperty.Create(
			"ItemTemplate",
			typeof(DataTemplate),
			typeof(ItemsControl),
			null);
		public DataTemplate ItemTemplate
		{
			get
			{
				return (DataTemplate)GetValue(ItemTemplateProperty);
			}
			set
			{
				SetValue(ItemTemplateProperty, value);
			}
		}
		#endregion

		#region IEnumerable ItemsSource dependency property
		public static BindableProperty ItemsSourceProperty = BindableProperty.Create(
			"ItemsSource",
			typeof(IEnumerable),
			typeof(ItemsControl),
			propertyChanged: (obj, oldVal, newVal) =>
			{
				ItemsControl ic = obj as ItemsControl;
				if (ic == null)
					return;
				if (oldVal is INotifyCollectionChanged oldINCC)
				{
					oldINCC.CollectionChanged -= ic.OnItemsSourceCollectionChanged;
				}
				if (newVal is INotifyCollectionChanged newINCC)
				{
					newINCC.CollectionChanged += ic.OnItemsSourceCollectionChanged;
				}

				if (newVal is IList ienum)
				{
					ic.Items = ienum;
				}
				else
				{
					ic.Items = new List<object>(((IEnumerable)newVal).Cast<object>());
				}
				ic.InvalidateLayout();
			});
		public IEnumerable ItemsSource
		{
			get
			{
				return (IEnumerable)GetValue(ItemsSourceProperty);
			}
			set
			{
				SetValue(ItemsSourceProperty, value);
			}
		}
		private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					_items.Add(e.NewItems.Cast<object>().First());
					break;
				case NotifyCollectionChangedAction.Remove:
					_items.RemoveAt(e.OldStartingIndex);
					break;
			}			
		}
		#endregion

		public IList Items
		{
			get
			{
				if (_items == null)
				{
					var oc = new ObservableCollection<object>();
					_items = oc;
					oc.CollectionChanged += (sender, e)=>
					{
						InvalidateLayout();
					};
				}
				return _items;
			}
			set
			{
				_items = new ObservableCollection<object>(value.Cast<object>());
				InvalidateLayout();
			}
		}

		/// <summary>
		/// Determine whether the ItemContainerStyle/StyleSelector should apply to the container
		/// </summary>
		/// <returns>true if the ItemContainerStyle should apply to the item</returns>
		protected virtual bool ShouldApplyItemContainerStyle(DependencyObject container, object item)
		{
			return true;
		}

		protected override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			ItemsHost = VisualTreeHelper.FindChildElement(
				this,
				elem => GetIsItemsHost(elem)) as Layout<View>;
		}

		public virtual View GetContainerForItemOverride()
		{
			return new ContentControl();
		}

		/// <summary>
		/// Return true if the item is (or should be) its own item container
		/// </summary>
		protected virtual bool IsItemItsOwnContainerOverride(object item)
		{
			return (item is VisualElement);
		}

		/// <summary>
		/// Prepare the element to display the item.  This may involve
		/// applying styles, setting bindings, etc.
		/// </summary>
		protected virtual void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			// Each type of "ItemContainer" element may require its own initialization.
			// We use explicit polymorphism via internal methods for this.
			//
			// Another way would be to define an interface IGeneratedItemContainer with
			// corresponding virtual "core" methods.  Base classes (ContentControl,
			// ItemsControl, ContentPresenter) would implement the interface
			// and forward the work to subclasses via the "core" methods.
			//
			// While this is better from an OO point of view, and extends to
			// 3rd-party elements used as containers, it exposes more public API.
			// Management considers this undesirable, hence the following rather
			// inelegant code.

			//HeaderedContentControl hcc;
			ContentControl cc;
			ContentPresenter cp;
			ItemsControl ic;
			//HeaderedItemsControl hic;

			if (false) ;
			//else if ((hcc = element as HeaderedContentControl) != null)
			//{
			//	hcc.PrepareHeaderedContentControl(item, ItemTemplate, ItemTemplateSelector, ItemStringFormat);
			//}
			else if ((cc = element as ContentControl) != null)
			{
				cc.PrepareContentControl(item, ItemTemplate, ItemTemplateSelector, ItemStringFormat);
			}
			else if ((cp = element as ContentPresenter) != null)
			{
				cp.PrepareContentPresenter(item, ItemTemplate, ItemTemplateSelector, ItemStringFormat);
			}
			//else if ((hic = element as HeaderedItemsControl) != null)
			//{
			//	hic.PrepareHeaderedItemsControl(item, this);
			//}
			//else if ((ic = element as ItemsControl) != null)
			//{
			//	if (ic != this)
			//	{
			//		ic.PrepareItemsControl(item, this);
			//	}
			//}
		}

		internal Layout<View> ItemsHost { get; set; }

		#region IsStyleSetFromGenerator Internal Property

		internal static readonly BindableProperty IsStyleSetFromGeneratorProperty =
			BindableProperty.Create(
				"IsStyleSetFromGenerator", 
				typeof(bool), 
				typeof(ItemsControl), 
				false);
		internal static bool GetIsStyleSetFromGenerator(BindableObject obj)
		{
			return (bool)obj.GetValue(IsStyleSetFromGeneratorProperty);
		}
		internal static void SetIsStyleSetFromGenerator(BindableObject obj, bool value)
		{
			obj.SetValue(IsStyleSetFromGeneratorProperty, value);
		}

		#endregion

		/// <summary>
		/// Different from public GetItemsOwner
		/// Returns ip.TemplatedParent instead of ip.Owner
		/// More accurate when we want to distinguish if owner is a GroupItem or ItemsControl
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		internal static BindableObject GetItemsOwnerInternal(
			BindableObject element,
			out ItemsControl itemsControl)
		{
			ITemplateParent container = null;
			Layout panel = element as Layout;
			itemsControl = null;

			if (panel != null && GetIsItemsHost(panel))
			{
				// see if element was generated for an ItemsPresenter
				ItemsPresenter ip = ItemsPresenter.FromPanel(panel);

				if (ip != null)
				{
					// if so use the element whose style begat the ItemsPresenter
					container = ip.TemplatedParent;
					itemsControl = ip.Owner;
				}
				else
				{
					// otherwise use element's templated parent
					container = panel.TemplatedParent();
					itemsControl = container as ItemsControl;
				}
			}

			return container as BindableObject;
		}

		// A version of Object.Equals with paranoia for mismatched types, to avoid problems
		// with classes that implement Object.Equals poorly
		internal static bool EqualsEx(object o1, object o2)
		{
			try
			{
				return Object.Equals(o1, o2);
			}
			catch (System.InvalidCastException)
			{
				// A common programming error: the type of o1 overrides Equals(object o2)
				// but mistakenly assumes that o2 has the same type as o1:
				//     MyType x = (MyType)o2;
				// This throws InvalidCastException when o2 is a sentinel object,
				// e.g. UnsetValue, DisconnectedItem, NewItemPlaceholder, etc.
				// Rather than crash, just return false - the objects are clearly unequal.
				return false;
			}
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			base.LayoutChildren(x, y, width, height);			
			GenerateChildren();
		}

		internal virtual void GenerateChildren()
		{
			Debug.WriteLine($"GenerateChildren called");
			IItemContainerGenerator generator = (IItemContainerGenerator)this.ItemContainerGenerator;
			if (generator != null)
			{
				using (generator.StartAt(new GeneratorPosition(-1, 0), GeneratorDirection.Forward))
				{
					View child;
					while ((child = generator.GenerateNext() as View) != null)
					{
						Debug.WriteLine($"Adding child");
						ItemsHostChildren?.Add(child);
						generator.PrepareItemContainer(child);
					}
				}
			}
		}

		/// <summary>
		/// Throw if more than one of DisplayMemberPath, xxxTemplate and xxxTemplateSelector
		/// properties are set on the given element.
		/// </summary>
		private void CheckTemplateSource()
		{
			//if (string.IsNullOrEmpty(DisplayMemberPath))
			//{
			//	Helper.CheckTemplateAndTemplateSelector("Item", ItemTemplateProperty, ItemTemplateSelectorProperty, this);
			//}
			//else
			//{
			//	if (!(this.ItemTemplateSelector is DisplayMemberTemplateSelector))
			//	{
			//		throw new InvalidOperationException(SR.Get(SRID.ItemTemplateSelectorBreaksDisplayMemberPath));
			//	}
			//	if (Helper.IsTemplateDefined(ItemTemplateProperty, this))
			//	{
			//		throw new InvalidOperationException(SR.Get(SRID.DisplayMemberPathAndItemTemplateDefined));
			//	}
			//}
		}

		#region IGeneratorHost Implementation (and helper methods)

		ObservableCollection<object> IGeneratorHost.View
		{
			get => _items;
		}

		/// <summary>
		/// Prepare the element to act as the ItemContainer for the corresponding item.
		/// </summary>
		void IGeneratorHost.PrepareItemContainer(BindableObject container, object item)
		{
			// GroupItems are special - their information comes from a different place
			//GroupItem groupItem = container as GroupItem;
			//if (groupItem != null)
			//{
			//	groupItem.PrepareItemContainer(item, this);
			//	return;
			//}

			if (ShouldApplyItemContainerStyle(container, item))
			{
				// apply the ItemContainer style (if any)
				ApplyItemContainerStyle(container, item);
			}

			// forward ItemTemplate, et al.
			PrepareContainerForItemOverride(container, item);

			// set up the binding group
			//if (!Helper.HasUnmodifiedDefaultValue(this, ItemBindingGroupProperty) &&
			//	Helper.HasUnmodifiedDefaultOrInheritedValue(container, FrameworkElement.BindingGroupProperty))
			//{
			//	BindingGroup itemBindingGroup = ItemBindingGroup;
			//	BindingGroup containerBindingGroup =
			//		(itemBindingGroup != null) ? new BindingGroup(itemBindingGroup)
			//									: null;
			//	container.SetValue(FrameworkElement.BindingGroupProperty, containerBindingGroup);
			//}

			//if (container == item && TraceData.IsEnabled)
			//{
			//	// issue a message if there's an ItemTemplate(Selector) for "direct" items
			//	// The ItemTemplate isn't used, which may confuse the user (bug 991101).
			//	if (ItemTemplate != null || ItemTemplateSelector != null)
			//	{
			//		TraceData.Trace(TraceEventType.Error, TraceData.ItemTemplateForDirectItem, AvTrace.TypeName(item));
			//	}
			//}

			//TreeViewItem treeViewItem = container as TreeViewItem;
			//if (treeViewItem != null)
			//{
			//	treeViewItem.PrepareItemContainer(item, this);
			//}
		}


		/// <summary>
		/// Return the element used to display the given item
		/// </summary>
		BindableObject IGeneratorHost.GetContainerForItem(object item)
		{
			BindableObject container;

			// use the item directly, if possible (bug 870672)
			if (IsItemItsOwnContainerOverride(item))
				container = item as BindableObject;
			else
				container = GetContainerForItemOverride();

			// the container might have a parent from a previous
			// generation (bug 873118).  If so, clean it up before using it again.
			//
			// Note: This assumes the container is about to be added to a new parent,
			// according to the ItemsControl/Generator/Container pattern.
			// If someone calls the generator and doesn't add the container to
			// a visual parent, unexpected things might happen.
			VisualElement visual = container as VisualElement;
			if (visual != null)
			{
				VisualElement parent = visual.RealParent as VisualElement;
				if (parent != null)
				{
					Debug.Assert(parent is View);
					Layout p = parent as Layout;
					if (p != null && (visual is View view))
					{
						(p.Children as IList<Element>)?.Remove(view);
					}
				}
			}

			return container;
		}


		/// <summary>
		/// Return true if the item is (or should be) its own item container
		/// </summary>
		public bool IsItemItsOwnContainer(object item)
		{
			return IsItemItsOwnContainerOverride(item);
		}

		/// <summary>
		/// Undo any initialization done on the element during GetContainerForItem and PrepareItemContainer
		/// </summary>
		void IGeneratorHost.ClearContainerForItem(DependencyObject container, object item)
		{
			// This method no longer does most of the work it used to (bug 1445288).
			// It is called when a container is removed from the tree;  such a
			// container will be GC'd soon, so there's no point in changing
			// its properties.
			//
			// We still call the override method, to give subclasses a chance
			// to clean up anything they may have done during Prepare (bug 1561206).

			GroupItem groupItem = container as GroupItem;
			if (groupItem == null)
			{
				ClearContainerForItemOverride(container, item);

				//TreeViewItem treeViewItem = container as TreeViewItem;
				//if (treeViewItem != null)
				//{
				//	treeViewItem.ClearItemContainer(item, this);
				//}
			}
			//else
			//{
			//	// GroupItems are special - their information comes from a different place
			//	// Recursively clear the sub-generators, so that ClearContainerForItemOverride
			//	// is called on the bottom-level containers.
			//	groupItem.ClearItemContainer(item, this);
			//}
		}


		/// <summary>
		/// Determine if the given element was generated for this host as an ItemContainer.
		/// </summary>
		bool IGeneratorHost.IsHostForItemContainer(DependencyObject container)
		{
			// If ItemsControlFromItemContainer can determine who owns the element,
			// use its decision.
			ItemsControl ic = ItemsControlFromItemContainer(container);
			if (ic != null)
				return (ic == this);

			// If the element is in my items view, and if it can be its own ItemContainer,
			// it's mine.  Contains may be expensive, so we avoid calling it in cases
			// where we already know the answer - namely when the element has a
			// logical parent (ItemsControlFromItemContainer handles this case).  This
			// leaves only those cases where the element belongs to my items
			// without having a logical parent (e.g. via ItemsSource) and without
			// having been generated yet. HasItem indicates if anything has been generated.
			DependencyObject parent = (container as Element)?.Parent;
			if (parent == null)
			{
				return IsItemItsOwnContainerOverride(container) &&
					Items?.Count > 0 && Items.Contains(container);
			}

			// Otherwise it's not mine
			return false;
		}

		/// <summary>
		/// Undo the effects of PrepareContainerForItemOverride.
		/// </summary>
		protected virtual void ClearContainerForItemOverride(DependencyObject element, object item)
		{
			//HeaderedContentControl hcc;
			ContentControl cc;
			ContentPresenter cp;
			ItemsControl ic;
			//HeaderedItemsControl hic;

			if (false) ;
			//else if ((hcc = element as HeaderedContentControl) != null)
			//{
			//	hcc.ClearHeaderedContentControl(item);
			//}
			else if ((cc = element as ContentControl) != null)
			{
				cc.ClearContentControl(item);
			}
			else if ((cp = element as ContentPresenter) != null)
			{
				cp.ClearContentPresenter(item);
			}
			//else if ((hic = element as HeaderedItemsControl) != null)
			//{
			//	hic.ClearHeaderedItemsControl(item);
			//}
			//else if ((ic = element as ItemsControl) != null)
			//{
			//	if (ic != this)
			//	{
			//		ic.ClearItemsControl(item);
			//	}
			//}
		}

		private void ApplyItemContainerStyle(DependencyObject container, object item)
		{
			View foContainer = container as View;

			// don't overwrite a locally-defined style (bug 1018408)
			if (!GetIsStyleSetFromGenerator(foContainer) &&
				container.GetValue(FrameworkElement.StyleProperty) != null)
			{
				return;
			}

			// Control's ItemContainerStyle has first stab
			Style style = ItemContainerStyle;

			// no ItemContainerStyle set, try ItemContainerStyleSelector
			if (style == null)
			{
				if (ItemContainerStyleSelector != null)
				{
					style = ItemContainerStyleSelector.SelectStyle(item, container);
				}
			}

			// apply the style, if found
			if (style != null)
			{
				// verify style is appropriate before applying it
				if (!style.TargetType.IsInstanceOfType(container))
					throw new InvalidOperationException();

				foContainer.Style = style;
				SetIsStyleSetFromGenerator(foContainer, true);
			}
			else if ((bool)foContainer.GetValue(IsStyleSetFromGeneratorProperty))
			{
				// if Style was formerly set from ItemContainerStyle, clear it
				SetIsStyleSetFromGenerator(foContainer, false);
				container.ClearValue(FrameworkElement.StyleProperty);
			}
		}


		/// <summary>
		///     Returns the ItemsControl for which element is an ItemsHost.
		///     More precisely, if element is marked by setting IsItemsHost="true"
		///     in the style for an ItemsControl, or if element is a panel created
		///     by the ItemsPresenter for an ItemsControl, return that ItemsControl.
		///     Otherwise, return null.
		/// </summary>
		public static ItemsControl GetItemsOwner(BindableObject element)
		{
			ItemsControl container = null;
			Layout panel = element as Layout;

			if (panel != null && GetIsItemsHost(panel))
			{
				// see if element was generated for an ItemsPresenter
				ItemsPresenter ip = ItemsPresenter.FromPanel(panel);

				if (ip != null)
				{
					// if so use the element whose style begat the ItemsPresenter
					container = ip.Owner;
				}
				else
				{
					// otherwise use element's templated parent
					container = panel.TemplatedParent() as ItemsControl;
				}
			}

			return container;
		}

		///<summary>
		/// Return the ItemsControl that owns the given container element
		///</summary>
		public static ItemsControl ItemsControlFromItemContainer(DependencyObject container)
		{
			Element ui = container as Element;
			if (ui == null)
				return null;

			// ui appeared in items collection
			ItemsControl ic = VisualTreeHelper.FindParent(ui, elem => elem is ItemsControl)
				as ItemsControl;
			if (ic != null)
			{
				// this is the right ItemsControl as long as the item
				// is (or is eligible to be) its own container
				IGeneratorHost host = ic as IGeneratorHost;
				if (host.IsItemItsOwnContainer(ui))
					return ic;
				else
					return null;
			}

			ui = ui.Parent as Element;

			return ItemsControl.GetItemsOwner(ui);
		}


		#endregion

		#region Methods Ported from WPF Panel so that any generic Layout<View> works

		private void OnItemsChanged(object sender, ItemsChangedEventArgs args)
		{
			//if (VerifyBoundState())
			{
				Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while receiving an ItemsChanged from a generator.");

				bool affectsLayout = OnItemsChangedInternal(sender, args);

				if (affectsLayout)
				{
					InvalidateMeasure();
				}
			}
		}

		// This method returns a bool to indicate if or not the panel layout is affected by this collection change
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
					//case NotifyCollectionChangedAction.Replace:
					//	ReplaceChildren(args.Position, args.ItemCount, args.ItemUICount);
					//	break;
					//case NotifyCollectionChangedAction.Move:
					//	MoveChildren(args.OldPosition, args.Position, args.ItemUICount);
					//	break;

					//case NotifyCollectionChangedAction.Reset:
					//	ResetChildren();
					//	break;
			}

			return true;
		}

		private void AddChildren(GeneratorPosition pos, int itemCount)
		{
			Debug.Assert(_itemContainerGenerator != null, "Encountered a null _itemContainerGenerator while receiving an Add action from a generator.");

			IItemContainerGenerator generator = (IItemContainerGenerator)_itemContainerGenerator;
			using (generator.StartAt(pos, GeneratorDirection.Forward))
			{
				for (int i = 0; i < itemCount; i++)
				{
					View e = generator.GenerateNext() as View;
					if (e != null)
					{
						ItemsHostChildren?.Insert(pos.Index + 1 + i, e);
						generator.PrepareItemContainer(e);
					}
					else
					{
						_itemContainerGenerator.Verify();
					}
				}
			}
		}

		private void RemoveChildren(GeneratorPosition pos, int containerCount)
		{
			// If anything is wrong, I think these collections should do parameter checking
			while (containerCount-- > 0)
				ItemsHostChildren?.RemoveAt(pos.Index);
		}

		#endregion

		private IList<View> ItemsHostChildren =>
			this.ItemsHost?.Children;
	}
}

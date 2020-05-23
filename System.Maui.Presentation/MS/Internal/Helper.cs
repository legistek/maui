using System;
using System.Windows;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Maui.Presentation.Controls;
using System.Maui.Presentation.Controls.Primitives;

using DependencyObject = System.Maui.BindableObject;
using DependencyProperty = System.Maui.BindableProperty;

namespace System.Maui.Presentation.MS.Internal
{
    internal static class Helper
    {
		/// <summary>
		/// Measure a simple element with a single child.
		/// </summary>
		internal static Size MeasureElementWithSingleChild(Layout element, Size constraint)
		{
			Layout child = element.Children?.FirstOrDefault() as Layout;

			if (child != null)
			{
				return child.Measure(constraint.Width, constraint.Height).Request;
			}

			return new Size();
		}

		/// <summary>
		/// Arrange a simple element with a single child.
		/// </summary>
		internal static Size ArrangeElementWithSingleChild(Layout element, double x, double y, double width, double height)
		{
			var child = element.Children?.FirstOrDefault() as VisualElement;

			if (child != null)
			{
				Layout.LayoutChildIntoBoundingRegion(
					child, 
					new Rectangle(x, y, width, height));				
			}

			return new Size(width, height);
		}

		internal static void ApplyCorrectionFactorToPixelHeaderSize(
			ItemsControl scrollingItemsControl,
			FrameworkElement virtualizingElement,
			Panel itemsHost,
			ref Size headerSize)
		{
			if (!VirtualizingStackPanel.IsVSP45Compat)
				return;

			bool shouldApplyItemsCorrectionFactor = itemsHost != null && itemsHost.IsVisible;
			if (shouldApplyItemsCorrectionFactor)
			{
				Thickness itemsCorrectionFactor = GroupItem.DesiredPixelItemsSizeCorrectionFactorField.GetValue(virtualizingElement);
				headerSize.Height = Math.Max(itemsCorrectionFactor.Top, headerSize.Height);
			}
			else
			{
				headerSize.Height = Math.Max(virtualizingElement.DesiredSize.Height, headerSize.Height);
			}
			headerSize.Width = Math.Max(virtualizingElement.DesiredSize.Width, headerSize.Width);
		}

		internal static HierarchicalVirtualizationItemDesiredSizes ApplyCorrectionFactorToItemDesiredSizes(
			Control virtualizingElement,
			Panel itemsHost)
		{
			HierarchicalVirtualizationItemDesiredSizes itemDesiredSizes =
				GroupItem.HierarchicalVirtualizationItemDesiredSizesField.GetValue(virtualizingElement);

			if (!VirtualizingStackPanel.IsVSP45Compat)
				return itemDesiredSizes;

			if (itemsHost != null && itemsHost.IsVisible)
			{
				Size itemPixelSize = itemDesiredSizes.PixelSize;
				Size itemPixelSizeInViewport = itemDesiredSizes.PixelSizeInViewport;
				Size itemPixelSizeBeforeViewport = itemDesiredSizes.PixelSizeBeforeViewport;
				Size itemPixelSizeAfterViewport = itemDesiredSizes.PixelSizeAfterViewport;
				bool correctionComputed = false;
				Thickness correctionFactor = new Thickness(0);
				Size desiredSize = virtualizingElement.DesiredSize;

				if (DoubleUtil.GreaterThan(itemPixelSize.Height, 0))
				{
					correctionFactor = GroupItem.DesiredPixelItemsSizeCorrectionFactorField.GetValue(virtualizingElement);
					itemPixelSize.Height += correctionFactor.Bottom;
					correctionComputed = true;
				}
				itemPixelSize.Width = Math.Max(desiredSize.Width, itemPixelSize.Width);

				if (DoubleUtil.AreClose(itemDesiredSizes.PixelSizeAfterViewport.Height, 0) &&
					DoubleUtil.AreClose(itemDesiredSizes.PixelSizeInViewport.Height, 0) &&
					DoubleUtil.GreaterThan(itemDesiredSizes.PixelSizeBeforeViewport.Height, 0))
				{
					if (!correctionComputed)
					{
						correctionFactor = GroupItem.DesiredPixelItemsSizeCorrectionFactorField.GetValue(virtualizingElement);
					}
					itemPixelSizeBeforeViewport.Height += correctionFactor.Bottom;
					correctionComputed = true;
				}
				itemPixelSizeBeforeViewport.Width = Math.Max(desiredSize.Width, itemPixelSizeBeforeViewport.Width);

				if (DoubleUtil.AreClose(itemDesiredSizes.PixelSizeAfterViewport.Height, 0) &&
					DoubleUtil.GreaterThan(itemDesiredSizes.PixelSizeInViewport.Height, 0))
				{
					if (!correctionComputed)
					{
						correctionFactor = GroupItem.DesiredPixelItemsSizeCorrectionFactorField.GetValue(virtualizingElement);
					}
					itemPixelSizeInViewport.Height += correctionFactor.Bottom;
					correctionComputed = true;
				}
				itemPixelSizeInViewport.Width = Math.Max(desiredSize.Width, itemPixelSizeInViewport.Width);

				if (DoubleUtil.GreaterThan(itemDesiredSizes.PixelSizeAfterViewport.Height, 0))
				{
					if (!correctionComputed)
					{
						correctionFactor = GroupItem.DesiredPixelItemsSizeCorrectionFactorField.GetValue(virtualizingElement);
					}
					itemPixelSizeAfterViewport.Height += correctionFactor.Bottom;
					correctionComputed = true;
				}
				itemPixelSizeAfterViewport.Width = Math.Max(desiredSize.Width, itemPixelSizeAfterViewport.Width);

				itemDesiredSizes = new HierarchicalVirtualizationItemDesiredSizes(itemDesiredSizes.LogicalSize,
					itemDesiredSizes.LogicalSizeInViewport,
					itemDesiredSizes.LogicalSizeBeforeViewport,
					itemDesiredSizes.LogicalSizeAfterViewport,
					itemPixelSize,
					itemPixelSizeInViewport,
					itemPixelSizeBeforeViewport,
					itemPixelSizeAfterViewport);
			}
			return itemDesiredSizes;
		}


		internal static object ReadItemValue(DependencyObject owner, object item, BindableProperty bp)
		{
			if (item != null)
			{
				List<KeyValuePair<BindableProperty, object>> itemValues = GetItemValues(owner, item);

				if (itemValues != null)
				{
					for (int i = 0; i < itemValues.Count; i++)
					{
						if (itemValues[i].Key == bp)
						{
							return itemValues[i].Value;
						}
					}
				}
			}

			return null;
		}


		internal static void StoreItemValue(DependencyObject owner, object item, BindableProperty bp, object value)
		{
			if (item != null)
			{
				List<KeyValuePair<BindableProperty, object>> itemValues = EnsureItemValues(owner, item);

				//
				// Find the key, if it exists, and modify its value.  Since the number of DPs we want to store
				// is typically very small, using a List in this manner is faster than hashing
				//

				bool found = false;
				KeyValuePair<BindableProperty, object> keyValue = new KeyValuePair<BindableProperty, object>(bp, value);

				for (int j = 0; j < itemValues.Count; j++)
				{
					if (itemValues[j].Key == bp)
					{
						itemValues[j] = keyValue;
						found = true;
						break;
					}
				}

				if (!found)
				{
					itemValues.Add(keyValue);
				}
			}
		}

		internal static void ClearItemValue(DependencyObject owner, object item, BindableProperty bp)
		{
			if (item != null)
			{
				List<KeyValuePair<BindableProperty, object>> itemValues = GetItemValues(owner, item);

				if (itemValues != null)
				{
					for (int i = 0; i < itemValues.Count; i++)
					{
						if (itemValues[i].Key == bp)
						{
							itemValues.RemoveAt(i);
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Returns the ItemValues list for a given item.  May return null if one hasn't been set yet.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		internal static List<KeyValuePair<BindableProperty, object>> GetItemValues(DependencyObject owner, object item)
		{
			return GetItemValues(owner, item, ItemValueStorageField.GetValue(owner));
		}

		internal static List<KeyValuePair<BindableProperty, object>> GetItemValues(
			DependencyObject owner, 
			object item,
			WeakDictionary<object, 
				List<KeyValuePair<BindableProperty, object>>> itemValueStorage)
		{
			Debug.Assert(item != null);
			List<KeyValuePair<BindableProperty, object>> itemValues = null;

			if (itemValueStorage != null)
			{
				itemValueStorage.TryGetValue(item, out itemValues);
			}

			return itemValues;
		}


		internal static List<KeyValuePair<BindableProperty, object>> EnsureItemValues(DependencyObject owner, object item)
		{
			WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>> itemValueStorage = EnsureItemValueStorage(owner);
			List<KeyValuePair<BindableProperty, object>> itemValues = GetItemValues(owner, item, itemValueStorage);

			if (itemValues == null)
			{
				itemValues = new List<KeyValuePair<BindableProperty, object>>(3);    // So far the only use of this is to store three values.
				itemValueStorage[item] = itemValues;
			}

			return itemValues;
		}


		internal static WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>> EnsureItemValueStorage(DependencyObject owner)
		{
			WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>> itemValueStorage = ItemValueStorageField.GetValue(owner);

			if (itemValueStorage == null)
			{
				itemValueStorage = new WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>>();
				ItemValueStorageField.SetValue(owner, itemValueStorage);
			}

			return itemValueStorage;
		}

		///// <summary>
		///// Sets all values saved in ItemValueStorage for the given item onto the container
		///// </summary>
		///// <param name="container"></param>
		///// <param name="item"></param>
		//internal static void SetItemValuesOnContainer(DependencyObject owner, DependencyObject container, object item)
		//{
		//	BindableProperty[] dpIndices = ItemValueStorageIndices;
		//	List<KeyValuePair<BindableProperty, object>> itemValues = 
		//		GetItemValues(owner, item) ?? new List<KeyValuePair<BindableProperty, object>>();

		//	for (int j = 0; j < dpIndices.Length; j++)
		//	{
		//		DependencyProperty dp = dpIndices[j];
		//		object value = null;

		//		for (int i = 0; i < itemValues.Count; i++)
		//		{
		//			if (itemValues[i].Key == dp)
		//			{
		//				value = itemValues[i].Value;
		//				break;
		//			}
		//		}

		//		if (dp != null)
		//		{
		//			if (value != null)
		//			{
		//				ModifiedItemValue modifiedItemValue = value as ModifiedItemValue;
		//				if (modifiedItemValue == null)
		//				{
		//					// for real properties, call SetValue so that the property's
		//					// change-callback is called
		//					container.SetValue(dp, value);
		//				}
		//				else if (modifiedItemValue.IsCoercedWithCurrentValue)
		//				{
		//					// set as current-value
		//					container.SetValue(dp, modifiedItemValue.Value);
		//				}
		//			}
		//			else if (container != container.GetValue(ItemContainerGenerator.ItemForItemContainerProperty))
		//			{
		//				// at this point we have
		//				//   a. a real property (dp != null)
		//				//   b. with no saved value (value != Unset)
		//				//   c. a generated container (container != item)
		//				// If the container has a local or current value for the
		//				// property, it came from a previous lifetime before the
		//				// container was recycled and should be discarded
		//				EntryIndex entryIndex = container.LookupEntry(dpIndex);
		//				EffectiveValueEntry entry = new EffectiveValueEntry(dp);

		//				// first discard the current value, if any.
		//				if (entryIndex.Found)
		//				{
		//					entry = container.EffectiveValues[entryIndex.Index];

		//					if (entry.IsCoercedWithCurrentValue)
		//					{
		//						// call ClearCurrentValue  (when it exists - for now, use the substitute - (see comment at DO.InvalidateProperty)
		//						container.InvalidateProperty(dp, preserveCurrentValue: false);

		//						// side-effects may move the entry - re-fetch it
		//						entryIndex = container.LookupEntry(dpIndex);

		//						if (entryIndex.Found)
		//						{
		//							entry = container.EffectiveValues[entryIndex.Index];
		//						}
		//					}
		//				}

		//				// next discard values that were set in a previous lifetime
		//				if (entryIndex.Found)
		//				{
		//					if ((entry.BaseValueSourceInternal == BaseValueSourceInternal.Local ||
		//						 entry.BaseValueSourceInternal == BaseValueSourceInternal.ParentTemplate) &&
		//						 !entry.HasModifiers)
		//					{
		//						// this entry denotes a value from a previous lifetime - discard it
		//						container.ClearValue(dp);
		//					}
		//				}
		//			}
		//		}
		//		else if (value != null)
		//		{
		//			// for "fake" properties (no corresponding DP - e.g. VSP's desired-size),
		//			// set the property directly into the effective value table
		//			EntryIndex entryIndex = container.LookupEntry(dpIndex);
		//			container.SetValue(entryIndex, null /*dp*/, dp, null /*metadata*/, value, BaseValueSourceInternal.Local);
		//		}
		//	}
		//}

		///// <summary>
		///// Stores the value of a container for the given item and set of dependency properties
		///// </summary>
		///// <param name="container"></param>
		///// <param name="item"></param>
		///// <param name="dpIndices"></param>
		//internal static void StoreItemValues(IContainItemStorage owner, DependencyObject container, object item)
		//{
		//	BindableProperty[] dpIndices = ItemValueStorageIndices;

		//	DependencyObject ownerDO = (DependencyObject)owner;

		//	//
		//	// Loop through all DPs we care about storing.  If the container has a current-value or locally-set value we'll store it.
		//	//
		//	for (int i = 0; i < dpIndices.Length; i++)
		//	{
		//		var dpIndex = dpIndices[i];
		//		EntryIndex entryIndex = container.LookupEntry(dpIndex);

		//		if (entryIndex.Found)
		//		{
		//			EffectiveValueEntry entry = container.EffectiveValues[entryIndex.Index];

		//			if ((entry.BaseValueSourceInternal == BaseValueSourceInternal.Local ||
		//				 entry.BaseValueSourceInternal == BaseValueSourceInternal.ParentTemplate) &&
		//				 !entry.HasModifiers)
		//			{
		//				// store local values that aren't modified
		//				StoreItemValue(ownerDO, item, dpIndex, entry.Value);
		//			}
		//			else if (entry.IsCoercedWithCurrentValue)
		//			{
		//				// store current-values
		//				StoreItemValue(ownerDO, item,
		//								dpIndex,
		//								new ModifiedItemValue(entry.ModifiedValue.CoercedValue, FullValueSource.IsCoercedWithCurrentValue));
		//			}
		//			else
		//			{
		//				ClearItemValue(ownerDO, item, dpIndex);
		//			}
		//		}
		//	}
		//}

		internal static void ClearItemValueStorage(DependencyObject owner)
		{
			ItemValueStorageField.ClearValue(owner);
		}

		internal static void ClearItemValueStorage(DependencyObject owner, BindableProperty[] dpIndices)
		{
			ClearItemValueStorageRecursive(ItemValueStorageField.GetValue(owner), dpIndices);
		}

		private static void ClearItemValueStorageRecursive(
			WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>> itemValueStorage, 
			BindableProperty[] dpIndices)
		{
			if (itemValueStorage != null)
			{
				foreach (List<KeyValuePair<BindableProperty, object>> itemValuesList in itemValueStorage.Values)
				{
					for (int i = 0; i < itemValuesList.Count; i++)
					{
						KeyValuePair<BindableProperty, object> itemValue = itemValuesList[i];
						//if (itemValue.Key == ItemValueStorageField.GlobalIndex)
						//{
						//	ClearItemValueStorageRecursive(
						//		(WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>>)itemValue.Value, dpIndices);
						//}

						for (int j = 0; j < dpIndices.Length; j++)
						{
							if (itemValue.Key == dpIndices[j])
							{
								itemValuesList.RemoveAt(i--);
								break;
							}
						}
					}
				}
			}
		}

		// ItemValueStorage.  For each data item it stores a list of (DP, value) pairs that we want to preserve on the container.
		private static readonly UncommonField<WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>>> ItemValueStorageField =
							new UncommonField<WeakDictionary<object, List<KeyValuePair<BindableProperty, object>>>>();

		// Since ItemValueStorage is private and only used for TreeView and Grouping virtualization we hardcode the DPs that we'll store in it.
		// If we make this available as a service to the rest of the platform we'd come up with some sort of DP registration mechanism.
		private static readonly BindableProperty[] ItemValueStorageIndices = new BindableProperty[] {
			//ItemValueStorageField,
			//TreeViewItem.IsExpandedProperty.GlobalIndex,
			//Expander.IsExpandedProperty.GlobalIndex,
			//GroupItem.DesiredPixelItemsSizeCorrectionFactorField.GlobalIndex,
			//VirtualizingStackPanel.ItemsHostInsetProperty.GlobalIndex
		};
	}
}

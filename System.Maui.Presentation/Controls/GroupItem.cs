using System;
using System.Collections.Generic;
using System.Text;

using System.Windows;

using System.Maui.Presentation.Controls.Primitives;

namespace System.Maui.Presentation.Controls
{
    public class GroupItem : ContentControl
	{
		//------------------------------------------------------
		//
		// Internal Properties
		//
		//------------------------------------------------------

		internal ItemContainerGenerator Generator
		{
			get { return _generator; }
			set { _generator = value; }
		}

		internal Panel ItemsHost
		{
			get
			{
				return _itemsHost;
			}
			set { _itemsHost = value; }
		}

		//------------------------------------------------------
		//
		// Private Fields
		//
		//------------------------------------------------------

		ItemContainerGenerator _generator;
		private Panel _itemsHost;
		FrameworkElement _header;
		//Expander _expander;

		internal static readonly UncommonField<bool> MustDisableVirtualizationField = new UncommonField<bool>();
		internal static readonly UncommonField<bool> InBackgroundLayoutField = new UncommonField<bool>();

		internal static readonly UncommonField<Thickness> DesiredPixelItemsSizeCorrectionFactorField = new UncommonField<Thickness>();

		internal static readonly UncommonField<HierarchicalVirtualizationConstraints> HierarchicalVirtualizationConstraintsField =
			new UncommonField<HierarchicalVirtualizationConstraints>();
		internal static readonly UncommonField<HierarchicalVirtualizationHeaderDesiredSizes> HierarchicalVirtualizationHeaderDesiredSizesField =
			new UncommonField<HierarchicalVirtualizationHeaderDesiredSizes>();
		internal static readonly UncommonField<HierarchicalVirtualizationItemDesiredSizes> HierarchicalVirtualizationItemDesiredSizesField =
			new UncommonField<HierarchicalVirtualizationItemDesiredSizes>();
	}
}

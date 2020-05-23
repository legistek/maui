// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
// Description: ItemsPresenter object - site of the panel for layout of groups or items.
//
// Specs:       Data Styling.mht
//

using System;
using System.Linq;
using System.Collections;
using System.Diagnostics;

using System.Maui.Presentation.Internals;
using System.Maui.Presentation.MS.Internal;

using FrameworkTemplate = System.Maui.DataTemplate;
using ItemsPanelTemplate = System.Maui.DataTemplate;
using DependencyObject = System.Maui.BindableObject;

namespace System.Maui.Presentation.Controls
{
	/// <summary>
	///     An ItemsPresenter marks the site (in a style) of the panel that controls
	///     layout of groups or items.
	/// </summary>	
	public class ItemsPresenter : FrameworkElement, ITemplateParent
	{
		//------------------------------------------------------
		//
		// Public Methods
		//
		//------------------------------------------------------

		/// <summary>
		/// Called when the Template's tree is about to be generated
		/// </summary>
		internal override void OnPreApplyTemplate()
		{
			base.OnPreApplyTemplate();
			AttachToOwner();
		}

		/// <summary>
		///     This is the virtual that sub-classes must override if they wish to get
		///     notified that the template tree has been created.
		/// </summary>
		protected override void OnApplyTemplate()
		{
			// verify that the template produced a panel with no children
			Panel panel = this.Children.FirstOrDefault() as Panel;
			if (panel == null || panel.Children?.Count > 0)
				throw new InvalidOperationException();

			OnPanelChanged(this, EventArgs.Empty);

			base.OnApplyTemplate();
		}

		//------------------------------------------------------
		//
		// Protected Methods
		//
		//------------------------------------------------------

		/// <summary>
		/// Override of <seealso cref="FrameworkElement.MeasureOverride" />.
		/// </summary>
		/// <param name="constraint">Constraint size is an "upper limit" that the return value should not exceed.</param>
		/// <returns>The ItemsPresenter's desired size.</returns>		
		protected override SizeRequest MeasureOverride(double widthConstraint, double heightConstraint)
		{
			return new SizeRequest(Helper.MeasureElementWithSingleChild(this, new Size(widthConstraint, heightConstraint)));
		}

		protected override void LayoutChildren(double x, double y, double width, double height)
		{
			Helper.ArrangeElementWithSingleChild(this, x, y, width, height);
		}

		//------------------------------------------------------
		//
		// Internal Properties
		//
		//------------------------------------------------------

		internal ItemsControl Owner
		{
			get { return _owner; }
		}

		internal ItemContainerGenerator Generator
		{
			get { return _generator; }
		}

		//// Internal Helper so the FrameworkElement could see this property
		//internal override FrameworkTemplate TemplateInternal
		//{
		//	get { return Template; }
		//}

		//// Internal Helper so the FrameworkElement could see the template cache
		//internal override FrameworkTemplate TemplateCache
		//{
		//	get { return _templateCache; }
		//	set { _templateCache = (ItemsPanelTemplate)value; }
		//}

		/// <summary>
		/// TemplateProperty
		/// </summary>
		internal static readonly BindableProperty TemplateProperty =
				BindableProperty.Create(
						"Template",
						typeof(DataTemplate),
						typeof(ItemsPresenter),
						null,
						propertyChanged: (obj, oldVal, newVal) =>
						{
							//ItemsPresenter ip = (ItemsPresenter)obj;
							//StyleHelper.UpdateTemplateCache(
							//	ip,
							//	(FrameworkTemplate)oldVal,
							//	(FrameworkTemplate)newVal,
							//	TemplateProperty);
						});
		
		/// <summary>
		/// Template Property
		/// </summary>
		private ItemsPanelTemplate Template
		{
			get { return _templateCache; }
			set { SetValue(TemplateProperty, value); }
		}

		// Internal helper so FrameworkElement could see call the template changed virtual
		internal 
			// override
			void OnTemplateChangedInternal(
			FrameworkTemplate oldTemplate, 
			FrameworkTemplate newTemplate)
		{
			OnTemplateChanged((ItemsPanelTemplate)oldTemplate, (ItemsPanelTemplate)newTemplate);
		}

		/// <summary>
		///     Template has changed
		/// </summary>
		/// <remarks>
		///     When a Template changes, the VisualTree is removed. The new Template's
		///     VisualTree will be created when ApplyTemplate is called
		/// </remarks>
		/// <param name="oldTemplate">The old Template</param>
		/// <param name="newTemplate">The new Template</param>
		protected virtual void OnTemplateChanged(ItemsPanelTemplate oldTemplate, ItemsPanelTemplate newTemplate)
		{
		}

		//------------------------------------------------------
		//
		// Internal Methods
		//
		//------------------------------------------------------

		internal static ItemsPresenter FromPanel(Layout panel)
		{
			if (panel == null)
				return null;
			return VisualTreeHelper.FindParent(panel, elem => elem is ItemsPresenter) 
				as ItemsPresenter;
		}

		internal static ItemsPresenter FromGroupItem(GroupItem groupItem)
		{
			if (groupItem == null)
				return null;

			VisualElement parent = VisualTreeHelper.GetParent(groupItem) as VisualElement;
			if (parent == null)
				return null;

			return VisualTreeHelper.GetParent(parent) as ItemsPresenter;
		}

		internal override void OnAncestorChanged()
		{
			if (TemplatedParent == null)
			{
				UseGenerator(null);
				ClearPanel();
			}

			base.OnAncestorChanged();
		}


		//------------------------------------------------------
		//
		// Private Methods
		//
		//------------------------------------------------------

		// initialize (called during measure, from ApplyTemplate)
		void AttachToOwner()
		{
			DependencyObject templatedParent = this.TemplatedParent as DependencyObject;
			ItemsControl owner = templatedParent as ItemsControl;
			ItemContainerGenerator generator;

			if (owner != null)
			{
				// top-level presenter - get information from ItemsControl
				generator = owner.ItemContainerGenerator;
			}
			else
			{
				// subgroup presenter - get information from GroupItem
				GroupItem parentGI = templatedParent as GroupItem;
				ItemsPresenter parentIP = FromGroupItem(parentGI);

				if (parentIP != null)
					owner = parentIP.Owner;

				generator = (parentGI != null) ? parentGI.Generator : null;
			}

			_owner = owner;
			UseGenerator(generator);

			// create the panel, based either on ItemsControl.ItemsPanel or GroupStyle.Panel
			ItemsPanelTemplate template = null;
			GroupStyle groupStyle = (_generator != null) ? _generator.GroupStyle : null;
			if (groupStyle != null)
			{
				// If GroupStyle.Panel is set then we dont honor ItemsControl.IsVirtualizing
				template = groupStyle.Panel;
				if (template == null)
				{
					// create default Panels
					if (VirtualizingPanel.GetIsVirtualizingWhenGrouping(owner))
					{
						template = GroupStyle.DefaultVirtualizingStackPanel;
					}
					else
					{
						template = GroupStyle.DefaultStackPanel;
					}
				}
			}
			else
			{
				// Its a leaf-level ItemsPresenter, therefore pick ItemsControl.ItemsPanel
				template = (_owner != null) ? _owner.ItemsPanel : null;
			}
			Template = template;
		}

		void UseGenerator(ItemContainerGenerator generator)
		{
			if (generator == _generator)
				return;

			if (_generator != null)
				_generator.PanelChanged -= new EventHandler(OnPanelChanged);

			_generator = generator;

			if (_generator != null)
				_generator.PanelChanged += new EventHandler(OnPanelChanged);
		}

		private void OnPanelChanged(object sender, EventArgs e)
		{
			// something has changed that affects the ItemsPresenter.
			// Re-measure.  This will recalculate everything from scratch.
			InvalidateMeasure();

			//
			// If we're under a ScrollViewer then its ScrollContentPresenter needs to
			// be updated to work with the new panel.
			//

			// TODO
			//ScrollViewer parent = Parent as ScrollViewer;
			//if (parent != null)
			//{
			//	// If our logical parent is a ScrollViewer then the visual parent is a ScrollContentPresenter.
			//	ScrollContentPresenter scp = VisualTreeHelper.GetParent(this) as ScrollContentPresenter;

			//	if (scp != null)
			//	{
			//		scp.HookupScrollingComponents();
			//	}
			//}
		}

		// workaround, pending bug 953483.  The panel is
		// being removed from the tree, so it should release
		// its resources (chiefly - stop listening for generator's
		// ItemsChanged event).  Until there's a mechanism for
		// this, just mark the panel as a non-ItemsHost, so
		// that the next time it gets ItemsChanged it will
		// stop listening.  (See also bug 942265)
		private void ClearPanel()
		{
			Panel oldPanel = this.Children?.FirstOrDefault() as Panel;
			Type type = null;

			//if (Template != null)
			//{
			//	// Get the type of the template content's root

			//	// Is this a FEF-based template?
			//	if (Template.VisualTree != null)
			//	{
			//		type = Template.VisualTree.Type;
			//	}

			//	// Or, is it a (non-empty) Baml-based template?
			//	else if (Template.HasXamlNodeContent)
			//	{
			//		System.Xaml.XamlType xType = (Template.Template as TemplateContent).RootType;
			//		type = xType.UnderlyingType;
			//	}
			//}

			if (oldPanel != null && oldPanel.GetType() == type)
			{
				oldPanel.IsItemsHost = false;
			}
		}

		//------------------------------------------------------
		//
		// Private Fields
		//
		//------------------------------------------------------

		ItemsControl _owner;
		ItemContainerGenerator _generator;
		ItemsPanelTemplate _templateCache;
	}
}
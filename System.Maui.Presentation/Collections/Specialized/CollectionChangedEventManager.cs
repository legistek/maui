using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace System.Collections.Specialized
{
	public class CollectionChangedEventManager : WeakEventManager
	{
		/// <summary>
		/// Add a handler for the given source's event.
		/// </summary>
		public static void AddHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");

			source.CollectionChanged += (sender, e) => handler(sender, e);
		}

		/// <summary>
		/// Remove a handler for the given source's event.
		/// </summary>
		public static void RemoveHandler(INotifyCollectionChanged source, EventHandler<NotifyCollectionChangedEventArgs> handler)
		{
			if (handler == null)
				throw new ArgumentNullException("handler");

			source.CollectionChanged -= (sender, e) => handler(sender, e);
		}
	}
}

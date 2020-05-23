using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;

using System.Maui;

namespace System.Windows
{
    internal class UncommonField<T>
    {
		ConcurrentDictionary<BindableObject, object> _values;
		T _defaultValue;
		internal static object UnsetValue = new object();

		public UncommonField() : this(default(T))
		{
		}

		public UncommonField(T defaultValue)
		{
			_defaultValue = defaultValue;
		}

		public void SetValue(BindableObject instance, T value)
		{
			_values[instance] = value;
		}

		public T GetValue(BindableObject instance)
		{
			_values.TryGetValue(instance, out object value);
			if (value is T)
				return (T)value;
			return default(T);
		}

		public void ClearValue(BindableObject instance)
		{
			_values[instance] = UnsetValue;
		}

		public int GlobalIndex => GetHashCode();
	}
}

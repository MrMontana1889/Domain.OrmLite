// DictionaryLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Support;
using System;
using System.Collections.Generic;

namespace OrmLite.Support.Library
{
	/// <summary>
	/// Static utility methods for working with dictionaries and hashtables where System.Type is not involved (see TypeLibrary).
	/// </summary>
	public static class DictionaryLibrary
	{
		#region Public Static Methods
		/// <summary>
		/// Returns a new IDictionary<TKey, TValue> implementation based on the requested capcity.
		/// Prefer using this method as apposed to instantiating Dictionary<TKey, TValue> instances directly for short lived objects.
		/// This method will construct ChunkyDictionary<TKey, TValue> instances if required to minimize objects on the large object heap (LOH).
		/// </summary>
		/// <param name="capacity">Initial capacity for the dictionary. Set to the maximum size required if possible.</param>
		static public IDictionary<TKey, TValue> NewDictionary<TKey, TValue>(Int32 capacity)
		{
			return new Dictionary<TKey, TValue>(capacity);
		}
		#endregion
	}
}

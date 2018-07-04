// Interfaces.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Units;
using System;
using System.Collections;
using System.Collections.Generic;

namespace OrmLite.Support.Support
{
	#region INamable/ILabeled
	public interface IEditLabeled : ILabeled
	{
		/// <summary>
		/// Gets or sets the label property.
		/// </summary>
		new string Label { get; set; }
	}

	public interface ILabeled
	{
		/// <summary>
		/// Return a localized, human-readable description.
		/// </summary>
		string Label { get; }
	}

	/// <summary>
	/// Interface describing an object that supports fundamental
	/// Haestad naming and labeling conventions.
	/// </summary>
	public interface INamable
	{
		/// <summary>
		/// Return an internal, non-numeric identifier for the object.
		/// Never translated. Expected to be unique within some context.
		/// Generally starts lower-case.
		/// </summary>
		string Name { get; }
	}
	#endregion

	#region FieldTypes/Fields

	public interface IFieldType : INamable, IEditLabeled
	{
		FieldDataType FieldDataType { get; }
		string Category { get; set; }
		object DefaultValue { get; }
		string Notes { get; set; }
		Type Type { get; }
		string FormatterName { get; }
		Unit StorageUnit { get; }
	}

	public interface IField : INamable, IEditLabeled
	{
		string Notes { get; set; }
		string Category { get; set; }
		FieldDataType FieldDataType { get; }

		object GetValue(int id);
		IDictionary<int, object> GetValues();
		IDictionary<int, object> GetValues(IEnumerable<int> ids);
	}

	public interface IEditField : IField
	{
		object DefaultValue { get; }

		void SetValue(int id, object value);
		void SetValues(SetValuesOperation operation, object value);
	}

	public interface IUnitizedField : IField
	{
		UnitConversionManager.UnitIndex StorageUnitIndex { get; }
		UnitConversionManager.UnitIndex WorkingUnitIndex { get; set; }

		double GetDoubleValue(int id);
	}

	#endregion

	#region ISet

	/// <summary>
	/// Interface for objects that behave as sets.
	/// </summary>
	public interface ISet : ICollection
	{
		bool Add(object aobject);
		bool AddAll(ICollection aicollection);
		void Clear();
		bool Contains(object aobject);
		bool Remove(object aobject);
		bool RemoveAll(ICollection aicollection);
	}

	#endregion
}

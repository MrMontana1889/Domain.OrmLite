// Enumerations.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

namespace OrmLite.Support.Support
{
	public enum FieldDataType
	{
		Integer = 1,
		Real = 2,
		Text = 3,
		LongText = 4,
		DateTime = 5,
		Boolean = 6,
		LongBinary = 7,
		Referenced = 8,
		Collection = 9,
		Enumerated = 10,
	}

	public enum SetValuesOperation
	{
		Add = 1,
		Divide = 2,
		Multiply = 3,
		Set = 4,
		Subtract = 5
	}

	/// <summary>
	/// HMI-standard enumeration of the common system types, for use in 
	/// _HmiTypeDefinition tables and elsewhere. Existing values cannot be
	/// changed at the risk of breaking existing databases! Dates from
	/// framework 1.x/2.x -- possibly obsolete.
	/// </summary>
	public enum SystemType
	{
		Bool = 1,
		Byte = 2,
		Char = 3,
		DateTime = 4,
		Decimal = 5,
		Delegate = 6,
		Double = 7,
		Int = 8,
		Long = 9,
		Object = 10,
		SelectionSet = 11,
		String = 12,
		Guid = 13
	}
}

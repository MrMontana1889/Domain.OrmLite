// StringLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;

namespace OrmLite.Resource.Library
{
	public static class ResourceStringLibrary
	{
		static public int DigitValue(char achar)
		{
			// DO NOT LOCALIZE <ccla>
			if (achar > 47 && achar < 58)
				return achar - 48;
			if (achar > 64 && achar < 91)
				return achar - 55;
			throw new Exception("CharLibrary.DigitValue"); // DO NOT LOCALIZE <ccla>
		}

		/// <summary>
		/// Return a boolean whether passed string is null, empty, or only spaces.
		/// </summary>
		static public bool IsBlank(String astring)
		{
			if (astring == null) return true;
			// Fast implementation that produces no garbage.
			int aintSize = astring.Length;
			int aintStart = 0;
			while (aintStart < aintSize && Char.IsWhiteSpace(astring[aintStart]))
				aintStart++;
			return aintStart == aintSize;
		}
	}
}

// NumericFormatterLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Units;

namespace OrmLite.Support.Library
{
    public static class NumericFormatterLibrary
    {
        #region Public Methods
        public static string GetNumericFormatterName(StandardNumericFormatters numericFormatter)
        {
            return numericFormatter.ToString().ToLowerInvariant();
        }
        #endregion
    }
}

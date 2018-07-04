// StandardFormatters.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Library;
using System;

namespace OrmLite.Support.Units
{
    public static class StandardFormatters
    {
        // C

        #region CurrencyFormatter
        static public NumericFormatter CurrencyFormatter()
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.Currency,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.Currency),
                Unit.Dollars,
                StandardFormat,
                2,
                Unit.Dollars,
                Unit.Dollars);
        }
        static public NumericFormatter CurrencyFormatter(Unit us, Unit si)
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.Currency,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.Currency),
                us,
                StandardFormat,
                2,
                si,
                us);
        }
        #endregion

        // N

        #region None
        static public NumericFormatter None()
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.None,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.None),
                Unit.None,
                StandardFormat,
                0,
                Unit.None,
                Unit.None);
        }
        static public NumericFormatter None(Unit us, Unit si)
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.None,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.None),
                us,
                StandardFormat,
                0,
                si,
                us);
        }
        #endregion

        #region Number
        static public NumericFormatter Number()
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.Number,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.Number),
                Unit.None,
                StandardFormat,
                0,
                Unit.None,
                Unit.None);
        }
        static public NumericFormatter Number(Unit us, Unit si)
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.Number,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.Number),
                us,
                StandardFormat,
                0,
                si,
                us);
        }
        #endregion

        // P

        #region Percent
        static public NumericFormatter Percent()
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.Percent,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.Percent),
                Unit.PercentPercent,
                StandardFormat,
                1,
                Unit.PercentPercent,
                Unit.PercentPercent);
        }
        static public NumericFormatter Percent(Unit us, Unit si)
        {
            return new NumericFormatter(
                (int)StandardNumericFormatters.Percent,
                NumericFormatterLibrary.GetNumericFormatterName(StandardNumericFormatters.Percent),
                us,
                StandardFormat,
                1,
                si,
                us);
        }
        #endregion

        #region Constants
        static private readonly String StandardFormat = "n"; // DO NOT LOCALIZE <ccla>
        #endregion
    }
}

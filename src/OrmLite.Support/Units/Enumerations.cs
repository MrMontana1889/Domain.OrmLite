// Enumerations.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

namespace OrmLite.Support.Units
{
    /// <summary>
    /// The dimension and unit enumerations here originated in GEMS, however not all
    /// the GEMS enumerations may be here, and enumerations may have been added here
    /// that are not in GEMS. These enumerations were added after the original Unit
    /// namespace implementation. Their utility and correct usage are unclear. Prefer
    /// the original, more object-oriented unit mechanisms where possible.
    /// </summary>
    public enum DimensionType
    {
        Dimensionless = 0,
        Percentage = 5,
        Currency = 26,
        Unitless = 35,
    }

    public enum PercentUnit
    {
        Percent = 1,
        Unitless = 2
    }

    public enum CurrencyUnit
    {
        BaseCurrency = 1,
        KiloCurrency = 2,
        MegaCurrency = 3,
        GigaCurrency = 4,
    }

    public enum StandardNumericFormatters
    {
        None,
        Percent,
        Currency,
        Number,
        Unitless,
    }
}

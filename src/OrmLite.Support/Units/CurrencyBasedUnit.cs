// CurrencyBasedUnit.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Units which include currency symbols are handled slightly differently then standard units
    /// </summary>
    public class CurrencyBasedUnit : Unit
    {
        #region Construction
        public CurrencyBasedUnit(String astringName, Dimension adimension, UnitSystem aunitsystem, double adouble, int aintEnumValue) :
            base(astringName, adimension, aunitsystem, adouble, aintEnumValue)
        {

        }

        public CurrencyBasedUnit(String astringName, Dimension adimension, UnitSystem aunitsystem, double adouble, int aintEnumValue, String bentleyName) :
            base(astringName, adimension, aunitsystem, adouble, aintEnumValue, bentleyName)
        {

        }

        public CurrencyBasedUnit(String astringName, Dimension adimension, UnitSystem aunitsystem, IUnitConverter aiunitconverter, int aintEnumValue) :
            base(astringName, adimension, aunitsystem, aiunitconverter, aintEnumValue)
        {

        }

        public CurrencyBasedUnit(String astringName, Dimension adimension, UnitSystem aunitsystem, IUnitConverter aiunitconverter, int aintEnumValue, String bentleyName) :
            base(astringName, adimension, aunitsystem, aiunitconverter, aintEnumValue, bentleyName)
        {
        }
        #endregion

        #region Private Properties
        protected override string[] Parameters
        {
            get
            {
                return new String[] { NumberFormatInfoLibrary.Current.CurrentNumberFormatInfo.CurrencySymbol };
            }
        }
        #endregion
    }
}

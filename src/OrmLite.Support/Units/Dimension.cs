// Dimension.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Resource.Support;
using OrmLite.Support.Support;
using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Instances group together a type of unit, all of which
    /// are convertible between each other via a base unit.
    /// </summary>
    public class Dimension : INamable
    {
        #region Static Public Convenience Methods for Accessing Dimension Singletons

        // TODO 1: <whar> Change these *back* to alpha order for easier perusal.
        static public Dimension Currency { get { return UnitConversionManager.Current.DimensionAt(UnitConversionManager.DimensionIndex.Currency); } }
        static public Dimension None { get { return UnitConversionManager.Current.DimensionAt(UnitConversionManager.DimensionIndex.None); } }
        static public Dimension Percent { get { return UnitConversionManager.Current.DimensionAt(UnitConversionManager.DimensionIndex.Percent); } }
        static public Dimension Unitless { get { return UnitConversionManager.Current.DimensionAt(UnitConversionManager.DimensionIndex.Unitless); } }
        #endregion


        #region Static Public Methods

        static public Dimension FromIndex(UnitConversionManager.DimensionIndex aindex)
        {
            return UnitConversionManager.Current.DimensionAt(aindex);
        }

        static public Dimension FromEnum(int aint)
        {
            switch (aint)
            {
                // These values per GEMS hmiUnitLib.idl 8/7/2002, 34 dimensions.
                case 0: return Dimension.None;
                case 5: return Dimension.Percent;
                // TODO 9 : Currency provided by .Net framework?
                case 26: return Dimension.Currency;
                case 35: return Dimension.Unitless;
                // These values reserved by .NET framework.
                case 101: return Dimension.Currency;                    //TODO 7: redundant with case 26 above?
                                                                        // Note: <whar> Somehow I don't think all these values were reserved by .NET; just Currency.
                                                                        // We should have kept adding from 36, not 102. Ah well...

                // Appears that a lookup or something else of that sort is being built, so we don't want this behavior to change
                // so using invariant culture.
                default:
                    throw new Exception("Dimension.FromEnum: " + aint.ToString(CultureInfo.InvariantCulture)); // DO NOT LOCALIZE <ccla>
            }
        }

        // TODO 1: <whar> Change these to alpha order for easier perusal.
        static public Dimension FromEnum(DimensionType dimension)
        {
            switch (dimension)
            {
                case DimensionType.Dimensionless: return Dimension.None;
                case DimensionType.Percentage: return Dimension.Percent;
                case DimensionType.Currency: return Dimension.Currency;
                case DimensionType.Unitless: return Dimension.Unitless;
                default:
                    // Appears that a lookup or something else of that sort is being built, so we don't want this behavior to change
                    // so using invariant culture.
                    throw new Exception("Dimension.FromEnum: " + dimension.ToString()); // DO NOT LOCALIZE <ccla>
            }
        }

        /// <summary>
        /// Lookup dimension singleton by name. Not indexed. Use only when performance not critical.
        /// </summary>
        static public Dimension FromName(String asNameDimension)
        {
            foreach (Dimension adimension in UnitConversionManager.Current.AvailableDimensions())
                if (adimension.Name.Equals(asNameDimension))
                    return adimension;
            return null;
        }

        static public double Convert(double adouble, DimensionType dimensionType, int aintFromUnit, int aintToUnit)
        {
            Dimension dimension = Dimension.FromEnum(dimensionType);
            return ((Unit)dimension.Enum2Unit[aintToUnit]).ConvertFrom(adouble, (Unit)dimension.Enum2Unit[aintFromUnit]);
        }

        #endregion

        #region Constructors

        public Dimension(String astringName, int aintEnumValue) :
            this(astringName, aintEnumValue, String.Empty)
        {
        }

        public Dimension(String astringName, int aintEnumValue, String bentleyName)
        {
            m_stringName = astringName;
            m_intEnumValue = aintEnumValue;
            m_bentleyName = bentleyName;
        }

        #endregion

        #region Public Properties

        public IList AvailableUnitsSorted
        {
            get
            {
                SortedList asortedlistUnit = new SortedList();
                foreach (Unit aunit in Units)
                {
                    if (!asortedlistUnit.ContainsKey(aunit.Label))
                        asortedlistUnit.Add(aunit.Label, aunit);
                    else
                        asortedlistUnit.Add(aunit.Label + " ", aunit); // i18n ok <abar>
                }
                return asortedlistUnit.GetValueList();
            }
        }

        /// <summary>
        /// Return the GEMS-style enum value. Not the same as DimensionIndex.
        /// </summary>
        public int EnumValue
        {
            get { return m_intEnumValue; }
        }

        public String Label
        {
            get { return TextManager.Current[LabelKey]; }
        }

        public String LabelKey
        {
            get { return Name; }
        }

        public String Name
        {
            get { return m_stringName; }
        }

        public String BentleyName
        {
            get { return m_bentleyName; }
        }

        public String ShortLabel
        {
            get { return TextManager.Current.GetShortString(LabelKey); }
        }

        public ISet Units
        {
            get { return m_isetUnit; }
        }

        #endregion

        #region Public Methods

        public IList AvailableUnitsSortedEx(int currentUnit)
        {
            SortedList asortedlistUnit = new SortedList();
            // add the currently selected unit first
            foreach (Unit aunit in Units)
            {
                if (aunit.EnumValue == currentUnit)
                {
                    asortedlistUnit.Add(aunit.Label, aunit);
                    break;
                }
            }
            // add remaining units, ignore duplicates
            foreach (Unit aunit in Units)
            {
                if (!asortedlistUnit.ContainsKey(aunit.Label))
                    asortedlistUnit.Add(aunit.Label, aunit);
            }
            return asortedlistUnit.GetValueList();
        }

        public double Convert(double adouble, int aintFromUnit, int aintToUnit)
        {
            return ((Unit)Enum2Unit[aintToUnit]).ConvertFrom(adouble, (Unit)Enum2Unit[aintFromUnit]);
        }

        public Unit UnitFromEnum(int aiEnumValue)
        {
            return (Unit)Enum2Unit[aiEnumValue];
        }

        #endregion

        #region Public Constants

        public const String XSDDIMENSIONTAG = "HMIDATA:DIMENSION";
        public const String XSDDIMENSIONENUMTAG = "HMIDATA:DIMENSIONENUM";

        #endregion

        #region Internal Methods

        internal void Register(Unit aunit)
        {
            Units.Add(aunit);
            Debug.Assert(!Enum2Unit.Contains(aunit.EnumValue));     // Should be no redundant enums or registrations.
            Enum2Unit.Add(aunit.EnumValue, aunit);
        }

        #endregion

        #region Private Methods

        private IDictionary Enum2Unit
        {
            get { return m_idictionaryEnum2Unit; }
        }

        #endregion

        #region Private Fields

        private int m_intEnumValue;
        private String m_stringName;
        private String m_bentleyName;
        private ISet m_isetUnit = new Hashset();
        private IDictionary m_idictionaryEnum2Unit = new Hashtable();

        #endregion
    }
}

// Unit.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Resource.Support;
using OrmLite.Support.Support;
using System;
using System.Globalization;

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Singleton instances represent a specific unit with behavior for managing labels and finding 
    /// conversion factors. Serves as facade to unit conversion namespace. To add a new unit:
    /// 
    ///		1) Add it to the *end* of the UnitIndex enum in UnitConversionManager.cs: existing indexes cannot be changed.
    ///		2) Add it to the *end* of UnitConversionManager.InitializeUnits(). Be sure to assign a unique enum in the constructor, the same value as in GEMS if it has the unit.
    ///		3) Add it to the appropriate enumeration in Enumerations.cs, using the same value as in 2) above. (Though it is unclear if and how those enumerations are used.)
    ///		4) Add it to the "Static Public Convenience Methods" here in Unit.cs
    ///		5) Add the text string to Haestad.Support.Resources.HaestadStrings.resx, using the unit name as key.
    ///		6) Add a test to UnitTestCase: unit conversions are very important, and easy to overlook: we must be careful!
    ///		
    /// </summary>
    public class Unit : INamable
    {
        #region Static Public Convenience Methods for Accessing Unit Singletons

        static public Unit Dollars { get { return UnitConversionManager.Current.UnitAt(UnitConversionManager.UnitIndex.Dollars); } }
        static public Unit None { get { return UnitConversionManager.Current.UnitAt(UnitConversionManager.UnitIndex.None); } }
        static public Unit PercentPercent { get { return UnitConversionManager.Current.UnitAt(UnitConversionManager.UnitIndex.PercentPercent); } }
        static public Unit UnitlessPercent { get { return UnitConversionManager.Current.UnitAt(UnitConversionManager.UnitIndex.UnitlessPercent); } }
        static public Unit UnitlessUnit { get { return UnitConversionManager.Current.UnitAt(UnitConversionManager.UnitIndex.UnitlessUnit); } }
        #endregion

        #region Static Public Methods

        /// Lookup unit from external (database) enum. 
        static public Unit FromDimensionEnum(Dimension adimension, int aint)
        {
            Unit aunit = adimension.UnitFromEnum(aint);

            if (aunit == null)
                throw new Exception(TextManager.Current["hsuException_UnitFromDimensionEnumNameString", adimension.Name, aint.ToString(CultureInfo.InvariantCulture)]);

            return aunit;
        }

        /// <summary>
        /// Lookup unit from dimension and unit name. Inefficient linear search.
        /// </summary>
        static public Unit FromDimensionName(Dimension adimension, String asNameUnit)
        {
            foreach (Unit aunit in adimension.Units)
                if (aunit.Name.Equals(asNameUnit))
                    return aunit;
            return null;
        }

        /// <summary>
        /// Lookup unit from dimension and unit label. Inefficient linear search.
        /// </summary>
        static public Unit FromLabel(Dimension adimension, String aUnitLabel)
        {
            foreach (Unit aunit in adimension.Units)
                if (aunit.Label.Equals(aUnitLabel))
                    return aunit;
            return null;
        }

        static public Unit FromIndex(UnitConversionManager.UnitIndex aindex)
        {
            return UnitConversionManager.Current.UnitAt(aindex);
        }

        static public Unit FromSerializedString(String astring)
        {
            string[] aarray = astring.Split(new char[] { ':' });
            return FromDimensionName(Dimension.FromName(aarray[0].Trim()), aarray[1].Trim());
        }

        #endregion

        #region Constructors

        public Unit(String astringName, Dimension adimension, UnitSystem aunitsystem, double adouble, int aintEnumValue) :
            this(astringName, adimension, aunitsystem, adouble, aintEnumValue, String.Empty)
        {
        }

        public Unit(String astringName, Dimension adimension, UnitSystem aunitsystem, double adouble, int aintEnumValue, String bentleyName)
        {
            Initialize(astringName, adimension, aunitsystem, new FactorConverter(adouble), aintEnumValue, bentleyName);
        }

        public Unit(String astringName, Dimension adimension, UnitSystem aunitsystem, IUnitConverter aiunitconverter, int aintEnumValue) :
            this(astringName, adimension, aunitsystem, aiunitconverter, aintEnumValue, String.Empty)
        {
        }

        public Unit(String astringName, Dimension adimension, UnitSystem aunitsystem, IUnitConverter aiunitconverter, int aintEnumValue, String bentleyName)
        {
            Initialize(astringName, adimension, aunitsystem, aiunitconverter, aintEnumValue, bentleyName);
        }

        #endregion

        #region Public Properties

        public Dimension Dimension
        {
            get { return m_dimension; }
        }

        /// <summary>
        /// Return the GEMS-style enum value for this unit. These are unique only within a given
        /// dimension. This is *not* the same as a UnitIndex.
        /// </summary>
        public int EnumValue
        {
            get { return m_intEnumValue; }
        }

        /// <summary>
        /// External, globalized user-readable label for the unit.
        /// </summary>
        public virtual String Label
        {
            get
            {
                if (m_stringLabel == null)
                    m_stringLabel = TextManager.Current[LabelKey, Parameters];
                return m_stringLabel;
            }
        }

        public String LabelKey
        {
            get { return Name; }
        }

        /// <summary>
        /// Internal, US-developer-readable constant identifier for the unit.
        /// </summary>
        public String Name
        {
            get { return m_stringName; }
        }

        public String BentleyName
        {
            get { return m_bentleyName; }
        }

        public virtual String ShortLabel
        {
            get { return TextManager.Current.GetShortString(LabelKey, Parameters); }
        }

        public UnitSystem UnitSystem
        {
            get { return m_unitsystem; }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Convert passed number in passed unit to a number in this
        /// object's unit.
        /// </summary>
        public double ConvertFrom(double adouble, Unit aunit)
        {
            if (aunit == this) return adouble;
            if (aunit.Dimension != this.Dimension)
                throw new ConversionException(TextManager.Current["hsuConversionException_IncompatibleDimensionsDimensionDimension",
                    aunit.Dimension.Label, this.Dimension.Label]);

            return this.m_iunitconverter.FromBaseUnit(
                aunit.m_iunitconverter.ToBaseUnit(adouble));
        }

        public double ConversionFactor(Unit aunit)
        {
            return 1.0 / ConvertFrom(1.0, aunit);
        }

        /// <summary>
        /// Answer a string suitable for serializing all of this Unit's state
        /// to human-readable formats like XML. Note this method allows persisting
        /// internal Dimension and Unit names, meaning they can no longer change!
        /// Could easily persist enums instead, but why bother using a 
        /// human-readable format if filling it with unreadable enums? Another 
        /// option is to persist enum names (DimensionType, Unit enumerations).
        /// </summary>
        public String ToSerializedString()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", Dimension.Name, Name);
        }

        //TODO 1: better way to control displayed text in ComboBox?
        public override String ToString()
        {
            return Label;
        }

        #endregion

        #region Private Methods

        private void Initialize(String astringLabel, Dimension adimension,
            UnitSystem aunitsystem, IUnitConverter aiunitconverter, int aintEnumValue, String bentleyName)
        {
            m_stringName = astringLabel;
            m_iunitconverter = aiunitconverter;
            m_dimension = adimension;
            m_intEnumValue = aintEnumValue;
            m_dimension.Register(this);
            m_unitsystem = aunitsystem;
            m_bentleyName = bentleyName;
        }

        private Unit()
        {
        }

        #endregion

        #region Public Constants

        public const String XSDSTORAGEUNITTAG = "HMIDATA:STORAGEUNIT";
        public const String XSDSTORAGEUNITENUMTAG = "HMIDATA:STORAGEUNITENUM";

        #endregion

        #region Protected Properties
        protected IUnitConverter UnitConverter
        {
            get { return m_iunitconverter; }
        }
        protected virtual String[] Parameters
        {
            get { return new String[] { }; }
        }
        #endregion

        #region Private Fields

        private IUnitConverter m_iunitconverter;
        private Dimension m_dimension;
        private String m_stringName;
        private String m_stringLabel;
        private String m_bentleyName;
        private UnitSystem m_unitsystem;
        private int m_intEnumValue;

        #endregion

        #region ConversionException

        public class ConversionException : ApplicationException
        {
            public ConversionException(String asMessage) :
                base(asMessage)
            {
            }
        }

        #endregion

        #region FactorConverter

        private class FactorConverter : IUnitConverter
        {
            public FactorConverter(double adouble)
            {
                m_doubleFactor = adouble;
            }

            public double FromBaseUnit(double adouble)
            {
                return adouble * m_doubleFactor;
            }

            public double ToBaseUnit(double adouble)
            {
                return adouble / m_doubleFactor;
            }

            private double m_doubleFactor;
        }

        #endregion
    }
}

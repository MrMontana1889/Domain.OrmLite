// NumericFormatter.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Resource.Library;
using OrmLite.Resource.Support;
using OrmLite.Support.Library;
using OrmLite.Support.Support;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Xml.Serialization;

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Instances are used for mapping numbers back
    /// and forth to strings for display and editing.
    /// </summary>
    public class NumericFormatter : INamable, ILabeled
    {
        #region Constructors

        /// <summary>
        /// Default public constructor required for XmlSerializer.
        /// </summary>
        public NumericFormatter()
        {
        }

        /// <summary>
        /// Construct formatter without default display units.
        /// </summary>
        public NumericFormatter(int aintId, String asName, Unit aunit, String asFormatCode, int aintDecimalDigits) :
            this(aintId, asName, aunit, asFormatCode, aintDecimalDigits, null, null, true, String.Empty)
        {
        }

        /// <summary>
        /// Construct formatter without default display units.
        /// </summary>
        public NumericFormatter(int aintId, String asName, Unit aunit, String asFormatCode, int aintDecimalDigits,
            bool aboolIsStandardFormatter, String astringLabel) :
            this(aintId, asName, aunit, asFormatCode, aintDecimalDigits, null, null, aboolIsStandardFormatter, astringLabel)
        {
        }

        /// <summary>
        /// Construct formatter using default US display unit as the active display unit.
        /// </summary>
        public NumericFormatter(int aintId, String asName, String asFormatCode, int aintDecimalDigits,
            Unit aunitDisplayDefaultSi, Unit aunitDisplayDefaultUs) :
            this(aintId, asName, aunitDisplayDefaultUs, asFormatCode, aintDecimalDigits,
            aunitDisplayDefaultSi, aunitDisplayDefaultUs, true, String.Empty)
        {
        }

        /// <summary>
        /// Construct formatter using default US display unit as the active display unit.
        /// </summary>
        public NumericFormatter(int aintId, String asName, String asFormatCode, int aintDecimalDigits,
            Unit aunitDisplayDefaultSi, Unit aunitDisplayDefaultUs, bool aboolIsStandardFormatter, String astringLabel) :
            this(aintId, asName, aunitDisplayDefaultUs, asFormatCode, aintDecimalDigits, aunitDisplayDefaultSi,
            aunitDisplayDefaultUs, aboolIsStandardFormatter, astringLabel)
        {
        }

        /// <summary>
        /// Construct formatter using full state.
        /// </summary>
        public NumericFormatter(int aintId, String asName, Unit aunit, String asFormatCode, int aintDecimalDigits,
            Unit aunitDisplayDefaultSi, Unit aunitDisplayDefaultUs)
            : this(aintId, asName, aunit, asFormatCode, aintDecimalDigits, aunitDisplayDefaultSi, aunitDisplayDefaultUs,
            true, String.Empty)
        {
        }

        /// <summary>
        /// Construct formatter using full state.
        /// </summary>
        public NumericFormatter(int aintId, String asName, Unit aunit, String asFormatCode, int aintDecimalDigits,
            Unit aunitDisplayDefaultSi, Unit aunitDisplayDefaultUs, bool aboolIsStandardFormatter, String astringLabel)
        {
            m_intId = aintId;
            m_stringName = asName;
            m_unitDisplay = aunit;
            m_stringFormatCode = asFormatCode;
            m_intDecimalDigits = aintDecimalDigits;
            m_unitDisplayDefaultSi = aunitDisplayDefaultSi;
            m_unitDisplayDefaultUs = aunitDisplayDefaultUs;
            m_boolIsStandardFormatter = aboolIsStandardFormatter;
            m_stringLabel = astringLabel;

            Debug.Assert(!ResourceStringLibrary.IsBlank(m_stringName));
            Debug.Assert(!ResourceStringLibrary.IsBlank(m_stringFormatCode));
            Debug.Assert(m_unitDisplay != null);
            Debug.Assert(m_unitDisplayDefaultSi == null || m_unitDisplayDefaultSi.Dimension == m_unitDisplay.Dimension);
            Debug.Assert(m_unitDisplayDefaultUs == null || m_unitDisplayDefaultUs.Dimension == m_unitDisplay.Dimension);
        }

        #endregion

        #region Public Events

        public event EventHandler DecimalDigitsChanged;
        public event EventHandler DisplayUnitChanged;
        public event EventHandler FormatCodeChanged;

        #endregion

        #region Public Properties

        public int DecimalDigits
        {
            get { return m_intDecimalDigits; }
            set
            {
                if (m_intDecimalDigits == value) return;
                if (value >= 0 && value <= MAXDECIMALDIGITS)
                {
                    m_intDecimalDigits = value;
                    OnDecimalDigitsChanged(EventArgs.Empty);
                }
                // TODO2: Throw an exception if value < 0.
            }
        }

        [XmlIgnore()]
        public Dimension Dimension
        {
            get { return DisplayUnit.Dimension; }
        }

        [XmlIgnore()]
        public Unit DisplayUnit
        {
            get { return m_unitDisplay; }
            set
            {
                if (m_unitDisplay == value) return;
                m_unitDisplay = value;
                OnDisplayUnitChanged(EventArgs.Empty);
            }
        }

        public String DisplayUnitLabel
        {
            get { return DisplayUnit.Label; }
        }

        public virtual String FormatCode
        {
            get { return m_stringFormatCode; }
            set
            {
                if (m_stringFormatCode == value) return;
                m_stringFormatCode = value;
                OnFormatCodeChanged(EventArgs.Empty);
            }
        }

        public String Label
        {
            get
            {
                if (IsStandardFormatter)
                    return TextManager.Current[LabelKey];
                return m_stringLabel;
            }
        }

        public String LabelKey
        {
            get { return Name; }
        }

        public String Name
        {
            get { return m_stringName; }
            set { m_stringName = value; }       // Public setter for XmlSerializer.
        }

        /// <summary>
        /// For database persistence.
        /// </summary>
        public int NumericFormatterId
        {
            get { return m_intId; }
            set { m_intId = value; }            // Public setter for XmlSerializer.
        }

        public bool IsStandardFormatter
        {
            get { return m_boolIsStandardFormatter; }
        }

        /// <summary>
        /// NumericFormatter must support same API as StationFormatter, else ObjectAttributeModelBase
        /// constructor blows up. Root issue: disconnected managers can manage only 1 type of element.
        /// </summary>
        [XmlIgnore()]
        public virtual int Places
        {
            get { return -1; }
            set { /* Ignore */  }
        }

        public String ShortLabel
        {
            get
            {
                if (IsStandardFormatter)
                    return TextManager.Current.GetShortString(LabelKey);
                return Label;
            }
        }

        public string XmlDisplayUnit
        {
            get { return DisplayUnit.ToSerializedString(); }
            set { DisplayUnit = Unit.FromSerializedString(value); }
        }

        #endregion

        #region Public Methods

        public double DoubleFromDoubleUnit(double adouble, Unit aunit)
        {
            return DisplayUnit.ConvertFrom(adouble, aunit);
        }

        public double DoubleUnitFromDouble(Unit aunit, double adouble)
        {
            return aunit.ConvertFrom(adouble, DisplayUnit);
        }

        public virtual double DoubleUnitFromString(Unit aunit, String astring)
        {
            //TODO 3: Prefer handler at higher-level, rather than silently defaulting to 0.0 here. 
            double adouble = 0.0;
            bool abool = Double.TryParse(astring, NumberStyles.Any, NumberFormatInfo.CurrentInfo, out adouble);
            if (!abool)
                TraceLibrary.WriteLine(CeTraceLevel.Debug, this, "DoubleUnitFromString", "cannot parse: " + astring);// I18N OK <ccla>
            return DoubleUnitFromDouble(aunit, adouble);
        }

        public void InitializeDefaultsFrom(NumericFormatter anf)
        {
            this.DefaultSiDisplayUnit = anf.DefaultSiDisplayUnit;
            this.DefaultUsDisplayUnit = anf.DefaultUsDisplayUnit;
        }

        /// <summary>
        /// Initialize all state in self from passed formatter, non-destructively.
        /// (Don't null out state that isn't null now.) Does not initialize ID's.
        /// </summary>
        public virtual void InitializeFrom(NumericFormatter anf)
        {
            Debug.Assert(m_stringName == anf.m_stringName);

            this.m_intDecimalDigits = anf.m_intDecimalDigits;
            if (anf.m_stringFormatCode != null) this.m_stringFormatCode = anf.m_stringFormatCode;
            if (anf.m_unitDisplay != null) this.m_unitDisplay = anf.m_unitDisplay;
            if (anf.m_unitDisplayDefaultUs != null) this.m_unitDisplayDefaultUs = anf.m_unitDisplayDefaultUs;
            if (anf.m_unitDisplayDefaultSi != null) this.m_unitDisplayDefaultSi = anf.m_unitDisplayDefaultSi;
            if (anf.IsStandardFormatter != IsStandardFormatter) this.m_boolIsStandardFormatter = anf.IsStandardFormatter;
            if (anf.Label != Label) this.m_stringLabel = anf.Label;
        }

        public void ResetDefault(UnitSystem aunitsystem)
        {

        }

        public virtual String StringFromDoubleUnit(double adouble, Unit aunit)
        {
            return StringFromDouble(DisplayUnit.ConvertFrom(adouble, aunit));
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Can be null, as in cases where this formatter was reconstituted from persistence.
        /// </summary>
        protected Unit DefaultSiDisplayUnit
        {
            get { return m_unitDisplayDefaultSi; }
            set { m_unitDisplayDefaultSi = value; }
        }

        /// <summary>
        /// Can be null, as in cases where this formatter was reconstituted from persistence.
        /// </summary>
        protected Unit DefaultUsDisplayUnit
        {
            get { return m_unitDisplayDefaultUs; }
            set { m_unitDisplayDefaultUs = value; }
        }

        protected void OnDecimalDigitsChanged(EventArgs e)
        {
            if (DecimalDigitsChanged != null) DecimalDigitsChanged(this, e);
        }

        protected void OnDisplayUnitChanged(EventArgs e)
        {
            if (DisplayUnitChanged != null) DisplayUnitChanged(this, e);
        }

        protected void OnFormatCodeChanged(EventArgs e)
        {
            if (FormatCodeChanged != null) FormatCodeChanged(this, e);
        }

        #endregion

        #region Private Methods

        private String GetFormatString()
        {
            if (HasStandardFormatSpecifier)
                return FormatCode + m_intDecimalDigits.ToString(CultureInfo.InvariantCulture);
            else
                return FormatCode;      //Allow custom format string, programmatically, ignoring precision.
        }

        private bool HasStandardFormatSpecifier
        {
            get { return FormatCode.Length == 1; }
        }

        private String StringFromDouble(double adouble)
        {
            return adouble.ToString(GetFormatString(), CultureInfo.CurrentCulture);
        }

        #endregion

        #region Private Fields

        private int m_intDecimalDigits;
        private int m_intId;
        private String m_stringFormatCode;
        private String m_stringName;
        private Unit m_unitDisplay;
        private Unit m_unitDisplayDefaultUs;
        private Unit m_unitDisplayDefaultSi;
        private bool m_boolIsStandardFormatter = true;
        private String m_stringLabel = String.Empty;

        #endregion

        #region Public Fields

        public const int MAXDECIMALDIGITS = 15;
        #endregion
    }
}

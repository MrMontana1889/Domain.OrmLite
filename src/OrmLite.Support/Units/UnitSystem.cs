// UnitSystem.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Resource.Support;
using OrmLite.Support.Support;
using System;

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Instances represent one system of units. All units belong to a single
    /// unit system, but that system may be "None."
    /// </summary>
    public class UnitSystem : INamable
    {

        #region Static Public Methods
        static public UnitSystem None { get { return UnitConversionManager.Current.UnitSystemAt(UnitConversionManager.UnitSystemIndex.None); } }

        static public UnitSystem FromSerializedString(String astring)
        {
            if (astring == null) return UnitSystem.None;
            return UnitSystem.None;
        }

        #endregion


        #region Constructors

        public UnitSystem(String astringName)
        {
            m_stringName = astringName;
        }

        #endregion


        #region Public Properties

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

        public String ShortLabel
        {
            get { return TextManager.Current.GetShortString(LabelKey); }
        }

        #endregion


        #region Public Methods

        public String ToSerializedString()
        {
            return Name;
        }

        #endregion


        #region Private Fields

        private String m_stringName;

        #endregion
    }
}

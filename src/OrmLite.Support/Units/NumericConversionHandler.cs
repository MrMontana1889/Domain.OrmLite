// NumericConversionHandler.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Diagnostics;

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Instances manage the conversion between strings and unitized
    /// quantities for clients like simple-bound controls and DataGrids. Knows both
    /// the storage unit and formatter, thus obsoleting Unums. 
    /// </summary>
    public class NumericConversionHandler
    {
        #region Constructors

        public NumericConversionHandler(Unit aunitStorage, NumericFormatter anf)
        {
            m_unitStorage = aunitStorage;
            m_numericformatter = anf;

            Debug.Assert(m_unitStorage != null);
            Debug.Assert(m_numericformatter != null);
            Debug.Assert(m_unitStorage.Dimension == m_numericformatter.DisplayUnit.Dimension, m_unitStorage.Dimension.Name + " != " + m_numericformatter.DisplayUnit.Dimension.Name); // I18N OK <ccla>
        }

        #endregion

        #region Public Properties

        public NumericFormatter Formatter
        {
            get { return m_numericformatter; }
        }

        public Unit StorageUnit
        {
            get { return m_unitStorage; }
        }

        #endregion

        #region Public Methods

        public bool DependsOn(NumericFormatter anf)
        {
            return m_numericformatter == anf;
        }

        /// Following two methods used by UnitizedDataGridTextBoxColumn.
        public double StorageDoubleFromViewDouble(Double adouble)
        {
            return m_numericformatter.DoubleUnitFromDouble(m_unitStorage, adouble);
        }

        public double StorageDoubleFromViewString(String astring)
        {
            return m_numericformatter.DoubleUnitFromString(m_unitStorage, astring);
        }

        public double StorageToViewFactor()
        {
            return m_numericformatter.DisplayUnit.ConvertFrom(1.0, m_unitStorage);
        }

        public double ViewDoubleFromStorageDouble(double adouble)
        {
            return m_numericformatter.DoubleFromDoubleUnit(adouble, m_unitStorage);
        }

        public String ViewStringFromStorageDouble(Double adouble)
        {
            return m_numericformatter.StringFromDoubleUnit(adouble, m_unitStorage);
        }

        #endregion

        #region Private Fields

        private NumericFormatter m_numericformatter;
        private Unit m_unitStorage;

        #endregion
    }
}

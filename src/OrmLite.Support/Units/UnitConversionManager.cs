// UnitConversionManager.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Collections;
using System.Diagnostics;

namespace OrmLite.Support.Units
{
    /// <summary>
    /// Singleton instance manages all singletons in unit subsystem:
    /// units, dimensions, unit systems.
    /// </summary>
    public class UnitConversionManager
    {
        #region Static Public Methods

        static public UnitConversionManager Current
        {
            get
            {
                if (CurrentManager == null)
                    CurrentManager = new UnitConversionManager();
                return CurrentManager;
            }
        }

        #endregion

        #region Public Enumerations

        /// <summary>
        /// DimensionIndex and UnitIndex were originally (framwork v2) NOT intended to be persisted.
        /// They were meant to be a dynamic, in-memory index. However Domain v3 persisted them so now
        /// both styles of enums -- GEMS or .NET -- can be persisted, and are guaranteed not to change.
        /// (Lesson learned: these should have been declared internal originally.)
        /// </summary>
        public enum DimensionIndex
        {
            None,   //Unitless: Favor singletons for sentinel values instead of null, 34 dimensions.
            Currency,
            Percent,
            Unitless,
        }

        public enum UnitIndex
        {
            None,
            Dollars,
            PercentPercent,
            UnitlessPercent,
            UnitlessUnit,
        }

        public enum UnitSystemIndex
        {
            None,           // Replaces VSW "unitless" unit system.
            Si,
            UsCustomary,
            Both
        }

        #endregion

        #region Constructors

        private UnitConversionManager()
        {
            //Sequence here is critical.
            InitializeDimensions();
            InitializeUnitSystems();
            InitializeUnits();

            Debug.Assert(Enum.GetValues(typeof(DimensionIndex)).Length == m_rgdimension.Length);
            Debug.Assert(Enum.GetValues(typeof(UnitIndex)).Length == m_rgunit.Length);
            Debug.Assert(Enum.GetValues(typeof(UnitSystemIndex)).Length == m_rgunitsystem.Length);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Return a list of all known dimensions. Can be useful for enumerating
        /// all known units. For testing and low-performance parsing. Be cautious
        /// of algorithms that require this method in high performance situations.
        /// </summary>
        public IList AvailableDimensions()
        {
            ArrayList aal = new ArrayList();
            foreach (DimensionIndex adi in Enum.GetValues(typeof(DimensionIndex)))
                aal.Add(DimensionAt(adi));
            return aal;
        }

        public Dimension DimensionAt(DimensionIndex adimensionindex)
        {
            return m_rgdimension[(int)adimensionindex];
        }

        public UnitIndex UnitIndexFor(Unit unit)
        {
            for (int i = 0; i < m_rgunit.Length; ++i)
            {
                if (m_rgunit[i] == unit)
                    return (UnitIndex)i;
            }
            return UnitIndex.None;
        }

        public Unit UnitAt(UnitIndex aunitindex)
        {
            return m_rgunit[(int)aunitindex];
        }

        public UnitSystem UnitSystemAt(UnitSystemIndex aunitsystemindex)
        {
            return m_rgunitsystem[(int)aunitsystemindex];
        }

        #endregion

        #region Protected Methods

        protected void InitializeDimensions()
        {
            //Enums here taken from Dimension.FromEnum() 11/6/2003.
            m_rgdimension = new Dimension[] {
                new Dimension("none",                           0,                          "ONE"),
                new Dimension("currency",                       26,                         "ONE"),
                new Dimension("percent",                        5,                          "ONE"),
                new Dimension("unitless",                       35,                         "ONE"),
            };
        }

        protected void InitializeUnits()
        {
            //TODO 0: assign unit systems, proof/test all units!!!
            //Updated by Whar to be as per GEMS hmiUnitLib.idl 8/7/2002. 13 new dimensions, 115 new units, 1 new unit system.
            m_rgunit = new Unit[] {
                new Unit("none",                                    DimensionAt(DimensionIndex.None),                               UnitSystemAt(UnitSystemIndex.None),             1.0,                                0,                                  "NONE"),
                new CurrencyBasedUnit("dollars",                    DimensionAt(DimensionIndex.Currency),                           UnitSystemAt(UnitSystemIndex.None),             1.0,                                1,                                  "DOLLAR"),
                new Unit("percentPercent",                          DimensionAt(DimensionIndex.Percent),                            UnitSystemAt(UnitSystemIndex.None),             1.0,                                1,                                  "PERCENT_PERCENT"),
                new Unit("unitlessPercent",                         DimensionAt(DimensionIndex.Percent),                            UnitSystemAt(UnitSystemIndex.None),             0.01,                               2,                                  "UNITLESS_PERCENT"),
                new Unit("unitlessUnit",                            DimensionAt(DimensionIndex.Unitless),                           UnitSystemAt(UnitSystemIndex.None),             1,                                  0,                                  "UNITLESS_UNIT"),
            };
        }

        protected void InitializeUnitSystems()
        {
            m_rgunitsystem = new UnitSystem[] {
                new UnitSystem("none"),
                new UnitSystem("si"),
                new UnitSystem("usCustomary"),
                new UnitSystem("both")
            };
        }

        #endregion

        #region Private Fields

        static private UnitConversionManager CurrentManager;
        private Dimension[] m_rgdimension;
        private Unit[] m_rgunit;
        private UnitSystem[] m_rgunitsystem;

        #endregion
    }
}

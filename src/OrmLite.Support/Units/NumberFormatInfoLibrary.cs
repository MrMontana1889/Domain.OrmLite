// NumberFormatInfoLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System.Globalization;

namespace OrmLite.Support.Units
{
    public class NumberFormatInfoLibrary
    {
        private NumberFormatInfoLibrary()
        {
        }

        public NumberFormatInfo CurrentNumberFormatInfo
        {
            get { return m_cachedCultureInfo.NumberFormat; }
        }

        public CultureInfo CurrentCultureInfo
        {
            get { return m_cachedCultureInfo; }
        }

        /// <summary>
        /// Current returns the instance contained within this singleton.
        /// </summary>
        public static NumberFormatInfoLibrary Current
        {
            get { return m_instance; }
        }

        private static NumberFormatInfoLibrary m_instance = new NumberFormatInfoLibrary();
        private static CultureInfo m_cachedCultureInfo = new CultureInfo(CultureInfo.CurrentCulture.Name, true);
    }
}

// StringLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Units;
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using OrmLite.Resource.Support;
using OrmLite.Resource.Library;

namespace OrmLite.Support.Library
{

    /// <summary>
    /// Static utility methods for String class.
    /// </summary>
    public static class StringLibrary
    {
        #region Public Methods
        public static string CleanTextResource(string resourceText)
        {
            string cleanResourceText = resourceText;
            cleanResourceText = StringLibrary.StripEllipsis(cleanResourceText);
            cleanResourceText = cleanResourceText.Replace("&", String.Empty);
            return cleanResourceText;
        }

        static public char SPACE
        {
            get { return ' '; }
        }

        //      static public string GroupFormat(int number)
        //      {
        //          return string.Format("{0:n0}", number);
        //      }

        ////Answer astring in aa, ab, ac ... aaa, aab format
        //static public string AAFormat(int anumber)
        //{
        //	// HMGeoNetworkExternalDatabaseType uses this guy for ModelBuilder. It uses this to convert an integer
        //	// to it's equivalent string value... Talked to sbug and a simple integer.toString() would have fixed this instead.
        //	// He is looked at it for the ModelBuilder work.
        //	int avalue = anumber;
        //	int arem;
        //	StringBuilder abuilder = new StringBuilder();
        //	while(avalue > 0)
        //	{
        //		arem = ((avalue - 1) % 26) + 1;
        //		avalue = (avalue - arem) / 26;
        //		abuilder.Insert(0, (char)(arem - 1 + 'a'));
        //	}
        //	return(abuilder.ToString());
        //}

        //static public String AppendPathSeparatorIfNeeded(String astring)
        //{
        //	StringBuilder asb = new StringBuilder(astring);
        //	StringBuilderLibrary.AppendPathSeparatorIfNeeded(asb);
        //	return asb.ToString();
        //}

        /// <summary>
        /// Return the passed file extension in a standard format for comparison.
        /// Assumes passed string is an extension, not a full path.
        /// </summary>
        static public String CanonicalExtension(String astringExtension)
        {
            if (ResourceStringLibrary.IsBlank(astringExtension)) return astringExtension;
            if (astringExtension[0] != '.') // I18n ok <abar>
                astringExtension = '.' + astringExtension; // I18n ok <abar>

            return astringExtension.ToLowerInvariant();
        }

        static public String Capitalize(String astring)
        {
            if (astring.Length == 0) return astring;
            StringBuilder asb = new StringBuilder(astring);
            StringBuilderLibrary.Capitalize(asb);
            return asb.ToString();
        }

        static public string TitleCase(string astring)
        {
            TextInfo textinfo = new CultureInfo(CultureInfo.CurrentUICulture.Name, false).TextInfo;
            return textinfo.ToTitleCase(astring);
        }

        static public bool EqualsIgnoreCase(String astring1, String astring2)
        {
            return String.Compare(astring1, astring2, true, CultureInfo.CurrentCulture) == 0;
        }

        //static public String Proper(String astring)
        //{
        //	if (astring.Length == 0) return astring;
        //	StringBuilder asb = new StringBuilder(astring);
        //	StringBuilderLibrary.Proper(asb);
        //	return asb.ToString();
        //}

        /// <summary>
        /// If the passed string contains any CSV meta-characters,
        /// handle them and quote it.
        /// </summary>
        static private char[] CSV_META_CHARS = new char[] { ' ', ',', '"' }; // DO NOT LOCALIZE <ccla>
        static public String QuoteForCsv(String astring)
        {
            if (astring.IndexOfAny(CSV_META_CHARS) == -1)
                return astring;
            StringBuilder asb = new StringBuilder();
            asb.Append('"');
            foreach (char achar in astring)
            {
                if (achar == '"') asb.Append('"');
                asb.Append(achar);
            }
            asb.Append('"');
            return asb.ToString();
        }

        /// <summary>
        /// Convert passed list to a comma-separated-value string, quoting
        /// and delimiting fields as required.
        /// </summary>
        static public String ToCsv(IList ailist)
        {
            return ToCsv((IEnumerable)ailist);
        }
        static public String ToCsv(IEnumerable aienumerable)
        {
            StringBuilder asb = new StringBuilder();
            bool aboolFirst = true;
            foreach (Object aobject in aienumerable)
            {
                if (aboolFirst)
                    aboolFirst = false;
                else
                    asb.Append(',');
                asb.Append(QuoteForCsv(aobject.ToString()));
            }
            return asb.ToString();
        }

        /// <summary>
        /// Parse the passed line into an array of tokens using (mostly) Excel CSV
        /// conventions. (Note this favors expected rather than pure Excel behavior
        /// for unquoted white space: see test cases.)
        /// </summary>
        static public String[] TokenizeCsv(String astring)
        {
            ArrayList aal = new ArrayList();
            if (astring != null && astring.Length > 0)
                ParseCsv(astring, aal);
            return CollectionLibrary.ToStringArray(aal);
        }

        /// <summary>
        /// Returns a boolean indicating whether any of the specified charcters are present in the string.
        /// </summary>
        static public bool Contains(String astring, String acharacters)
        {
            if (ResourceStringLibrary.IsBlank(astring)) return false;
            char[] acharArray = acharacters.ToCharArray();
            if ((astring.IndexOfAny(acharArray)) == -1) return false;
            return true;
        }

        ///// <summary>
        ///// Returns a boolean indicating whether digits are present in the string.
        ///// </summary>
        //static public bool ContainsDigits(String astring)
        //{
        //	int i = 0;
        //	if (IsBlank(astring)) return false;
        //	for (i = 0; i < astring.Length; i++) 
        //	{
        //		if (char.IsDigit(astring, i)) return true;
        //	}
        //	return false;
        //}

        /// <summary>
        /// Returns a boolean indicating whether the first and last characters in the string
        /// are double quotation characters. No consideration of internal characters is given.
        /// </summary>
        static public bool IsQuoted(String astring)
        {
            if (ResourceStringLibrary.IsBlank(astring)) return false;
            return ((astring.IndexOf('"') == 0) && (astring.LastIndexOf('"')
                == (astring.Length - 1))) ? true : false;
        }

        /// <summary>
        /// Removes quotes from a quoted string. If the string is not quoted the original
        /// string is returned so there is no need to check if the string is quoted first.
        /// </summary>
        static public String RemoveQuotes(String astring)
        {
            if (IsQuoted(astring))
            {
                return astring.Substring(1, astring.Length - 2);
            }
            else
            {
                return astring;
            }
        }

        //      public static bool TryGetInteger(string text, TryGetIntegerStrategy strategy, out int returnValue)
        //      {
        //          if (IsWholeNumber(text))
        //              return int.TryParse(text, out returnValue);

        //          double doubleValue = ParseDoubleAny(text, Double.NaN);
        //          if (Double.IsNaN(doubleValue))
        //          {
        //              returnValue = 0;
        //              return false;
        //          }

        //          switch (strategy)
        //          {
        //              case TryGetIntegerStrategy.RoundDown:
        //                  returnValue = Convert.ToInt32(Math.Floor(doubleValue));
        //                  return true;
        //              case TryGetIntegerStrategy.RoundUp:
        //                  returnValue = Convert.ToInt32(Math.Ceiling(doubleValue));
        //                  return true;
        //              case TryGetIntegerStrategy.RoundNearest:
        //                  returnValue = Convert.ToInt32(Math.Round(doubleValue));
        //                  return true;
        //          }

        //          returnValue = 0;
        //          return false;
        //      }

        ///// <summary>
        ///// Returns a string of digits extracted from the supplied string. 
        ///// </summary>
        //static public String GetDigits(String astring)
        //{
        //	String adigits = "";
        //	int i = 0;
        //	if (IsBlank(astring)) return "";
        //	for (i = 0; i < astring.Length; i++) 
        //	{
        //		if (char.IsDigit(astring, i)) adigits += astring.Substring(i,1);
        //	}
        //	return adigits;
        //}

        /// <summary>
        /// Returns a boolean indicating whether the string represents a whole number.
        /// </summary>
        static public bool IsWholeNumber(String astring)
        {
            return IsWholeNumber(astring, NumberFormatInfoLibrary.Current.CurrentNumberFormatInfo);
        }

        /// <summary>
        /// Returns a boolean indicating whether the string represents a whole number.
        /// The provided sign is used in order to recognize negative numbers.
        /// </summary>
        static public bool IsWholeNumber(String astring, NumberFormatInfo numberFormat)
        {
            int i = 0;
            int acount = 0;
            if (ResourceStringLibrary.IsBlank(astring)) return false;
            for (i = 0; i < astring.Length; i++)
            {
                if (char.IsDigit(astring, i)) acount++;
            }

            if (acount == astring.Length)
                return true;
            else
            {
                bool retVal;
                int negativePattern = numberFormat.NumberNegativePattern;
                switch (negativePattern)
                {
                    case 0: // (n)
                        retVal = (astring.Length == acount + 2 && astring[0] == '(' && astring[astring.Length - 1] == ')');
                        break;
                    case 1: // -n
                        retVal = (astring.Length == acount + 1 && astring.Substring(0, numberFormat.NegativeSign.Length) == numberFormat.NegativeSign);
                        break;
                    case 2: // - n
                        retVal = (astring.Length == acount + 2 && astring.Substring(0, numberFormat.NegativeSign.Length) == numberFormat.NegativeSign);
                        break;
                    case 3: // n-
                        retVal = (astring.Length == acount + 1 &&
                            astring.Substring(astring.Length - numberFormat.NegativeSign.Length, numberFormat.NegativeSign.Length) == numberFormat.NegativeSign);
                        break;
                    case 4: // n -
                        retVal = (astring.Length == acount + 2 &&
                            astring.Substring(astring.Length - numberFormat.NegativeSign.Length, numberFormat.NegativeSign.Length) == numberFormat.NegativeSign);
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                }

                return retVal;
            }
        }

        /// <summary>
        /// Returns a string array of tokens separated by the specified delimiters. 
        /// Note that quoted terms containing delimiters are not treated as whole
        /// terms. Quotes are treated the same as other characters. See test case
        /// for expected behavior.
        /// </summary>
        static public String[] GetTokens(String astring, char[] adelimiters)
        {
            int i = 0, j = 0;
            int astart = 0;
            int alength = 0;
            String atemp = null;

            ArrayList aalReturn = new ArrayList();
            char[] acharArray = astring.ToCharArray();
            for (i = 0; i <= acharArray.GetUpperBound(0); i++)
            {
                alength++;
                for (j = 0; j <= adelimiters.GetUpperBound(0); j++)
                {
                    if (adelimiters[j].CompareTo(acharArray[i]) == 0)
                    {
                        atemp = astring.Substring(astart, alength - 1);
                        if (atemp != "") aalReturn.Add(atemp);
                        alength = 0;
                        astart = i + 1;
                    }
                }
            }

            atemp = astring.Substring(astart, alength);
            if (atemp != "") aalReturn.Add(atemp);
            return (String[])aalReturn.ToArray(typeof(String));
        }

        ///// <summary>
        ///// Returns a string array of tokens separated by spaces. 
        ///// Quoted terms containing delimiters *are* treated as whole terms.
        ///// Designed for parsing a command line. Behaves differently to GetTokens.
        ///// </summary>
        //static public String[] GetCommandLineArguments(String astring)
        //{
        //	int astart = 0;
        //	int alength = 0;
        //	bool aisInQuote = false;
        //	Char acommandLineDelimiter = ' ';
        //	Char aquoteCharacter = '\"';
        //	String atemp = null;
        //	ArrayList areturnArgs = new ArrayList();
        //	char[] acharArray = astring.ToCharArray();

        //	for (int ai = 0; ai <= acharArray.GetUpperBound(0); ai++)
        //	{
        //		alength++;

        //		if (aquoteCharacter.CompareTo(acharArray[ai]) == 0)
        //			aisInQuote = !aisInQuote;

        //		if (acommandLineDelimiter.CompareTo(acharArray[ai]) == 0 && !aisInQuote)
        //		{
        //			atemp = astring.Substring(astart, alength - 1);
        //			atemp = RemoveQuotes(atemp);
        //			if (atemp != "") areturnArgs.Add(atemp);
        //			alength = 0;
        //			astart = ai + 1;
        //		}
        //	}

        //	atemp = astring.Substring(astart, alength);
        //	atemp = RemoveQuotes(atemp);
        //	if (atemp != "") areturnArgs.Add(atemp);
        //	return (String[])areturnArgs.ToArray(typeof(String));
        //}

        /// <summary>
        /// Safely parse the string representation of a double: returning a default
        /// value instead of throwing exceptions on parsing errors. "Any" number style
        /// will accept all legal numerical formatting, except hex digits.
        /// </summary>
        static public double ParseDoubleAny(String astring, double adoubleDefault)
        {
            return ParseDouble(astring, adoubleDefault, NumberStyles.Any);
        }

        static public double ParseDouble(String astring, double adoubleDefault, NumberStyles astyle)
        {
            double adouble;
            bool abool = Double.TryParse(astring, astyle, NumberFormatInfo.CurrentInfo, out adouble);
            return abool ? adouble : adoubleDefault;
        }

        static public String StripAmpersands(String astringText)
        {
            String astringNewText = String.Empty;
            int aintIndexOf = astringText.IndexOf("&"); // do we use ampersands in other languages??? - acceleratorPrefix (more to resx file)
            if (aintIndexOf > 0)
            {
                astringNewText = astringText.Substring(0, aintIndexOf);
                astringNewText += astringText.Substring(aintIndexOf + 1, astringText.Length - (aintIndexOf + 1));
            }
            else if (aintIndexOf == 0)
                astringNewText = astringText.Substring(1, astringText.Length - 1);
            if (astringNewText != String.Empty)
                return astringNewText;
            else
                return astringText;
        }

        static public string StripEllipsis(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            if (text.EndsWith("..."))
            {
                int startIndex = text.IndexOf("...");
                text = text.Remove(startIndex, 3);
                return text;
            }

            return text;
        }

        ///// <summary>
        ///// Returns a label with its numeric portion padded with zeros. Useful for smart label sort/filter operations.
        ///// </summary>
        //static public String GetSmartLabel(String alabel)
        //{
        //	if (alabel == null) return null;
        //          if (alabel.Length > SmartLabelBuffer.Length) alabel = alabel.Substring(0, SmartLabelBuffer.Length);

        //	bool hasDigits = false;
        //	int startDigitIndex = alabel.Length;
        //	for (int i=0; i<alabel.Length; ++i)
        //	{
        //		if (char.IsDigit(alabel[i]))
        //		{
        //			hasDigits = true;
        //			startDigitIndex = i;
        //			break;
        //		}
        //	}

        //	if (!hasDigits) return alabel;

        //	int endDigitIndex = alabel.Length;
        //	for (int i=startDigitIndex + 1; i<alabel.Length; ++i)
        //	{
        //		if (!char.IsDigit(alabel[i]))
        //		{
        //			endDigitIndex = i;
        //			break;
        //		}
        //	}

        //	int padLength = endDigitIndex - startDigitIndex;
        //	int padNeeded = (padLength < SmartLabelLength) ? (SmartLabelLength - padLength) : 0;
        //	alabel.CopyTo(0, SmartLabelBuffer, 0, startDigitIndex);

        //          if ((padNeeded + startDigitIndex) > SmartLabelBuffer.Length) padNeeded = SmartLabelBuffer.Length - startDigitIndex;
        //	SmartLabelPadChars.CopyTo(0, SmartLabelBuffer, startDigitIndex, padNeeded);

        //	bool overFlow = (padNeeded + alabel.Length) > SmartLabelBuffer.Length;
        //	alabel.CopyTo(startDigitIndex, SmartLabelBuffer, startDigitIndex + padNeeded,
        //              (overFlow) ? (SmartLabelBuffer.Length - padNeeded - startDigitIndex) : alabel.Length - startDigitIndex);

        //          return new String(SmartLabelBuffer, 0, (overFlow) ? SmartLabelBuffer.Length : padNeeded + alabel.Length);
        //}
        ///// <summary>
        ///// Returns a globalized string that represents the number of hours, minutes and seconds.
        ///// </summary>
        //static public string ConvertSecondsToHourMinuteSecondString(int seconds)
        //{
        //          if (seconds < 0)
        //              throw new ArgumentException("Parameter 'seconds' must be zero or greater."); // I18N. OK. Whar.

        //	int hours = Convert.ToInt32(Math.Floor((double)seconds / 3600.0));
        //	seconds -= hours * 3600;
        //	int minutes = Convert.ToInt32(Math.Floor((double)seconds / 60.0));
        //	seconds -= minutes * 60;

        //	return TextManager.Current["hs.HourMinuteSecondFormat", hours.ToString("00", CultureInfo.InvariantCulture), 
        //		minutes.ToString("00", CultureInfo.InvariantCulture), seconds.ToString("00", CultureInfo.InvariantCulture)];
        //}

        ///// <summary>
        ///// Returns a globalized string that represents the number of hours, minutes and seconds.
        ///// </summary>
        //static public string ConvertSecondsToHourMinuteSecondString(double seconds)
        //{
        //          if (seconds < 0.0)
        //              throw new ArgumentException("Parameter 'seconds' must be zero or greater."); // I18N. OK. Whar.

        //	int hours = Convert.ToInt32(Math.Floor(seconds / 3600.0));
        //	seconds -= hours * 3600;
        //	int minutes = Convert.ToInt32(Math.Floor(seconds / 60.0));
        //	seconds -= minutes * 60;

        //	return TextManager.Current["hs.HourMinuteSecondFormat", hours.ToString("00", CultureInfo.InvariantCulture), 
        //		minutes.ToString("00", CultureInfo.InvariantCulture), seconds.ToString("00.00", CultureInfo.InvariantCulture)];
        //}

        ///// <summary>
        ///// Method implements the String.GetHashCode() function from MS.Net 1.x. Can be used in lieu
        ///// of String.GetHashCode() where you need the hash code value to be immutable between framework
        ///// versions.
        ///// </summary>
        //static public int GetHashCode(string text)
        //{
        //	int hash = 5381;
        //	for (int i = 0; i < text.Length; i++)
        //	{
        //		int c = (int)text[i];
        //		hash = ((hash << 5) + hash) ^ c;
        //	}
        //	return hash;
        //}

        //public static string ShortenToLength(string text, int targetLength)
        //{
        //	if (text == null) return text;
        //	if (text.Length < targetLength)
        //		return text;

        //	string retVal = text.Substring(0, targetLength - 3).TrimEnd();
        //	retVal = retVal.Replace("&", "&&");

        //	retVal += "...";
        //	return retVal;
        //}
        #endregion

        //#region Private Methods

        static private void ParseCsv(String asCurrent, ArrayList aalReturn)
        {
            String asFirst = null;
            String asRemainder = null;
            int aiComma = FirstDelimiter(asCurrent);

            if (aiComma > -1)
            {
                asFirst = asCurrent.Substring(0, aiComma).Trim();
                asRemainder = asCurrent.Substring(aiComma + 1).Trim();
            }
            else
                asFirst = asCurrent;        // No commas, so entire line is token.

            aalReturn.Add(ReduceQuotes(asFirst));

            if (asRemainder != null)
                ParseCsv(asRemainder, aalReturn);
        }

        static private int FirstDelimiter(String asCurrent)
        {
            if (asCurrent.StartsWith("\""))
            {
                bool aboolInQuote = true;
                int aiLength = asCurrent.Length;
                for (int ai = 1; ai < aiLength; ai++)
                {
                    char acharCurrent = asCurrent[ai];
                    if (acharCurrent == '"')
                    {
                        aboolInQuote = !aboolInQuote;
                    }
                    else if (acharCurrent == ',' && !aboolInQuote)
                    {
                        return ai;
                    }
                }
                return -1;
            }
            else
                return asCurrent.IndexOf(',');
        }

        /// <summary>
        /// Remove quotes around tokens, as well as pairs of quotes
        /// within tokens.
        /// </summary>
        static private String ReduceQuotes(String asCurrent)
        {
            StringBuilder asb = new StringBuilder();
            int aiLength = asCurrent.Length;
            int aiCurrent = 0;

            if (asCurrent.StartsWith("\"") && asCurrent.EndsWith("\""))
            {
                aiCurrent = 1;
                aiLength--;
            }

            bool aboolOneQuoteSeen = false;
            bool aboolTwoQuotesSeen = false;

            while (aiCurrent < aiLength)
            {
                char acharCurrent = asCurrent[aiCurrent];
                if (acharCurrent == '"')
                {
                    aboolTwoQuotesSeen = (aboolOneQuoteSeen) ? true : false;
                    aboolOneQuoteSeen = true;
                }
                else
                {
                    aboolOneQuoteSeen = false;
                    aboolTwoQuotesSeen = false;
                }

                if (aboolTwoQuotesSeen)
                {
                    aboolTwoQuotesSeen = false;
                    aboolOneQuoteSeen = false;
                    aiCurrent++;
                    continue;
                }
                asb.Append(acharCurrent);
                aiCurrent++;
            }

            return asb.ToString();
        }

        //#endregion

        //#region Constructors

        //private StringLibrary()
        //{
        //}

        //#endregion

        //#region Private Fields
        //// Using static member variables in order to avoid unnecessary String instantiations
        //// during SmartLabel generation. Handling networks numbered up to 9,999,999.
        //private static char[] SmartLabelBuffer = new char[255];
        //private static readonly String SmartLabelPadChars = "0000000";
        //private static readonly int SmartLabelLength = 7;
        //      #endregion

        //      #region Public Enumerations
        //      public enum TryGetIntegerStrategy
        //      {
        //          RoundDown,
        //          RoundUp,
        //          RoundNearest,
        //      }
        //      #endregion
    }
}

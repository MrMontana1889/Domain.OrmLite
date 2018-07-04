// StringBuilderLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace OrmLite.Support.Library
{

    /// <summary>
    /// Static utility methods for string building operations.
    /// </summary>
    public static class StringBuilderLibrary
    {

        public static void AppendPathSeparatorIfNeeded(StringBuilder asb)
        {
            if (asb.Length == 0) return;
            if (!(asb[asb.Length - 1] == Path.DirectorySeparatorChar))
                asb.Append(Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Capitalize the passed StringBuilder by uppercasing the first letter.
        /// </summary>
        public static void Capitalize(StringBuilder astringbuilder)
        {
            if (astringbuilder.Length > 0)
                astringbuilder[0] = Char.ToUpper(astringbuilder[0], CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Capitalize first letter of passed StringBuilder and lower case the rest.
        /// </summary>
        public static void Proper(StringBuilder astringbuilder)
        {
            for (int ai = 0; ai < astringbuilder.Length; ai++)
                astringbuilder[ai] = Char.ToLower(astringbuilder[ai], CultureInfo.CurrentCulture);
            Capitalize(astringbuilder);
        }
    }
}

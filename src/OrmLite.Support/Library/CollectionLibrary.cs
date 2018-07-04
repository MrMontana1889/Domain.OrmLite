// CollectionLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace OrmLite.Support.Library
{

    /// <summary>
    /// Static utility methods for Collection classes and interfaces.
    /// </summary>
    public static class CollectionLibrary
    {
        static public char SPACE
        {
            get { return ' '; }
        }

        /// <summary>
        /// Return the elements of the collection as substrings in a 
        /// single blank-delimited string.
        /// </summary>
        static public String AsSubstrings(ICollection aic)
        {
            return AsSubstringsSeparatedBy(aic, SPACE);
        }

        /// <summary>
        ///  Return the elements of the collection as substrings in a single 
        ///  String delimited by passed char.
        /// </summary>
        static public String AsSubstringsSeparatedBy(ICollection aic, char achar)
        {
            StringBuilder asb = new StringBuilder();
            StringWriter asw = new StringWriter(asb, CultureInfo.InvariantCulture);

            int aint = 0;
            foreach (Object aobject in aic)
            {
                aint++;
                asw.Write(aobject.ToString());
                if (!(aint == aic.Count))
                    asw.Write(achar);
            }
            return asb.ToString().Trim();
        }

        ////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Convert the passed homogeneous collection to ADO.NET types using reflection.
        /// </summary>
        static public DataSet ToDataSet(ICollection aic)
        {
            DataSet adataset = new DataSet(aic.GetType().Name);
            adataset.Locale = CultureInfo.InvariantCulture;
            DataTable atable = ToDataTable(aic);
            if (atable != null)
            {
                atable.Locale = CultureInfo.InvariantCulture;
                adataset.Tables.Add(atable);
            }
            return adataset;
        }

        static public DataTable ToDataTable(ICollection aic, String aTableName, PropertyInfo[] metaInfo)
        {
            return ToDataTable(aic, aTableName, metaInfo, false);
        }

        static public DataTable ToDataTable(ICollection aic, String aTableName, PropertyInfo[] metaInfo, bool aboolAddIdColumn)
        {
            DataTable atable = new DataTable(aTableName);
            atable.Locale = CultureInfo.InvariantCulture;

            //TODO 1: parameterize name and type
            if (aboolAddIdColumn)
                atable.Columns.Add(new DataColumn("Id", typeof(Int32)));

            foreach (PropertyInfo api in metaInfo)
            {
                DataColumn acolumn = new DataColumn(
                    api.Name,
                    TypeLibrary.DefaultDatabaseType(api.PropertyType));
                atable.Columns.Add(acolumn);
            }

            foreach (Object aobject in aic)
            {
                DataRow arow = atable.NewRow();
                foreach (PropertyInfo api in metaInfo)
                {
                    Object aoProperty = api.GetValue(aobject, null);
                    arow[api.Name] = (aoProperty != null) ?
                        TypeLibrary.DatabaseFromObject(
                        TypeLibrary.DefaultDatabaseType(aoProperty.GetType()),
                        aoProperty) :
                        DBNull.Value;
                }
                atable.Rows.Add(arow);
            }
            return atable;
        }

        /// <summary>
        /// Convert the passed homogeneous collection to an ADO.NET DataTable.
        /// Calls to TypeLibrary here let us map complex types to primitive types
        /// understood by database.
        /// </summary>
        static public DataTable ToDataTable(ICollection aic, bool aboolAdIdColumn)
        {
            DataTable atable = null;
            if (aic.Count > 0)
            {
                IEnumerator eNum = aic.GetEnumerator();
                eNum.Reset();
                eNum.MoveNext();

                Object aobject = eNum.Current;
                atable = ToDataTable(aic, aobject.GetType().Name, aobject.GetType().GetProperties(), aboolAdIdColumn);
                atable.Locale = CultureInfo.InvariantCulture;
            }
            return atable;
        }

        static public DataTable ToDataTable(ICollection aic)
        {
            return ToDataTable(aic, false);
        }

        static public PropertyInfo[] AttributeDefinitionsToProperties(IList attributes, Type type)
        {
            PropertyInfo[] props = new PropertyInfo[attributes.Count];
            for (int i = 0; i < props.Length; i++)
                props[i] = type.GetProperty(attributes[i].ToString());

            return props;
        }

        ////////////////////////////////////////////////////////////////////////

        static public Object[] ToObjectArray(ICollection aic)
        {
            Object[] argo = new Object[aic.Count];
            int ai = 0;
            foreach (Object aobject in aic)
                argo[ai++] = aobject;
            return argo;
        }

        /// <summary>
        /// Convert a generic, dynamic collection to a typed, fixed-length 
        /// array of strings.
        /// </summary>
        static public String[] ToStringArray(ICollection aic)
        {
            //TODO 3: why doesn't this work? (String[])aal.ToArray(typeof(String[]))
            //TODO 3: This seems to work ... (String[])aal.ToArray(typeof(String)) -- Whar
            String[] args = new String[aic.Count];
            int ai = 0;
            foreach (Object aobject in aic)
                args[ai++] = (String)aobject;
            return args;
        }
    }
}

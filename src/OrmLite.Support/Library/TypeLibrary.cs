// TypeLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Resource.Library;
using OrmLite.Support.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace OrmLite.Support.Library
{

	/// <summary>
	/// Static utility methods for manipulating System.Type objects.
	/// </summary>
	public static class TypeLibrary
    {
        /// <summary>
        /// Map from a .NET Type to Haestad.Domain FieldDataType. Note that
        /// FieldDataType.LongText is currently never returned from this method.
        /// </summary>
        static public FieldDataType FieldDataTypeFromType(Type atype)
        {
            if (atype == typeof(bool)) return FieldDataType.Boolean;
            if (atype == typeof(DateTime)) return FieldDataType.DateTime;
            if (atype.IsEnum) return FieldDataType.Enumerated;
            if (atype == typeof(int)) return FieldDataType.Integer;
            if (atype == typeof(Int64)) return FieldDataType.Integer;
            if (atype == typeof(String)) return FieldDataType.Text;
            if (atype == typeof(double)) return FieldDataType.Real;
            if (atype == typeof(decimal)) return FieldDataType.Real;
            if (atype == typeof(Char)) return FieldDataType.Text;
            if (atype == typeof(Delegate)) return FieldDataType.LongBinary;
            if (atype == typeof(byte)) return FieldDataType.Collection;

            Debug.Assert(false);
            return FieldDataType.Text;
        }

        /// <summary>
        /// Map from a Haestad.Domain FieldDataType to a .NET Type.
        /// </summary>
        static public Type TypeFromFieldDataType(FieldDataType type)
        {
            switch (type)
            {
                case FieldDataType.Boolean:
                    return typeof(bool);
                case FieldDataType.DateTime:
                    return typeof(DateTime);
                case FieldDataType.Integer:
                case FieldDataType.Referenced:
                case FieldDataType.Enumerated:
                    return typeof(int);
                case FieldDataType.Text:
                case FieldDataType.LongText:
                    return typeof(String);
                case FieldDataType.Real:
                    return typeof(double);
                //TODO 0 Review later for robustness.
                case FieldDataType.Collection:
                    return typeof(byte);
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        /// <summary>
        /// Return the passed object, mapped to another object as required
        /// to persist in database.
        /// </summary>
        //TODO 3: generalize hard references to HMI domain classes here?
        static public Object DatabaseFromObject(Type atypeDatabase, Object aobject)
        {
            return aobject;
        }

        /// <summary>
        /// Return the passed object, converted from database format to
        /// in-memory object.
        /// </summary>
        static public Object ObjectFromDatabase(Type atypeHmi, Object aobject)
        {
            return aobject;
        }

        /// <summary>
        /// Return the type that is used to persist instances of the passed
        /// type in a database. Returned type should be a legal DataType for
        /// DataColumn constructor.
        /// </summary>
        static public Type DefaultDatabaseType(Type atype)
        {
            return atype;
        }

        static public Type FromEnum(SystemType ast)
        {
            switch (ast)
            {
                case SystemType.Bool: return typeof(bool);
                case SystemType.Byte: return typeof(byte);
                case SystemType.Char: return typeof(char);
                case SystemType.DateTime: return typeof(DateTime);
                case SystemType.Decimal: return typeof(Decimal);
                case SystemType.Delegate: return typeof(Delegate);
                case SystemType.Double: return typeof(double);
                case SystemType.Guid: return typeof(Guid);
                case SystemType.Int: return typeof(int);
                case SystemType.Long: return typeof(long);
                case SystemType.Object: return typeof(Object);
                case SystemType.String: return typeof(String);
                default:
                    Debug.Assert(false);
                    return null;
            }
        }

        /// <summary>
        /// Map a simple string description to a type object. Used for 
        /// soft-referencing types in database fields and elsewhere.
        /// </summary>
        static public Type FromString(String astringType)
        {
            if (astringType.Equals("bool")) return typeof(bool);
            if (astringType.Equals("char")) return typeof(char);
            if (astringType.Equals("decimal")) return typeof(Decimal);
            if (astringType.Equals("double")) return typeof(double);
            if (astringType.Equals("int")) return typeof(int);
            if (astringType.Equals("string")) return typeof(String);

            throw new Exception("TypeLibrary.FromString: " + astringType);  // DO NOT LOCALIZE <ccla>
        }

        /// <summary>
        /// Return a boolean, whether the passed type is a primitive numeric type.
        /// </summary>
        static public bool IsNumeric(Type atype)
        {
            if (atype == typeof(int) ||
                atype == typeof(double) ||
                atype == typeof(decimal) ||
                atype == typeof(sbyte) ||
                atype == typeof(byte) ||
                atype == typeof(short) ||
                atype == typeof(ushort) ||
                atype == typeof(uint) ||
                atype == typeof(long) ||
                atype == typeof(ulong) ||
                atype == typeof(float))
                return true;
            return false;
        }

        static public bool IsNumericFormatted(Type atype)
        {
            if (atype == typeof(double) ||
                atype == typeof(decimal) ||
                atype == typeof(float))
                return true;
            return false;
        }

        /// <summary>
        /// Instantiate a new instance of the passed FieldDataType by parsing the
        /// passed string. Note invalid double string representations will parse to
        /// 0.0 without throwing an exception.
        /// </summary>
        static public Object NewFromString(FieldDataType afieldDataType, String astring)
        {
            switch (afieldDataType)
            {
                case FieldDataType.Boolean:
                    Boolean boolFromString;
                    if (Boolean.TryParse(astring, out boolFromString))
                        return boolFromString;
                    return false;
                case FieldDataType.Integer:
                    return (int)Math.Round(StringLibrary.ParseDoubleAny(astring, 0.0));
                case FieldDataType.Enumerated:
                case FieldDataType.Referenced:
                    return Int32.Parse(astring);
                case FieldDataType.Real:
                    return StringLibrary.ParseDoubleAny(astring, 0.0);
                case FieldDataType.LongText:
                case FieldDataType.Text:
                    return astring;
                case FieldDataType.DateTime:
                    DateTime dateTimeFromString;
                    if (DateTime.TryParse(astring, out dateTimeFromString))
                        return dateTimeFromString;
                    return DateTime.Today;
                default:
                    throw new Exception("TypeLibrary.NewFromString: " + afieldDataType.ToString() + " \"" + astring + "\""); // DO NOT LOCALIZE <ccla>
            }
        }

        /// <summary>
        /// Instantiate a new instance of the passed type by parsing the
        /// passed string. If every Type implemented Parse() this would be
        /// easy, but they don't. (Type doesn't implement Parse().) Note 
        /// invalid double string representations will parse to
        /// 0.0 without throwing an exception.
        /// </summary>
        static public Object NewFromString(Type atype, String astring)
        {
            //TODO 3: use ComponentModel.TypeConverter?
            if (atype == typeof(Int32))
                return Int32.Parse(astring, CultureInfo.InvariantCulture);
            if (atype == typeof(Double))
                return StringLibrary.ParseDoubleAny(astring, 0.0);
            if (atype == typeof(String))
                return astring;
            if (atype == typeof(char))
                return Char.Parse(astring);
            if (atype == typeof(bool))
                return Boolean.Parse(astring);
            if (atype == typeof(DateTime))
                return DateTime.Parse(astring, CultureInfo.InvariantCulture);
            if (atype == typeof(Guid))
                return new Guid(astring);

            throw new Exception("TypeLibrary.NewFromString: " +
                atype.FullName + " \"" + astring + "\"");   // DO NOT LOCALIZE <ccla>
        }

        /// <summary>
        /// Instantiate passed type from string, supplying legal default
        /// values when the string is unparsable.
        /// </summary>
        static public Object NewFromStringDefault(Type atype, String astring)
        {
            if (IsNumeric(atype) && ResourceStringLibrary.IsBlank(astring))
                return DBNull.Value;
            //TODO 1: add other type tests
            return NewFromString(atype, astring);
        }

        /// <summary>
        /// Return a new object instantiated with the default constructor
        /// of the passed type.
        /// </summary>
        static public Object New(Type atype)
        {
            return atype.GetConstructor(new Type[] { }).Invoke(new Object[] { });
        }

        /// <summary>
        /// Returns a new object instantiated with the one-argument constructor
        /// taking the type of the passed object, or one of its supertypes. 
        /// </summary>
        static public Object NewWith(Type atype, Object aobject)
        {
            return NewWith(atype, new Object[] { aobject });
        }

        static public Object NewWith(Type atype, Object[] parameters)
        {
            //TODO 1: search for interfaces of aobject also
            ConstructorInfo aci = null;

            Type[] atypeParameters = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters.GetValue(i) != null)
                    atypeParameters.SetValue(parameters.GetValue(i).GetType(), i);
                else
                    atypeParameters.SetValue(typeof(Object), i);
            }

            while (aci == null && atypeParameters != null)
            {
                if ((aci = atype.GetConstructor(atypeParameters)) != null)
                    break;

                atypeParameters = (atypeParameters.Length == 1) ? new Type[] { ((Type)atypeParameters[0]).BaseType } : null;
                if (atypeParameters != null && atypeParameters[0] == null) break;
            }

            if (aci == null) throw new Exception("TypeLibrary.NewWith");
            return aci.Invoke(parameters);
        }

        /// <summary>
        /// Return the type that is used in memory to represent legal 
        /// values within the passed type. (A bit of a hack so SelectionSet's
        /// can live in TypeDefinition tables.)
        /// </summary>
        static public Type ValueType(Type atype)
        {
            return atype;
        }

        /// <summary>
        /// Returns a default (zero) value for a FieldDataType.
        /// </summary>
        /// <param name="atype">FieldDataType to return a default value for.</param>
        /// <returns>Default value for the passed FieldDataType.</returns>
        static public Object DefaultValueFor(FieldDataType atype)
        {
            switch (atype)
            {
                case FieldDataType.Enumerated:
                case FieldDataType.Integer:
                case FieldDataType.Referenced:
                    return 0;

                case FieldDataType.Real:
                    return 0.0;

                case FieldDataType.Text:
                case FieldDataType.LongText:
                    return String.Empty;

                case FieldDataType.Boolean:
                    return false;

                case FieldDataType.DateTime:
                    return DateTime.Now;

                case FieldDataType.LongBinary:
                case FieldDataType.Collection:
                    return null;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        static public Object DeserializeFromString(FieldDataType fieldDataType, String source)
        {
            bool aboolParseError;
            return DeserializeFromString(fieldDataType, source, null, out aboolParseError);
        }

        /// <summary>
        /// Deserialize an object from a base64 string
        /// </summary>
        /// <param name="source">Base64 string</param>
        /// <returns>Deserialized object</returns>		
        static public Object DeserializeFromString(FieldDataType fieldDataType, String source, Object defaultValue, out bool parseError)
        {
            return DeserializeFromString(fieldDataType, source, defaultValue, CultureInfo.InvariantCulture, out parseError);
        }

        /// <summary>
        /// Deserialize an object from a base64 string
        /// </summary>
        /// <param name="source">Base64 string</param>
        /// <returns>Deserialized object</returns>		
        static public Object DeserializeFromString(FieldDataType fieldDataType, String source, Object defaultValue, CultureInfo culture, out bool parseError)
        {
            parseError = false;
            if (source == null || (source == "" && fieldDataType != FieldDataType.Text && fieldDataType != FieldDataType.LongText)) return null;

            switch (fieldDataType)
            {
                case FieldDataType.Boolean:
                    return bool.Parse(source);

                case FieldDataType.DateTime:
                    return DateTime.Parse(source, culture);

                case FieldDataType.Enumerated:
                case FieldDataType.Integer:
                case FieldDataType.Referenced:
                    try
                    {
                        return int.Parse(source, culture);
                    }
                    catch
                    {
                        parseError = true;
                        return 0;
                    }

                case FieldDataType.LongText:
                case FieldDataType.Text:
                    return source;

                case FieldDataType.Real:
                    {
                        double adouble;
                        parseError = !double.TryParse(source, NumberStyles.Any, culture.NumberFormat, out adouble);
                        return parseError ? defaultValue : adouble;
                    }

                default:
                    using (MemoryStream stream = new MemoryStream(Convert.FromBase64String(source)))
                    {
                        BinaryReader br = new BinaryReader(stream);
                        BinaryFormatter formatter = new BinaryFormatter();
                        return formatter.Deserialize(stream);
                    }
            }
        }

        /// <summary>
        /// Serialize an object using .NET's Binary formatter and returns it encoded into base64 string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>Base64 string</returns>
        static public String SerializeToString(FieldDataType fieldDataType, Object obj)
        {
            return SerializeToString(fieldDataType, obj, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Serialize an object using .NET's Binary formatter and returns it encoded into base64 string
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <param name="culture">Culture to use during the serialization</param>
        /// <returns>Base64 string</returns>
        static public String SerializeToString(FieldDataType fieldDataType, Object obj, CultureInfo culture)
        {
            if (obj == null) return ""; // DO NOT LOCALIZE <ccla>

            switch (fieldDataType)
            {
                case FieldDataType.Real:
                    return ((Double)obj).ToString(culture.NumberFormat);
                case FieldDataType.DateTime:
                    return ((DateTime)obj).ToString(culture);
                case FieldDataType.Boolean:
                case FieldDataType.Enumerated:
                case FieldDataType.Integer:
                case FieldDataType.Referenced:
                case FieldDataType.LongText:
                case FieldDataType.Text:
                    return obj.ToString();

                default:
                    using (MemoryStream stream = new MemoryStream())
                    {
                        BinaryWriter bw = new BinaryWriter(stream);
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(stream, obj);
                        return Convert.ToBase64String(stream.GetBuffer());
                    }
            }
        }

        ////////////////////////////////////////////////////////////////////////
    }
}

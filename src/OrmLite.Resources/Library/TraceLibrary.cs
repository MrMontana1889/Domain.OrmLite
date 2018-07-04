// TraceLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

#define TIMESTAMP_ALL_LINES

using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace OrmLite.Resource.Library
{
	/// <summary>
	/// Assigning numeric values here is not necessary but helps document
	/// the required TraceLibrary value in .config file.
	/// </summary>
	public enum CeTraceLevel { Off = 0, Release = 1, Debug = 2, Debug1 = 3, Debug2 = 4 };

	/// <summary>
	/// Static utility methods for enhancing the .NET standard Trace singleton.
	/// These also simplify and unify writing to the Haestad log or transcript,
	/// without referencing Haestad singletons.
	/// </summary>
	public sealed class TraceLibrary
	{
		#region Static Public Methods (Not Requiring Method Name)

		/// <summary>
		/// WARNING: Use this method very sparingly, because it does not require
		/// logging the originating class/method, which will make understanding
		/// the trace much more difficult.
		/// </summary>
		static public void WriteLine(CeTraceLevel ahtl, String astring)
		{
			WriteLineBare(ahtl, astring);
		}

		static public void WriteLoadedModules(CeTraceLevel ahtl)
		{
			StringWriter asw = new StringWriter(CultureInfo.InvariantCulture);
			ResourceProcessLibrary.ListLoadedModules(asw);

			WriteDividerLine(ahtl);
			WriteBare(ahtl, asw.ToString());
			WriteDividerLine(ahtl);
		}

		static public void WriteTimeStampLine(CeTraceLevel ahmtracelevel)
		{
			WriteTimeStampLine(ahmtracelevel, null);
		}

		static public void WriteTimeStampLine(CeTraceLevel ahmtracelevel, String astring)
		{
#if TIMESTAMP_ALL_LINES
			String astringLine = ResourceStringLibrary.IsBlank(astring) ? String.Empty : astring;
#else
				String astringLine = FormatDateTimeNow();
				if (!StringLibrary.IsBlank(astring))
					astringLine = astringLine + " " + astring;
#endif
			WriteLineBare(ahmtracelevel, astringLine);
		}

		static public void WriteDividerLine(CeTraceLevel ahmtracelevel)
		{
			WriteLineBare(ahmtracelevel, new String('=', 80));
		}

		static public void WriteEmptyLine(CeTraceLevel ahmtracelevel)
		{
			WriteLineBare(ahmtracelevel, String.Empty);
		}

		#endregion

		#region Static Public Methods (Requiring Method Name)

		static public void WriteLine(CeTraceLevel ahtl, Object aobject, String asMethod)
		{
			WriteLine(ahtl, aobject, asMethod, (String)null);
		}

		static public void WriteLine(CeTraceLevel ahtl, Object aobject, String asMethod, String asMessage)
		{
			WriteLine(ahtl, aobject.GetType(), asMethod, asMessage);
		}

		static public void WriteLine(CeTraceLevel ahtl, Type atype, String asMethod, String asMessage)
		{
			// return early to avoid unnecessary string operations
			if (!ShouldTrace(ahtl))
				return;
			WriteLineBare(
				ahtl,
				Format(atype, asMethod) + (asMessage == null ? "" : ": " + asMessage));
		}

		#endregion

		#region Static Public Methods (Taking Exceptions)

		static public void WriteLine(CeTraceLevel ahtl, Object aobject, String asMethod, Exception aexception)
		{
			WriteLine(ahtl, aobject, asMethod, aexception, null);
		}

		static public void WriteLine(CeTraceLevel ahtl, Object aobject, String asMethod, Exception aexception, String asMessage)
		{
			WriteLine(ahtl, aobject.GetType(), asMethod, aexception, asMessage);
		}

		static public void WriteLine(CeTraceLevel ahtl, Type atype, String asMethod, Exception aexception)
		{
			WriteLine(ahtl, atype, asMethod, aexception, null);
		}

		static public void WriteLine(CeTraceLevel ahtl, Type atype, String asMethod, Exception aexception, String asMessage)
		{
			// I18N OK <ccla>
			WriteLineBare(ahtl, "Exception in " + Format(atype, asMethod) +
				": " + aexception.GetType().Name + ": " + aexception.Message +
				(asMessage == null ? String.Empty : ": \"" + asMessage + "\""));
		}

		static public void WriteLine(CeTraceLevel ahtl, Object aobject, String asMethod, OleDbException aode)
		{
			// return early to avoid unnecessary string operations
			if (!ShouldTrace(ahtl))
				return;
			WriteLine(ahtl, aobject, asMethod, (Exception)aode);    // Cast is critical to avoid recursion and stack overflow!
			foreach (OleDbError aerror in aode.Errors)
			{
				// I18N OK <ccla>
				WriteBare(ahtl, "\t");
				WriteBare(ahtl, "Message: " + aerror.Message);
				WriteBare(ahtl, " Native: " + aerror.NativeError);
				WriteBare(ahtl, " Source: " + aerror.Source);
				WriteBare(ahtl, " SQL: " + aerror.SQLState);
				WriteLineBare(ahtl, "");
			}
		}

		#endregion

		#region Static Public Methods (Taking Other Objects)

		static public void WriteLine(CeTraceLevel ahtl, Object aobject, String asMethod, DataRow adr)
		{
			// return early to avoid unnecessary string operations
			if (!ShouldTrace(ahtl))
				return;
			// I18N OK <ccla>
			WriteLine(ahtl, aobject, asMethod, adr.ToString());
			WriteBare(ahtl, "\t");
			WriteBare(ahtl, "HasErrors: " + adr.HasErrors);
			WriteBare(ahtl, " HasVersion(Current): " + adr.HasVersion(DataRowVersion.Current));
			WriteBare(ahtl, " HasVersion(Default): " + adr.HasVersion(DataRowVersion.Default));
			WriteBare(ahtl, " HasVersion(Original): " + adr.HasVersion(DataRowVersion.Original));
			WriteBare(ahtl, " HasVersion(Proposed): " + adr.HasVersion(DataRowVersion.Proposed));
			WriteBare(ahtl, " RowError: \"" + adr.RowError + "\"");
			WriteBare(ahtl, " RowState: " + adr.RowState);
			WriteLineBare(ahtl, "");
		}

		#endregion

		#region Static Public Methods (WriteCaller)

		/// <summary>
		/// Write stack data about the caller to trace listeners. Use passed
		/// level to control what is considered the caller. Use 0 for the method 
		/// that immediately called this one, subtracting 1 for each method higher
		/// up the call stack.
		/// </summary>
		static public void WriteCaller(CeTraceLevel ahmtracelevel, int aiLevel)
		{
			StackLevel asl = StackLevel.FromEnvironment(5 - aiLevel);
			WriteLineBare(ahmtracelevel, asl.CallingMethod);
			WriteBare(ahmtracelevel, asl.SourcePath);
			WriteBare(ahmtracelevel, " ");
			WriteLineBare(ahmtracelevel, asl.LineNumber);
		}

		static public void WriteCallerHeader(CeTraceLevel ahmtracelevel, int aiLevel)
		{
			WriteDividerLine(ahmtracelevel);
			WriteTimeStampLine(ahmtracelevel);
			WriteCaller(ahmtracelevel, aiLevel - 1);
			WriteDividerLine(ahmtracelevel);
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Static Constructor: Use default level unless another level has been 
		/// set explicitly from the TraceLibrary value in the application-named 
		/// .config file. This means cannot completely turn off trace messages in 
		/// the field, but we have not yet ever needed to disable Release-level 
		/// messages in the field.
		/// </summary>
		static TraceLibrary()
		{
			TraceSwitch aswitch = new TraceSwitch("TraceLibrary", "");

			if ((CeTraceLevel)aswitch.Level != CeTraceLevel.Off)        // Means found .config file.
				CurrentTraceLevel = (CeTraceLevel)aswitch.Level;
			else
				CurrentTraceLevel = DefaultTraceLevel;
		}

		private TraceLibrary()
		{
		}

		#endregion

		#region Static Public Methods

		static public String FormatDateTimeNow()
		{
			return DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.ff", DateTimeFormatInfo.InvariantInfo); // I18N OK <ccla>
		}

		#endregion

		#region Static Private Methods

		static private String Format(Type atype, String asMethod)
		{
			return atype.Name + "." + asMethod;
		}

		static private bool ShouldTrace(CeTraceLevel alevel)
		{
			return alevel != CeTraceLevel.Off && alevel <= CurrentTraceLevel;
		}

		static private void WriteBare(CeTraceLevel alevel, String astring)
		{
			Trace.WriteIf(ShouldTrace(alevel), astring);
		}

		static private void WriteLineBare(CeTraceLevel alevel, String astring)
		{
			// return early to avoid unnecessary string operations
			if (!ShouldTrace(alevel))
				return;
#if TIMESTAMP_ALL_LINES
			String astringLine = astring; // I18N OK <ccla>
#else
				String astringLine = astring;
#endif
			Trace.WriteLine(astringLine);
		}

		#endregion

		#region Fields

		static public CeTraceLevel CurrentTraceLevel;

#if DEBUG
		static private CeTraceLevel DefaultTraceLevel = CeTraceLevel.Debug;
#else
        static private CeTraceLevel DefaultTraceLevel = CeTraceLevel.Release;
#endif
		#endregion

	}

	#region StackLevel

	/// <summary>
	/// Instances parse and report properties of a certain level in the call stack.
	/// </summary>
	public class StackLevel
	{
		#region Constructors

		static public StackLevel FromEnvironment(int aiLevel)
		{
			return new StackLevel(aiLevel, Environment.StackTrace);
		}

		public StackLevel(int aiLevel, String asStack)
		{
			try
			{
				String[] args = asStack.Split(new char[] { '\n' });     // Split at lines.
				String astringLine = args[aiLevel].Trim();

				astringLine = astringLine.Substring(3);                 // Skip "at ".
				int aiClose = astringLine.IndexOf(')');
				m_stringCallingMethod = astringLine.Substring(0, aiClose + 1);

				astringLine = astringLine.Substring(aiClose + 1).Trim();
				args = astringLine.Split(null);                         // Split at white space.
				m_stringSourcePath = args[1];                           // Skip "in ".
				if (m_stringSourcePath.EndsWith(":line")) // I18N OK <ccla>
					m_stringSourcePath = m_stringSourcePath.Substring(0, m_stringSourcePath.Length - 5);

				m_stringLineNumber = args[2];
			}
			catch (Exception) { /* Ignore. */ }
		}

		#endregion

		#region Public Properties

		public String CallingMethod
		{
			get { return Check(m_stringCallingMethod, "method"); }      // I18N OK <ccla>
		}

		public String SourcePath
		{
			get { return Check(m_stringSourcePath, "source"); }         // I18N OK <ccla>
		}

		public String LineNumber
		{
			get { return Check(m_stringLineNumber, "line"); }           // I18N OK <ccla>
		}

		#endregion

		#region Protected Methods

		protected String Check(String astring, String asMessage)
		{
			if (ResourceStringLibrary.IsBlank(astring))
				return "<no " + asMessage + ">";                        // I18N OK <ccla>
			else
				return astring;
		}

		#endregion

		#region Private Fields

		private String m_stringCallingMethod;
		private String m_stringSourcePath;
		private String m_stringLineNumber;

		#endregion
	}

	#endregion
}

// TextManager.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Resource.Library;
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text;

namespace OrmLite.Resource.Support
{
	public delegate void TextManagerAssemblyLoadingDelegate(TextManagerEventArgs e);

	/// <summary>
	/// Instances represent a pool of resource strings and
	/// provide formatting and parameter substitution services.
	/// Maintains 2nd dictionary for runtime-defined strings.
	/// </summary>
	public class TextManager
	{
		#region Static Public Events
		public static event TextManagerAssemblyLoadingDelegate TextManagerAssemblyLoading;
		#endregion

		#region Static Public Methods

		/// <summary>
		/// Text management too fundamental to be owned by application session.
		/// Locate singleton here, so always available to test case, independent,
		/// and startup code.
		/// </summary>
		static public TextManager Current
		{
			get
			{
				if (CurrentTextManager == null)
				{
					lock (syncRoot)
					{
						if (CurrentTextManager == null)
							CurrentTextManager = new TextManager();
					}
				}

				return CurrentTextManager;
			}
		}

		#endregion

		#region Constructors

		// Modified by Jdec.  I fill the hashtable in this constructor because
		// the look up for the assembly resource is faster using this method 
		// then the previous method which relied on exception handling.
		private TextManager()
		{
			InitializeResourceManagers();
			AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(HandleAssemblyLoad);
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Look up string template value for passed key, favoring any runtime
		/// definitions over static resources.
		/// </summary>
		public String this[String astringName, params String[] argstring]
		{
			get
			{
				return this[astringName, Assembly.GetCallingAssembly(), argstring];
			}
		}

		/// <summary>
		/// Build an array of viewable strings, by looking up the passed array
		/// of key objects, assumed to be strings.
		/// </summary>
		public String[] this[Object[] argoName]
		{
			get
			{
				String[] argstringValue = new String[argoName.Length];
				Assembly aassembly = Assembly.GetCallingAssembly();

				for (int ai = 0; ai < argoName.Length; ai++)
					argstringValue[ai] = this[(String)argoName[ai], aassembly];

				return argstringValue;
			}
		}

		public String this[String astringName, Assembly aassemblyCaller, params String[] argstring]
		{
			get { return MessageFromParms(GetString(astringName, aassemblyCaller), argstring); }
		}

		// Added by Whar 8/13/2010.
		/// <summary>
		/// Method to get a resource string template without having parameter place holders (e.g., %1) replaced
		/// with arguments passed in. Generally, you don't want to call this method unless you have a use case that 
		/// is hammering TextManager and you want to cache the pre-processed string resource, which is why this 
		/// method is not exposed as another this[] overload, avoiding it accidentally be called in lieu of another 
		/// this[] overload. Fyi, profiling this approach (caching the pre-processed string resource on the client side) 
		/// and calling MessageFromParms as required, is 6x faster than calling into TextManager. For strings that require
		/// no parameter lookup caching on the client side is 11x faster (if done right). Obviously given the absolute 
		/// times involved that benefit is only realized over a large number of calls, but faster is faster, in anyone's 
		/// language and I have a use case that hits text manager quite a bit.
		/// </summary>
		/// <param name="astringName">The key to be looked up.</param>
		/// <returns>The raw resource string.</returns>
		public String GetStringWithoutParameterLookup(string astringName)
		{
			return GetString(astringName, Assembly.GetCallingAssembly());
		}

		/// <summary>
		/// Return a formatted string with debugging information about the internal
		/// state of the TextManager, ResourceManagers, and CultureInfos.
		/// </summary>
		/// <returns></returns>
		public String DebugString()
		{
			StringBuilder asb = new StringBuilder();
			asb.Append(DateTime.Now);
			asb.Append("\n");

			foreach (DictionaryEntry ade in ResourceManagers)
			{
				Assembly aassembly = (Assembly)ade.Key;
				ResourceManager arm = (ResourceManager)ade.Value;

				asb.AppendFormat(CultureInfo.InvariantCulture, "Assembly: {0}\n", aassembly.Location);
				foreach (String asResourceName in aassembly.GetManifestResourceNames())
					asb.AppendFormat(CultureInfo.InvariantCulture, "\t\tResource: {0}\n", asResourceName);

				asb.AppendFormat(CultureInfo.InvariantCulture, "\tResourceManager: {0}\n", arm is NullResourceManager ? "<NullResourceManager>" : arm.BaseName);
				if (!(arm is NullResourceManager))
					foreach (CultureInfo aci in CultureInfo.GetCultures(CultureTypes.AllCultures))
					{
						if (arm.GetResourceSet(aci, true, false) != null)
							asb.AppendFormat(CultureInfo.InvariantCulture, "\t\tCultureInfo: {0}\n", aci.Name.Length == 0 ? "<InvariantCulture>" : aci.Name);
					}
			}

			return asb.ToString();
		}

		public String GetShortString(String astringName, params String[] argstring)
		{
			Assembly aasemblyCaller = Assembly.GetCallingAssembly();

			return MessageFromParms(GetShortString(astringName, aasemblyCaller), argstring);
		}

		public void PutString(String astringName, String astringValue)
		{
			Assembly acallerAssembly = Assembly.GetCallingAssembly();
			StringDictionary asd = (StringDictionary)CustomStringDictionaries[acallerAssembly];
			if (asd == null)
			{
				asd = new StringDictionary();

				//rgur 03/2016 - LookUpAllLoadedResourceManagers can fail if this string dictionary gets modified while enumerating.
				//(Pretty rare, but has been happening intermittently lately -- perhaps related to adding Telerik assemblies for the ribbon).
				//So, if iterating, make a copy of the string dictionary which can be modified safely.
				if (m_isIteratingCustomStringDictionaries)
					m_customstringdictionaries = (Hashtable)m_customstringdictionaries.Clone();
				CustomStringDictionaries[acallerAssembly] = asd;
			}
			asd[astringName] = astringValue;
		}

		#endregion

		#region Public Static Methods
		//TODO 3: consider using String.Format() for this?
		/// <summary>
		/// Plug the astringTemplate with the parms provided. Plug locations 
		/// in the astringTemplate are marked by %n, where n is a single digit, 
		/// equal to the index in argstring that should be used to plug the 
		/// astringTemplate. If there are not enough parms, substitute a default 
		/// string. Extended to support meta-chars for embedded carriage
		/// returns and tabs.  
		/// </summary>
		public static String MessageFromParms(String astringTemplate, String[] argstring)
		{
			String astringDefault = "??";
			String astringMessage = "";
			int aintIndexLast = 0;
			int aintIndex = 0;
			int aintNumberOfParms = argstring.Length;
			int aintSize = astringTemplate.Length;

			// Note: <whar> I profiled this against using a StringBuilder (in lieu of +=) and whilst StringBuilder is
			// marginally faster the amount by which it is faster did not justify making any changes. This code as-is, is
			// significantly faster than an approach using String.Replace.

			while (aintIndex < aintSize)
			{
				if (astringTemplate[aintIndex] == '%')
				{
					char acharNext = astringTemplate[aintIndex + 1];
					astringMessage = astringMessage + astringTemplate.Substring(aintIndexLast, aintIndex - aintIndexLast);

					if (Char.IsDigit(acharNext))
					{
						int aintIndexParm = ResourceStringLibrary.DigitValue(acharNext) - 1;
						astringMessage = astringMessage +
							(aintIndexParm < aintNumberOfParms ?
							argstring[aintIndexParm] : astringDefault);
					}
					else if (acharNext == 'n')
						astringMessage += "\n";
					else if (acharNext == 'r')
						astringMessage += "\r";
					else if (acharNext == 't')
						astringMessage += "\t";
					else
						astringMessage += acharNext.ToString(CultureInfo.CurrentCulture);

					//Skip over the %n in the string.
					aintIndex += 2;
					aintIndexLast = aintIndex;
				}
				else
					aintIndex++;
			}
			return astringMessage + astringTemplate.Substring(aintIndexLast);
		}
		#endregion

		#region Static Private Methods
		private void HandleAssemblyLoad(object sender, AssemblyLoadEventArgs args)
		{
			AddResourceManagerFor(args.LoadedAssembly);
		}
		#endregion

		#region Private Methods

		private String GetShortString(String astringName, Assembly aasembly)
		{
			String astring = GetStringBare(astringName + SHORT_SUFFIX, aasembly);
			if (astring == null)
				astring = GetString(astringName, aasembly);
			return astring;
		}

		private String GetString(String astringName, Assembly aassemblyCaller)
		{
			String astring = GetStringBare(astringName, aassemblyCaller);
			if (astring == null)
			{
				TraceLibrary.WriteLine(CeTraceLevel.Debug, this,
					"TextManager.GetString: missing " + astringName);// I18N OK <ccla>
				astring = MISSING_PREFIX + astringName;
			}
			return astring;
		}

		// Modified by Jdec.  I added the lookup from a hashtable instead of 
		// trying to read an element and catching an exception.  My initial 
		// measurements show that this method is over 300 times faster than 
		// using the exception handler.
		/// <summary>
		/// WARNING: if assembly name does not match default namespace (see project properties),
		/// then the naming convention implemented here gets tricky, because we use Assembly.GetName()
		/// to derive the default namespace under which any resources are expected to be embedded.
		/// Our convention is that all HMI namespaces should begin with MNYX., and all HMI
		/// DLL names should begin with MNYX., however HMI EXE names may begin with just a product name.
		/// Thus special logic is implemented here for EXE's that don't begin with MNYX.
		/// </summary>
		private ResourceManager AddResourceManagerFor(Assembly aassembly)
		{
			if (ResourceReflectionLibrary.IsDynamicAssembly(aassembly))
				return null;

			// DKir 4/28/2005
			// Problem during enumeration in LookUpAllLoadedResourceManagers() when HandleAssemblyLoad() called 
			// implicitly from ResourceManager.GetString(): satellite assembly is added to ResourceManagers 
			// causing InvalidOperationException because collection was modified during enumeration. Attempt 
			// to detect satellite assembly by using suffix. (Warning: logic could fail on a non-satellite assembly 
			// with a similar suffix.) Note the satellite ResourceManagers apparently aren't needed here: 
			// strings are loaded implicitly via the neutral resource. 
			if (aassembly.Location.ToLower(CultureInfo.InvariantCulture).EndsWith(SATELLITE_ASSEMBLY_SUFFIX))
				return null;

			String astringPath = aassembly.GetName().Name + CE_STRINGS_SUFFIX;

			if (aassembly.Location.ToLower(CultureInfo.InvariantCulture).EndsWith(".exe") && !astringPath.StartsWith(CE_NAMESPACE_PREFIX))
				astringPath = CE_NAMESPACE_PREFIX + astringPath;

			String resourceName = astringPath + RESOURCE_EXTENSION;
			bool aboolContainsMNYXStrings = false;
			string[] names = aassembly.GetManifestResourceNames();
			for (int j = 0; j < names.Length; j++)
			{
				if (names[j] == resourceName)
				{
					aboolContainsMNYXStrings = true;
					break;
				}
			}

			if (aboolContainsMNYXStrings)
			{
				//Only execute the event if the assembly has LocalizedStrings.
				//Don't want the client to include if it doesn't have them!
				TextManagerEventArgs args = new TextManagerEventArgs(aassembly, aboolContainsMNYXStrings);
				if (TextManagerAssemblyLoading != null)
					TextManagerAssemblyLoading(args);

				//If modified, then reset the local variable.  This means it was changed from true to false
				//as the event is only ever fired if there ARE LocalizedString resources.
				if (args.IncludeModified)
					aboolContainsMNYXStrings = args.IncludeAssembly;
			}

			ResourceManager arm;
			if (aboolContainsMNYXStrings)
			{
				arm = new ResourceManager(astringPath, aassembly);
			}
			else
			{
				arm = new NullResourceManager();
			}

			lock (m_resourceManagersLock)
			{
				//rgur 03/2016 - LookUpAllLoadedResourceManagers can fail if this resource dictionary gets modified while enumerating.
				//(Pretty rare, but has been happening intermittently lately -- perhaps related to adding Telerik assemblies for the ribbon).
				//So, if iterating, make a copy of the resource dictionary which can be modified safely here.
				if (m_isIteratingResourceManagers)
					m_resourcemanagers = (Hashtable)m_resourcemanagers.Clone();
				ResourceManagers[aassembly] = arm;
			}

			return arm;
		}

		private void InitializeResourceManagers()
		{
			m_resourcemanagers = new Hashtable();

			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

			for (int i = 0; i < assemblies.Length; i++)
			{
				Assembly aassembly = assemblies[i];
				AddResourceManagerFor(aassembly);
			}
		}

		private String LookUpAllLoadedResourceManagers(String astringName)
		{
			lock (m_customStringDictionariesLock)
			{
				bool wasIteratingCustomStringDictionaries = m_isIteratingCustomStringDictionaries;
				try
				{
					m_isIteratingCustomStringDictionaries = true;

					IDictionaryEnumerator aenum = CustomStringDictionaries.GetEnumerator();
					StringDictionary asd;
					String astring;
					while (aenum.MoveNext())
					{
						asd = (StringDictionary)aenum.Value;
						astring = asd[astringName];
						if (astring != null)
							return astring;
					}
				}
				finally
				{
					m_isIteratingCustomStringDictionaries = wasIteratingCustomStringDictionaries;
				}
			}

			lock (m_resourceManagersLock)
			{
				bool wasIteratingResourceManagers = m_isIteratingResourceManagers;
				try
				{
					m_isIteratingResourceManagers = true;

					IDictionaryEnumerator aenum = ResourceManagers.GetEnumerator();
					ResourceManager arm;
					while (aenum.MoveNext())
					{
						arm = (ResourceManager)aenum.Value;
						String astring = arm.GetString(astringName);
						if (astring != null)
							return astring;
					}
				}
				finally
				{
					m_isIteratingResourceManagers = wasIteratingResourceManagers;
				}
				return null;
			}
		}

		private String GetStringBare(String astringName, Assembly aassemblyCaller)
		{
			lock (this)
			{
				String astring = null;

				StringDictionary asd = (StringDictionary)CustomStringDictionaries[aassemblyCaller];
				if (asd != null)
					astring = asd[astringName];
				if (astring == null)
				{
					ResourceManager arm = (ResourceManager)ResourceManagers[aassemblyCaller];
					if (arm == null)
						arm = AddResourceManagerFor(aassemblyCaller);

					if (arm != null)
						astring = arm.GetString(astringName);

					if (astring == null)
						astring = LookUpAllLoadedResourceManagers(astringName);
				}
				return astring;
			}
		}

		private IDictionary ResourceManagers
		{
			get { return m_resourcemanagers; }
		}

		private IDictionary CustomStringDictionaries
		{
			get { return m_customstringdictionaries; }
		}

		#endregion

		#region Static Private Fields

		static private Object syncRoot = new Object(); // Object to ensure thread-safe execution.

		static private readonly String CE_NAMESPACE_PREFIX = "CE.";
		static private readonly String CE_STRINGS_SUFFIX = ".Resources.LocalizedStrings";
		static private readonly String RESOURCE_EXTENSION = ".resources";
		static private readonly String MISSING_PREFIX = "#";
		static private readonly String SATELLITE_ASSEMBLY_SUFFIX = ".resources.dll";
		static private readonly String SHORT_SUFFIX = "_short";
		static private TextManager CurrentTextManager;

		#endregion

		#region Private Fields

		private Hashtable m_resourcemanagers;
		private Hashtable m_customstringdictionaries = new Hashtable();
		private bool m_isIteratingResourceManagers = false;
		private bool m_isIteratingCustomStringDictionaries = false;
		private object m_resourceManagersLock = new Object();
		private object m_customStringDictionariesLock = new Object();

		#endregion
	}
}

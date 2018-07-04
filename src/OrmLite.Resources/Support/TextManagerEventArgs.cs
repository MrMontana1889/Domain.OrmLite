// TextManagerEventArgs.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Reflection;

namespace OrmLite.Resource.Support
{
	/// <summary>
	/// Event arguments used with TextManager's static event
	/// that allows the client to override the IncludeAssembly
	/// flag.  This is primarily for purposes of resolving clashing
	/// values for the key in multiple assemblies.
	/// </summary>
	public class TextManagerEventArgs : EventArgs
	{
		#region Constructor
		public TextManagerEventArgs(Assembly assembly, bool includeAssembly)
		{
			m_assembly = assembly;
			m_boolIncludeAssembly = includeAssembly;
			m_boolIncludeModified = false;
		}
		#endregion

		#region Public Properties
		public Assembly Assembly
		{
			get { return m_assembly; }
		}
		public bool IncludeAssembly
		{
			get { return m_boolIncludeAssembly; }
			set
			{
				if (m_boolIncludeAssembly != value)
				{
					m_boolIncludeAssembly = value;
					m_boolIncludeModified = true;
				}
			}
		}
		internal bool IncludeModified
		{
			get { return m_boolIncludeModified; }
		}
		#endregion

		#region Private Fields
		private Assembly m_assembly;
		private bool m_boolIncludeAssembly;
		private bool m_boolIncludeModified;
		#endregion
	}
}

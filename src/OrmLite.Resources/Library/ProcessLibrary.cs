// ProcessLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace OrmLite.Resource.Library
{
	public static class ResourceProcessLibrary
	{
		/// <summary>
		/// Write all the loaded modules in the current process (managed and unmanaged) 
		/// with file versions, locations and sizes. Provides more information than the 
		/// regular exception window that .NET displays (which only shows managed modules).
		/// </summary>
		static public void ListLoadedModules(TextWriter atw)
		{
			foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
			{
				string modulePath = module.FileName;
				if (!File.Exists(modulePath)) continue;
				using (FileStream afs = File.OpenRead(modulePath))
				{
					// I18N OK <ccla>
					atw.Write(
						String.Format(CultureInfo.InvariantCulture, "\r\n{0}Size:             {1} bytes\r\n",
						module.FileVersionInfo, afs.Length));
				}
			}
		}
	}
}

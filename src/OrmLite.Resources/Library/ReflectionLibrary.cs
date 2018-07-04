// ReflectionLibrary.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Reflection;

namespace OrmLite.Resource.Library
{
	public static class ResourceReflectionLibrary
	{
		/// <summary>
		/// Check if an assembly is dynamic.
		/// </summary>
		/// <remarks>
		/// For dynamic assemblies accessing the Location property will throw a <see cref="NotSupportedException"/>.
		/// This method is intended to check upfront and avoid code dealing with the exception.
		/// </remarks>
		/// <param name="aassembly">Assembly to test</param>
		/// <returns>true if the assembly is dynamic otherwise false</returns>
		/// <see cref="http://stackoverflow.com/questions/1423733/how-to-tell-if-a-net-assembly-is-dynamic"/>
		static public bool IsDynamicAssembly(Assembly aassembly)
		{
			if (Environment.Version.Major > 3)
			{
				// under .NET Framework 4 and higher there is a property on the assembly available to query
				// Since we build for runtime version 3.5 we canot use it and the test below is not applicable since 
				// the class we are checking for has changed. So using reflection to obtain the Information
				PropertyInfo propInfo = aassembly.GetType().GetProperty("IsDynamic", BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
				return (bool)propInfo.GetValue(aassembly, null);
			}
			else
			{
				return aassembly.ManifestModule is System.Reflection.Emit.ModuleBuilder;
			}
		}
	}
}

// Interfaces.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Support;
using System;
using System.Data;

namespace OrmLite.Domain.DataObjects
{
	public interface IDataConnection : IDisposable
	{
		void New(string filename);
		void Open(string filename);
		bool IsOpen();
		bool Backup(string filename);
		void Flush();
		void Close();
	}

	public interface ICreatableDomainTable : INamable
	{
		bool Create(IDbConnection dbConnection);
		int Create_Index(IDbConnection dbConnection);
	}
}

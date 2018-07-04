// Interfaces.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Domain.DataObjects.Sqlite;

namespace OrmLite.Domain.ModelingObjects
{
	internal interface IRepositoryFactory
	{
		IDomainTableRepository<TTableType> CreateRepository<TTableType>(ISqliteDataConnection dataConnection, string name = "") where TTableType : class, IDomainTable;
	}
}

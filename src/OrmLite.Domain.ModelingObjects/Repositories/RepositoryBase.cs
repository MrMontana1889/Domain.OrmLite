﻿// RepositoryBase.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Domain.DataObjects.Sqlite;
using System.Data;

namespace OrmLite.Domain.ModelingObjects.Repositories
{
	public abstract class RepositoryBase : IRepository
	{
		#region Constructor
		public RepositoryBase(ISqliteDataConnection dataConnection)
		{
			Connection = dataConnection;
		}
		#endregion

		#region Public Properties
		public abstract string Name { get; }
		#endregion

		#region Protected Properties
		protected ISqliteDataConnection Connection
		{
			get;
			private set;
		}
		protected IDbConnection DbConnection
		{
			get { return Connection.DbConnection; }
		}
		#endregion
	}
}

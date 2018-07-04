// DataRepository.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Domain.DataObjects;
using OrmLite.Domain.DataObjects.Sqlite;
using OrmLite.Domain.ModelingObjects.Repositories;

namespace OrmLite.Domain.ModelingObjects
{
	public class DataRepository : IDataRepository
	{
		#region Constructor
		public DataRepository(IDataConnection dataConnection)
		{
			DataConnection = dataConnection as ISqliteDataConnection;
		}
		#endregion

		#region Public Methods
		public IDomainTableRepository<TTableType> GetTableRepositoryFor<TTableType>(string name) where TTableType : class, IDomainTable
		{
			return RepositoryFactory.CreateRepository<TTableType>(DataConnection, name);
		}
		#endregion

		#region Private Properties
		private ISqliteDataConnection DataConnection
		{
			get;
			set;
		}
		private IRepositoryFactory RepositoryFactory
		{
			get
			{
				if (_factory == null)
					_factory = new RepositoryFactory(this);
				return _factory;
			}
		}
		#endregion

		#region Private Fields
		private IRepositoryFactory _factory;
		#endregion
	}
}

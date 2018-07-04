// RepositoryFactory.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Domain.DataObjects.Sqlite;

namespace OrmLite.Domain.ModelingObjects.Repositories
{
	internal class RepositoryFactory : IRepositoryFactory
	{
		#region Constructor
		public RepositoryFactory(IDataRepository dataRepository)
		{
			DataRepository = dataRepository;
		}
		#endregion

		#region Public Methods
		public IDomainTableRepository<TTableType> CreateRepository<TTableType>(ISqliteDataConnection dataConnection, string name = "") where TTableType : class, IDomainTable
		{
			if (string.IsNullOrEmpty(name))
			{
				TTableType domainTable = System.Activator.CreateInstance<TTableType>();
				if (domainTable != null)
					name = domainTable.Name;
			}

			switch (name)
			{
				default:
					return new GenericTableRepository<TTableType>(name, dataConnection);
			}
		}
		#endregion

		#region Private Properties
		private IDataRepository DataRepository
		{
			get;
			set;
		}
		#endregion
	}
}

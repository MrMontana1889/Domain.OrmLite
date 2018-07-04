// RepositoryDataSource.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Domain.DataObjects;
using OrmLite.Domain.DataObjects.Sqlite;

namespace OrmLite.Domain.ModelingObjects
{
	public class RepositoryDataSource : DataSourceBase, IRepositoryDataSource
	{
		#region Constructor
		public RepositoryDataSource()
		{

		}
		#endregion

		#region Public Properties
		public IDataRepository DataRepository
		{
			get
			{
				if (dataRepository == null)
					dataRepository = NewDataRepository();
				return dataRepository;
			}
		}
		public IDataConnection GetDataConnection()
		{
			return DataConnection;
		}
		#endregion

		#region Protected Methods
		protected override void Dispose(bool disposing)
		{
			dataRepository = null;
			base.Dispose(disposing);
		}
		protected virtual IDataRepository NewDataRepository()
		{
			return new DataRepository(DataConnection);
		}
		protected override IDataConnection NewDataConnection()
		{
			return new SqliteDataConnection();
		}
		#endregion

		#region Private Fields
		private IDataRepository dataRepository;
		#endregion
	}
}

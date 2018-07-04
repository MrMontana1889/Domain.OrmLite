// DataSourceBase.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Domain.DataObjects;
using OrmLite.Domain.DataObjects.Sqlite.Library;
using OrmLite.Resource.Support;
using System;
using System.IO;

namespace OrmLite.Domain.ModelingObjects
{
	public abstract class DataSourceBase : IDataSource
	{
		#region Constructor
		public DataSourceBase()
		{

		}
		#endregion

		#region Public Methods
		public void New(string filename)
		{
			if (File.Exists(filename)) throw new FileLoadException(filename);
			DataConnection.New(filename);
		}
		public void Open(string filename)
		{
			if (IsOpen())
			{
				Close();
			}

			DataConnection.Open(filename);
		}
		public bool IsOpen()
		{
			if (dataConnection != null)
				return DataConnection.IsOpen();
			return false;
		}
		public bool Backup(string filename)
		{
			if (!IsOpen()) return false;

			//Must be open in order to execute backup.
			return DataConnection.Backup(filename);
		}
		public void Flush()
		{
			if (!IsOpen()) throw new InvalidOperationException(TextManager.Current["ManagerIsNotOpen"]);
			DataConnection.Flush();
		}
		public void Close()
		{
			Dispose();
		}
		public void Dispose()
		{
			Dispose(true);
		}
		#endregion

		#region Protected Methods
		protected abstract IDataConnection NewDataConnection();
		protected virtual void ResetConnection()
		{
			dataConnection = null;
		}
		protected virtual void Dispose(bool disposing)
		{
			if (dataConnection != null)
			{
				DataConnection.Close();
				DataConnection.Dispose();
			}
			ResetConnection();
		}
		#endregion

		#region Protected Properties
		protected IDataConnection DataConnection
		{
			get
			{
				if (dataConnection == null)
					dataConnection = NewDataConnection();
				return dataConnection;
			}
		}
		#endregion

		#region Private Fields
		private IDataConnection dataConnection;
		#endregion
	}
}

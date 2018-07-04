// SqliteDataConnection.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Domain.DataObjects.Sqlite.Library;
using OrmLite.Domain.DataObjects.Tables;
using OrmLite.Resource.Library;
using OrmLite.Resource.Support;
using ServiceStack.OrmLite;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Timers;

namespace OrmLite.Domain.DataObjects.Sqlite
{
	public class SqliteDataConnection : IDataConnection, ISqliteDataConnection
	{
		#region Constructor
		public SqliteDataConnection()
		{
			RegisterLicense();
			RegisterConverters();

			Filename = string.Empty;

			EnableAutoFlush();
		}
		#endregion

		#region Public Methods
		public virtual void New(string filename)
		{
			if (IsOpen()) throw new InvalidOperationException(TextManager.Current["ConnectionAlreadyOpen"]);
			if (File.Exists(filename)) throw new FileLoadException(filename);

			SqliteLibrary.CreateSqliteDatabase(filename);
			Filename = filename;
			LoadSchema();
			Open(filename);
		}
		public void Open(string filename)
		{
			Filename = filename;
			OpenImpl();
		}
		public bool IsOpen()
		{
			return DbConnection != null && DbConnection.State == ConnectionState.Open;
		}
		public bool Backup(string filename)
		{
			if (!IsOpen()) return false;

			Flush(false);       //Flushign any pending changes but do not create a new transaction.

			using (SQLiteConnection conn = new SQLiteConnection(SqliteLibrary.GetConnectionString(filename)))
			{
				try
				{
					SQLiteConnection db = DbConnection.ToDbConnection() as SQLiteConnection;
					conn.Open();
					db.BackupDatabase(conn, "main", "main", -1, null, 0);
				}
				finally
				{
					conn.Close();
				}
			}

			//A backup to an in-memory database returns false
			if (filename == ":memory:")
				return false;

			return File.Exists(filename);
		}
		public void Flush()
		{
			Flush(true);
		}
		public void Flush(bool newTransaction)
		{
			lock (this)
			{
				if (currentTransaction != null) currentTransaction.Commit();
				currentTransaction = null;
				if (newTransaction && DbConnection != null && DbConnection.State != ConnectionState.Closed) currentTransaction = DbConnection.BeginTransaction();
			}
		}
		public void Close()
		{
			CloseConnection();
		}
		public void Dispose()
		{
			Dispose(true);
		}
		public void EnableAutoFlush()
		{
			m_flushTimer = new Timer(FlushFrequencyInMilliseconds);
			m_flushTimer.Elapsed += new ElapsedEventHandler(HandleFlushTimerElapsed);
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(HandleUnhandledException);
		}
		public void ExecuteAutoFlush()
		{
			// Only flush (i.e. commit transaction) if one is pending (based on the Timer)
			// and not under any bulk-mode.
			if (!m_flushPending) return;
			TraceLibrary.WriteLine(CeTraceLevel.Release, "SqliteDataConnectionBase.ExecuteAutoFlush");
			Flush();
		}
		public void DisableAutoFlush()
		{
			StopFlushTimer();

			if (m_flushTimer != null)
			{
				AppDomain.CurrentDomain.UnhandledException -= new UnhandledExceptionEventHandler(HandleUnhandledException);
				m_flushTimer.Elapsed -= new ElapsedEventHandler(HandleFlushTimerElapsed);
				m_flushTimer.Dispose();
			}

			m_flushTimer = null;
		}
		#endregion

		#region Public Properties
		public IDbConnection DbConnection
		{
			get;
			private set;
		}
		#endregion

		#region Protected Methods
		protected virtual void RegisterConverters()
		{

		}
		protected virtual void RegisterLicense()
		{

		}
		protected virtual void OpenImpl()
		{
			if (!File.Exists(Filename)) throw new FileNotFoundException(Filename);
			if (!IsDataSourceImpl())
			{
				CloseConnection();
				throw new FileLoadException(TextManager.Current["DomainDatabaseFormatNotRecognized"]);
			}
			OpenConnection(false);
		}
		protected void OpenConnection(bool openExclusively)
		{
			if (!IsOpen())
			{
				OrmLiteConfig.DialectProvider = SqliteDialect.Provider;
				OrmLiteConfig.DialectProvider.NamingStrategy = new OrmLiteNamingStrategyBase();

				ConnectionFactory.ConnectionString = SqliteLibrary.GetConnectionString(Filename);
				DbConnection = ConnectionFactory.OpenDbConnection();
				currentTransaction = DbConnection.BeginTransaction();
				StartFlushTimer();
			}
		}
		protected void CloseConnection()
		{
			if (DbConnection != null)
			{
				StopFlushTimer();

				Flush(false);

				DbConnection.Close();
				DbConnection.Dispose();
			}

			DbConnection = null;
			connectionFactory = null;
		}
		protected virtual bool IsDataSourceBasic()
		{
			return DbConnection.TableExists(SchemaInfoTableName);
		}
		protected void LoadSchema()
		{
			try
			{
				OpenConnection(true);
				LoadSchemaImpl();
			}
			finally
			{
				CloseConnection();
			}
		}
		protected virtual void LoadSchemaImpl()
		{
			InfoTable infoTable = new InfoTable();
			infoTable.Create(DbConnection);
			infoTable.Create_Index(DbConnection);
		}
		protected virtual void Dispose(bool disposing)
		{
			CloseConnection();
		}
		#endregion

		#region Protected Properties
		protected virtual string SchemaInfoTableName
		{
			get { return SchemaTableName.INFOTABLE_V1; }
		}
		protected string Filename
		{
			get;
			set;
		}
		#endregion

		#region Private Methods
		protected bool IsDataSourceImpl()
		{
			OpenConnection(false);
			return IsDataSourceBasic();
		}
		private void StartFlushTimer()
		{
			if (m_flushTimer != null) m_flushTimer.Start();
		}
		private void StopFlushTimer()
		{
			m_flushPending = false;
			if (m_flushTimer != null) m_flushTimer.Stop();
		}
		#endregion

		#region Private Properties
		private OrmLiteConnectionFactory ConnectionFactory
		{
			get
			{
				if (connectionFactory == null)
					connectionFactory = new OrmLiteConnectionFactory() { AutoDisposeConnection = true };
				return connectionFactory;
			}
		}
		#endregion

		#region Private Fields
		private OrmLiteConnectionFactory connectionFactory;
		private IDbTransaction currentTransaction;
		private const double FlushFrequencyInMilliseconds = 30000.0;
		private Timer m_flushTimer;
		private bool m_flushPending = false;
		#endregion

		#region Event Handlers
		protected virtual void HandleFlushTimerElapsed(object sender, ElapsedEventArgs e)
		{
			m_flushPending = true;
		}
		private void HandleUnhandledException(object sender, UnhandledExceptionEventArgs t)
		{
			if (DbConnection.State != ConnectionState.Open) return;

			// Ensuring regular transaction is committed in the case of a UE to avoid
			// any data-loss.

			try
			{
				Flush();
			}
			catch (Exception ex)
			{
				TraceLibrary.WriteLine(CeTraceLevel.Release, ex.Message);
			}
		}
		#endregion
	}
}

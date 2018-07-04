// Interfaces.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Support;
using ServiceStack.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace OrmLite.Domain
{
	public interface IDataSource : IDisposable
	{
		void New(string filename);
		void Open(string filename);
		bool IsOpen();
		bool Backup(string filename);
		void Flush();
		void Close();
	}

	public interface IRepositoryDataSource : IDataSource
	{
		IDataRepository DataRepository { get; }
	}

	public interface IDomainTable : INamable, IHasId<int>
	{
		new int Id { get; set; }
		bool IsNew { get; }
	}

	public interface IEditableElement
	{
		IList<IField> SupportedFields();
		IField Field(string name);
	}

	public interface IRepository : INamable
	{

	}

	public interface IDomainTableRepository<TTableType> : IRepository where TTableType : class, IDomainTable
	{
		IList<int> IDs();

		TTableType Add(TTableType item);

		List<TTableType> Items();
		List<TTableType> Items(Expression<Func<TTableType, bool>> predicate);
		List<TTableType> Items(IEnumerable ids);

		TTableType Load(int id);
		List<TTableType> LoadItems();
		List<TTableType> LoadItems(Expression<Func<TTableType, bool>> predicate);

		TTableType this[int id] { get; }
		TTableType Find(int id);
		TTableType Find(Expression<Func<TTableType, bool>> expression);
		TTableType Update(TTableType item);
		bool Exists(Expression<Func<TTableType, bool>> expression);

		void Remove(int id);

		TTableType Save(TTableType item);
		int SaveAll(IEnumerable<TTableType> items);

		int GetCount();
		int GetCount(Expression<Func<TTableType, bool>> expression);
	}

	public interface IDataRepository
	{
		IDomainTableRepository<TTableType> GetTableRepositoryFor<TTableType>(string name = "") where TTableType : class, IDomainTable;
	}
}

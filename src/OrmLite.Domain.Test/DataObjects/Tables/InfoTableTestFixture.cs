// InfoTableTestFixture.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using FluentAssertions;
using NUnit.Framework;
using OrmLite.Domain.DataObjects.Tables;
using OrmLite.Domain.ModelingObjects;
using OrmLite.Support.Support;
using System.Collections.Generic;
using System.IO;

namespace OrmLite.Domain.Test.DataObjects.Tables
{
	[TestFixture]
	public class InfoTableTestFixture : DomainTestFixtureBase
	{
		#region Constructor
		public InfoTableTestFixture()
		{

		}
		#endregion

		#region Tests
		[Test]
		public void TestInfoTable()
		{
			using (RepositoryDataSource dataSource = new RepositoryDataSource())
			{
				string filename = Path.GetTempFileName();
				File.Delete(filename);
				File.Exists(filename).Should().BeFalse();

				try
				{
					dataSource.New(filename);
					dataSource.IsOpen().Should().BeTrue();

					InfoTable info = new InfoTable { InfoName = "Setting1", Value = "V1" };
					IDomainTableRepository<InfoTable> repository = dataSource.DataRepository.GetTableRepositoryFor<InfoTable>();
					repository.Should().NotBeNull();

					info = repository.Save(info);
					info.Id.Should().NotBe(0);

					InfoTable info2 = new InfoTable();
					info2 = repository[info.Id];
					info2.Id.Should().Be(info.Id);
					info2.InfoName.Should().Be(info.InfoName);
					info2.Value.Should().Be(info.Value);

					IList<IField> fields = info.SupportedFields();
					fields.Should().NotBeNull();

					fields.Count.Should().Be(2);

					fields[0].Name.Should().Be(StandardFieldName.InfoName);
					fields[1].Name.Should().Be(StandardFieldName.Value);

					((IEditField)fields[0]).SetValue(0, "Test2");
					((IEditField)fields[1]).SetValue(0, "Test3");

					info.InfoName.Should().Be("Test2");
					info.Value.Should().Be("Test3");

					repository.Save(info);
					info2.Should().NotBe(info);

					IField infoNameField = info.Field(StandardFieldName.InfoName);
					infoNameField.GetValue(0).Should().Be("Test2");

					IField valueField = info.Field(StandardFieldName.Value);
					valueField.GetValue(0).Should().Be("Test3");
				}
				finally { dataSource.Close(); }

				File.Delete(filename);
			}
		}
		#endregion
	}
}

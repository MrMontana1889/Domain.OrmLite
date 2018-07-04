# Domain.OrmLite

This component uses the latest version of ServiceStack.OrmLite.Sqlite.

The component is configured to use a Datasource along with a DataSourceConnection which creates the connection to a given filename.

The data source can create a new database or open an existing database.  A default table, InfoTable, is created by default.

See the unit test for some simpler operations that can be done.  If you want to add additional tables to the datasource by default, modify the LoadSchemaImpl method in SqliteDataConnection (follow along with how InfoTable is added).

THe DomainTable also provides a way to edit the properties thru an "IField" implementation.  This is done via reflection given minimal information about the field.  Unitized fields are also supported with minimal Units in OrmLite.Support.Units.  You can add new dimensions and units as needed there.

Resources are localized via the TextManager object in the OrmLite.Resources project.  Create a new LocalizedStrings.resx file to add your key/value pairs and then use TextManager.Current[key] to get the localized string.

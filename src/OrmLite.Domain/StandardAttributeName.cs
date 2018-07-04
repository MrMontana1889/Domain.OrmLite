// StandardAttributeName.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

namespace OrmLite.Domain
{
	public struct SchemaTableName
	{
		public const string INFOTABLE_V1 = "INFOTABLE_V1";
		public const string SETTING_V1 = "SETTING_V1";
	}

	public struct SchemaFieldName
	{
		public const string INFOID = "INFOID";
		public const string INFONAME = "INFONAME";
		public const string INFOVALUE = "INFOVALUE";
	}

	public struct StandardIndexName
	{
		public const string Index_InfoTable_InfoName = "IDX_INFOTABLE_INFONAME";
	}

	public struct StandardFieldName
	{
		public const string InfoName = "InfoName";
		public const string Value = "Value";
	}

	public struct StandardFieldNameLabelKey
	{
		public const string InfoName = "InfoNameLabel";
		public const string Value = "ValueLabel";
	}
}

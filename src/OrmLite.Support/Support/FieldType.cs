// FieldType.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Resource.Support;
using OrmLite.Support.Library;
using OrmLite.Support.Units;
using System;

namespace OrmLite.Support.Support
{
    public class FieldType : IFieldType
    {
        #region Constructor
        public FieldType(string name, Type valueType, string labelKey, string categoryKey,
            string notesKey, string formatterName, Unit storageUnit, object defaultValue)
        {
            Name = name;

            LabelKey = !string.IsNullOrEmpty(labelKey) ? labelKey : Name;
            NotesKey = notesKey;
            CategoryKey = categoryKey;

            Label = TextManager.Current[LabelKey];
            Notes = !string.IsNullOrEmpty(NotesKey) ? TextManager.Current[NotesKey] : string.Empty;
            Category = !string.IsNullOrEmpty(CategoryKey) ? TextManager.Current[CategoryKey] : string.Empty;

            FormatterName = formatterName;
            StorageUnit = storageUnit;

            DefaultValue = defaultValue;

            Type = valueType;
        }
        #endregion

        #region Public Properties
        public FieldDataType FieldDataType
        {
            get { return TypeLibrary.FieldDataTypeFromType(Type); }
        }
        public string Name
        {
            get;
            private set;
        }
        public string Label
        {
            get;
            set;
        }
        public string Category
        {
            get;
            set;
        }
        public string Notes
        {
            get;
            set;
        }
        public object DefaultValue
        {
            get;
            private set;
        }
        public Type Type
        {
            get;
            private set;
        }
        public string FormatterName
        {
            get;
            private set;
        }
        public Unit StorageUnit
        {
            get;
            private set;
        }
        #endregion

        #region Protected Properties
        protected string LabelKey
        {
            get;
            private set;
        }
        protected string CategoryKey
        {
            get;
            private set;
        }
        protected string NotesKey
        {
            get;
            private set;
        }
        #endregion
    }
}

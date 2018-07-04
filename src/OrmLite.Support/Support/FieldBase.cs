// FieldBase.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System.Collections.Generic;

namespace OrmLite.Support.Support
{
    public abstract class FieldBase : IField
    {
        #region Constructor
        public FieldBase(IFieldType fieldType)
        {
            FieldType = fieldType;
        }
        #endregion

        #region Public Methods
        public abstract object GetValue(int id);
        public abstract IDictionary<int, object> GetValues();
        public abstract IDictionary<int, object> GetValues(IEnumerable<int> ids);
        #endregion

        #region Public Properties
        public string Name
        {
            get { return FieldType.Name; }
        }
        public string Label
        {
            get { return FieldType.Label; }
            set { FieldType.Label = value; }
        }
        public string Notes
        {
            get { return FieldType.Notes; }
            set { FieldType.Notes = value; }
        }
        public string Category
        {
            get { return FieldType.Category; }
            set { FieldType.Category = value; }
        }
        public virtual FieldDataType FieldDataType
        {
            get { return FieldType.FieldDataType; }
        }
        public IFieldType FieldType
        {
            get;
            private set;
        }
        #endregion
    }
}

// ObjectEditField.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System.Reflection;

namespace OrmLite.Support.Support
{
    public class ObjectEditField<TElementType> : ObjectField<TElementType>, IEditField
    {
        #region Constructor
        public ObjectEditField(IFieldType fieldType, TElementType element)
            : base(fieldType, element)
        {

        }
        #endregion

        #region Public Methods
        public virtual void SetValue(int id, object value)
        {
            PropertyInfo.SetValue(ObjectAt(id), value, null);
        }
        public void SetValues(SetValuesOperation operation, object value)
        {
            SetValue(0, value);
        }
        #endregion

        #region Public Properties
        public object DefaultValue
        {
            get { return FieldType.DefaultValue; }
        }
        #endregion
    }
}

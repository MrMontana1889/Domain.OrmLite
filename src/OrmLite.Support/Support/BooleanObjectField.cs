// BooleanObjectField.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;

namespace OrmLite.Support.Support
{
    public class BooleanObjectField<TElementType> : ObjectEditField<TElementType>
    {
        #region Constructor
        public BooleanObjectField(IFieldType fieldType, TElementType element)
            : base(fieldType, element)
        {

        }
        #endregion

        #region Public Methods
        public override object GetValue(int index)
        {
            bool retVal = false;
            object value = base.GetValue(index);
            if (FieldType.Type == typeof(int))
            {
                if (value == DBNull.Value)
                    value = Convert.ToInt32(false);
                retVal = Convert.ToBoolean((int)value);
            }
            else if (FieldType.Type == typeof(string))
            {
                if (value == DBNull.Value)
                    value = bool.FalseString.ToUpperInvariant();
                bool.TryParse(value.ToString(), out retVal);
            }
            return retVal;
        }
        public override void SetValue(int index, object value)
        {
            object retVal = value;
            if (FieldType.Type == typeof(int))
            {
                retVal = Convert.ToInt32(value);
            }
            else if (FieldType.Type == typeof(string))
            {
                retVal = Convert.ToString(value).ToUpperInvariant();
            }
            base.SetValue(index, retVal);
        }
        #endregion

        #region Public Properties
        public override FieldDataType FieldDataType
        {
            get { return FieldDataType.Boolean; }
        }
        #endregion
    }
}

// DateTimeObjectField.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Globalization;

namespace OrmLite.Support.Support
{
    public class DateTimeObjectField<TElementType> : ObjectEditField<TElementType>
    {
        #region Constructor
        public DateTimeObjectField(IFieldType fieldType, TElementType element)
            : base(fieldType, element)
        {

        }
        #endregion

        #region Public Methods
        public override object GetValue(int id)
        {
            object retVal = PropertyInfo.GetValue(ObjectAt(id), null);
            if (retVal == null || retVal == DBNull.Value)
                retVal = DateTime.Today;
            else if (retVal is string && !string.IsNullOrEmpty((string)retVal))
                retVal = DateTime.Parse((string)retVal, CultureInfo.CurrentCulture.DateTimeFormat);
            else
                retVal = DateTime.Today;

            return (DateTime)retVal;
        }
        public override void SetValue(int id, object value)
        {
            string retVal = ((DateTime)value).ToString("yyyy-MM-dd");
            PropertyInfo.SetValue(ObjectAt(id), retVal, null);
        }
        #endregion

        #region Public Properties
        public override FieldDataType FieldDataType
        {
            get { return FieldDataType.DateTime; }
        }
        #endregion
    }
}

// UnitizedObjectEditFieldBase.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Units;
using System;

namespace OrmLite.Support.Support
{
    public class UnitizedObjectEditFieldBase<TElementType, TValueType> : UnitizedObjectFieldBase<TElementType>, IEditField
    {
        #region Constructor
        public UnitizedObjectEditFieldBase(IFieldType fieldType, TElementType element)
            : base(fieldType, element)
        {

        }
        #endregion

        #region Public Methods
        public virtual void SetValue(int id, object value)
        {
            var retVal = value;
            if (WorkingUnitIndex != StorageUnitIndex)
            {
                Unit workingUnit = UnitConversionManager.Current.UnitAt(WorkingUnitIndex);
                Unit storageUnit = UnitConversionManager.Current.UnitAt(StorageUnitIndex);
                retVal = storageUnit.ConvertFrom(Convert.ToDouble(retVal), workingUnit);
            }

            if (typeof(TValueType) == typeof(decimal))
                retVal = Convert.ToDecimal(retVal);

            PropertyInfo.SetValue(ObjectAt(id), retVal, null);
        }
        public virtual void SetValues(SetValuesOperation operation, object value)
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

// UnitizedObjectFieldBase.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Units;
using System;

namespace OrmLite.Support.Support
{
    public abstract class UnitizedObjectFieldBase<TElementType> : ObjectField<TElementType>, IUnitizedField
    {
        #region Constructor
        public UnitizedObjectFieldBase(IFieldType fieldType, TElementType element)
            : base(fieldType, element)
        {
            WorkingUnitIndex = StorageUnitIndex;
        }
        #endregion

        #region Public Methods
        public override object GetValue(int id)
        {
            var value = PropertyInfo.GetValue(ObjectAt(0), null);
            if (value == null) return null;
            if (WorkingUnitIndex == StorageUnitIndex) return Convert.ToDouble(value);

            Unit workingUnit = UnitConversionManager.Current.UnitAt(WorkingUnitIndex);
            Unit storageUnit = UnitConversionManager.Current.UnitAt(StorageUnitIndex);

            return workingUnit.ConvertFrom(Convert.ToDouble(value), storageUnit);
        }
        public double GetDoubleValue(int id)
        {
            return (double)GetValue(id);
        }
        #endregion

        #region Public Properties
        public UnitConversionManager.UnitIndex StorageUnitIndex
        {
            get { return UnitConversionManager.Current.UnitIndexFor(FieldType.StorageUnit); }
        }
        public UnitConversionManager.UnitIndex WorkingUnitIndex
        {
            get;
            set;
        }
        #endregion
    }
}

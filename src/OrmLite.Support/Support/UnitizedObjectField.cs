// UnitizedObjectField.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

namespace OrmLite.Support.Support
{
    public class UnitizedObjectField<TElementType, TValueType> : UnitizedObjectEditFieldBase<TElementType, TValueType>
    {
        #region Constructor
        public UnitizedObjectField(IFieldType fieldType, TElementType element)
            : base(fieldType, element)
        {

        }
        #endregion
    }
}

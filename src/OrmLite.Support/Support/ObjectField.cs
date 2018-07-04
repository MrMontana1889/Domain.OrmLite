// ObjectField.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

namespace OrmLite.Support.Support
{
    public class ObjectField<TElementType> : ObjectFieldBase<TElementType>
    {
        #region Constructor
        public ObjectField(IFieldType fieldType, TElementType element)
            : base(fieldType, element.GetType())
        {
            Element = element;
        }
        #endregion

        #region Protected Methods
        protected override TElementType ObjectAt(int index)
        {
            return Element;
        }
        #endregion

        #region Protected Properties
        protected override int Count
        {
            get { return 1; }
        }
        #endregion

        #region Private Properties
        protected TElementType Element { get; private set; }
        #endregion
    }
}

// ObjectFieldBase.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using OrmLite.Support.Library;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OrmLite.Support.Support
{
    public abstract class ObjectFieldBase<TElementType> : FieldBase
    {
        #region Constructor
        public ObjectFieldBase(IFieldType fieldType, Type elementType)
            : base(fieldType)
        {
            PropertyInfo = elementType.GetProperty(Name);
            if (PropertyInfo == null)
            {
                Type[] interfaces = elementType.GetInterfaces();
                foreach (Type i in interfaces)
                {
                    PropertyInfo = i.GetProperty(Name);
                    if (PropertyInfo != null)
                        break;
                }
            }
        }
        #endregion

        #region Public Methods
        public override object GetValue(int id)
        {
            return PropertyInfo.GetValue(ObjectAt(id), null);
        }
        public override IDictionary<int, object> GetValues()
        {
            IDictionary<int, object> retVal = DictionaryLibrary.NewDictionary<int, object>(Count);
            for (int i = 0; i < Count; ++i)
                retVal[i] = GetValue(i);
            return retVal;
        }
        public override IDictionary<int, object> GetValues(IEnumerable<int> ids)
        {
            IDictionary<int, object> retVal = DictionaryLibrary.NewDictionary<int, object>(100);
            foreach (int id in ids)
                retVal[id] = GetValue(id);
            return retVal;
        }
        #endregion

        #region Protected Methods
        protected abstract TElementType ObjectAt(int index);
        #endregion

        #region Protected Properties
        protected PropertyInfo PropertyInfo
        {
            get;
            private set;
        }
        protected abstract int Count { get; }
        #endregion
    }
}

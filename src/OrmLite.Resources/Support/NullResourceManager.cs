// NullResourceManager.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Resources;

namespace OrmLite.Resource.Support
{
	public class NullResourceManager : ResourceManager
    {
        #region Constructors

        public NullResourceManager()
        {
        }

        #endregion

        #region Public Methods

        public override String GetString(string labelkey)
        {
            return null;
        }

        public override Object GetObject(string resourcekey)
        {
            return null;
        }

        #endregion
    }
}

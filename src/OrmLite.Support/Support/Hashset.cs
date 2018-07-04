// Hashset.cs
// Copyright (c) 2018 Culin Enterprises, Incorporated. All Rights Reserved.

using System;
using System.Collections;
using System.Diagnostics;

namespace OrmLite.Support.Support
{
    /// <summary>
    /// Implementation of set interface using CLR Hashtable.
    /// Wastes space: temporary hack?
    /// </summary>
    [DebuggerDisplay("Count = {Count}")]
    [DebuggerTypeProxy(typeof(HashsetProxy))]
    public class Hashset : ISet
    {
        #region Constructors
        public Hashset()
        {
        }

        public Hashset(int count)
        {
            m_hashtable = new Hashtable(count);
        }
        #endregion

        public bool Add(object aobject)
        {
            Debug.Assert(aobject != null);
            if (!Contains(aobject))
            {
                m_hashtable.Add(aobject, null);
                return true;
            }
            else
                return false;
        }

        public bool AddAll(ICollection aicollection)
        {
            bool abool = false;
            foreach (object aobject in aicollection)
                abool = Add(aobject) || abool;
            return abool;
        }

        public void Clear()
        {
            m_hashtable.Clear();
        }

        public bool Contains(object aobject)
        {
            return m_hashtable.Contains(aobject);
        }

        public void CopyTo(Array aarray, int aint)
        {
            //TODO 3: CopyTo
            throw new InvalidOperationException();
        }

        public int Count
        {
            get { return m_hashtable.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            return new HashsetEnumerator(m_hashtable.GetEnumerator());
        }

        public bool IsSynchronized
        {
            get
            {
                //TODO 3: IsSynchronized
                throw new InvalidOperationException();
            }
        }

        public bool Remove(object aobject)
        {
            if (Contains(aobject))
            {
                m_hashtable.Remove(aobject);
                return true;
            }
            else
                return false;
        }

        public bool RemoveAll(ICollection aicollection)
        {
            bool abool = false;
            foreach (object aobject in aicollection)
                abool = Remove(aobject) || abool;
            return abool;
        }

        public object SyncRoot
        {
            get
            {
                //TODO 3: SyncRoot
                throw new InvalidOperationException();
            }
        }

        private Hashtable m_hashtable = new Hashtable();

        ////////////////////////////////////////////////////////////////////////

        private class HashsetEnumerator : IEnumerator
        {

            public HashsetEnumerator(IEnumerator aie)
            {
                m_ienumerator = aie;
            }

            public object Current
            {
                get { return ((DictionaryEntry)m_ienumerator.Current).Key; }
            }

            public bool MoveNext()
            {
                return m_ienumerator.MoveNext();
            }

            public void Reset()
            {
                m_ienumerator.Reset();
            }

            private IEnumerator m_ienumerator;
        }

        #region Debugger Type Proxy
        internal class HashsetProxy
        {
            private readonly Hashset m_source;
            public HashsetProxy(Hashset source)
            {
                m_source = source;
            }
            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public object[] Data
            {
                get
                {
                    object[] data = new object[m_source.Count];
                    int i = 0;
                    foreach (object datum in m_source)
                    {
                        data[i] = datum;
                        i++;
                    }
                    return data;
                }
            }
        }
        #endregion
    }
}

using LogicReinc.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Collection
{
    public class TSDualDictionary<K1, K2, T>
    {
        private RWLock locker = new RWLock();
        public bool SoftAccess { get; set; }
        private Dictionary<Key, T> dict = new Dictionary<Key, T>();

        public TSDualDictionary(bool soft = false)
        {
            SoftAccess = soft;
        }

        public bool ContainsKey(K1 a, K2 b)
        {
            Key k = new Key(a, b);
            return locker.ReadLock(()=>dict.ContainsKey(k));
        }


        public T this[K1 a, K2 b]
        {
            get
            {
                Key k = new Key(a, b);
                return locker.ReadLock(() => {
                    if (!dict.ContainsKey(k))
                    {
                        if (SoftAccess)
                            return default(T);
                        else
                            throw new IndexOutOfRangeException("Key not found");
                    }
                    return dict[k];
                });
            }
            set
            {
                locker.WriteLock(() =>
                {
                    Key k = new Key(a, b);
                    if (!dict.ContainsKey(k))
                    {

                        if (SoftAccess)
                        {
                            dict.Add(k, default(T));
                        }
                        else
                            throw new IndexOutOfRangeException("Key not found");
                    }
                    dict[k] = value;
                });
            }
        }


        private struct Key : IEquatable<Key>
        {
            public K1 Key1 { get; private set; }
            public K2 Key2 { get; private set; }

            public Key(K1 k1, K2 k2)
            {
                Key1 = k1;
                Key2 = k2;
            }

            public bool Equals(Key obj)
            {
                return Key1.Equals(obj.Key1) && Key2.Equals(obj.Key2);
            }
        }
    }


    public class DualDictionary<K1, K2, T>
    {
        public bool SoftAccess { get; set; }
        private Dictionary<Key, T> dict = new Dictionary<Key, T>();

        public DualDictionary(bool soft = false)
        {
            SoftAccess = soft;
        }

        public bool ContainsKey(K1 a, K2 b)
        {
            Key k = new Key(a, b);
            return dict.ContainsKey(k);
        }
        

        public T this[K1 a, K2 b]
        {
            get
            {
                Key k = new Key(a, b);
                if (!dict.ContainsKey(k))
                {
                    if (SoftAccess)
                        return default(T);
                    else
                        throw new IndexOutOfRangeException("Key not found");
                }
                return dict[k];
            }
            set
            {
                Key k = new Key(a, b);
                if (!dict.ContainsKey(k))
                {
                    if (SoftAccess)
                        dict.Add(k, default(T));
                    else
                        throw new IndexOutOfRangeException("Key not found");
                }
                dict[k] = value;
            }
        }


        private struct Key : IEquatable<Key>
        {
            public K1 Key1 { get; private set; }
            public K2 Key2 { get; private set; }

            public Key(K1 k1, K2 k2)
            {
                Key1 = k1;
                Key2 = k2;
            }

            public bool Equals(Key obj)
            {
                return Key1.Equals(obj.Key1) && Key2.Equals(obj.Key2);
            }
        }
    }
}

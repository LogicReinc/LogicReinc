using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogicReinc.Collection
{
    public class ObjectIndex<T>
    {
        private ReaderWriterLockSlim locked = new ReaderWriterLockSlim();
        private Dictionary<string, Dictionary<object, List<T>>> indexes = new Dictionary<string, Dictionary<object, List<T>>>();

        public bool AlwaysCopyResults { get; set; } = false;

        public List<string> Properties { get { lock (indexes) { return indexes.Keys.ToList(); } } }

        public ObjectIndex(bool copyResults = false)
        {
            AlwaysCopyResults = copyResults;
        }

        public void RemoveIndex(string property, object value, T obj)
        {
            locked.EnterReadLock();
            try
            {
                if (indexes.ContainsKey(property))
                {
                    Dictionary<object, List<T>> pIndex = indexes[property];

                    if (value != null && pIndex.ContainsKey(value))
                        pIndex[value].Remove(obj);
                }
            }
            finally
            {
                locked.ExitReadLock();
            }
        }

        public void AddIndex(string property, object value, T obj)
        {
            locked.EnterWriteLock();
            try
            {
                if (!indexes.ContainsKey(property))
                    indexes.Add(property, new Dictionary<object, List<T>>());
                Dictionary<object, List<T>> pIndex = indexes[property];

                if (value != null)
                {
                    if (!pIndex.ContainsKey(value))
                        pIndex.Add(value, new List<T>());
                    pIndex[value].Add(obj);
                }
            }
            finally
            {
                locked.ExitWriteLock();
            }
        }

        public bool HasTypeProperty(string property)
        {
            locked.EnterReadLock();
            try
            {
                if (!indexes.ContainsKey(property))
                    return false;
                return true;
            }
            finally
            {
                locked.ExitReadLock();
            }
        }

        public int GetIndexLength(string property, object value)
        {
            locked.EnterReadLock();
            try
            {
                if (!indexes.ContainsKey(property))
                    return 0;
                if (!indexes[property].ContainsKey(value))
                    return 0;
                return indexes[property][value].Count;
            }
            finally
            {
                locked.ExitReadLock();
            }
        }

        public List<T> GetIndex(string property, object value)
        {
            locked.EnterReadLock();
            try
            { 
                if (!indexes.ContainsKey(property))
                    return new List<T>();
                if (!indexes[property].ContainsKey(value))
                    return new List<T>();
                List<T> result = indexes[property][value];
                return (AlwaysCopyResults) ? result.ToList() : result;
            }
            finally
            {
                locked.ExitReadLock();
            }
        }
        public List<C> GetIndex<C>(string property, object value)
        {
            locked.EnterReadLock();
            try
            {
                if (!indexes.ContainsKey(property))
                    return new List<C>();
                if (!indexes[property].ContainsKey(value))
                    return new List<C>();
                List<T> result = indexes[property][value];
                return result.Cast<C>().ToList();
            }
            finally
            {
                locked.ExitReadLock();
            }
        }
    }
}

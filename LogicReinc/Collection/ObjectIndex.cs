using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Collection
{
    public class ObjectIndex<T>
    {
        public Dictionary<string, Dictionary<object, List<T>>> indexes = new Dictionary<string, Dictionary<object, List<T>>>();


        public void RemoveIndex(string property, object value, T obj)
        {
            lock(indexes)
            if(indexes.ContainsKey(property))
            {
                Dictionary<object, List<T>> pIndex = indexes[property];

                if (pIndex.ContainsKey(value))
                    pIndex[value].Remove(obj);
            }
        }

        public void AddIndex(string property, object value, T obj)
        {
            lock (indexes)
            {
                if (!indexes.ContainsKey(property))
                    indexes.Add(property, new Dictionary<object, List<T>>());
                Dictionary<object, List<T>> pIndex = indexes[property];

                if (!pIndex.ContainsKey(value))
                    pIndex.Add(value, new List<T>());
                pIndex[value].Add(obj);
            }
        }

        public bool HasTypeProperty(string property)
        {
            if (!indexes.ContainsKey(property))
                return false;
            return true;
        }

        public List<T> GetIndex(string property, object value)
        {
            lock(indexes)
            {
                if (!indexes.ContainsKey(property))
                    return new List<T>();
                if (!indexes[property].ContainsKey(value))
                    return new List<T>();
                return indexes[property][value];
            }
        }
    }
}

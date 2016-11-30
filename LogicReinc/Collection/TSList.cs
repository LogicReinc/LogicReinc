﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Collections
{
    public class TSList<T> : IEnumerable<T>
    {
        private List<T> list = new List<T>();

        public TSList()
        {

        }

        public TSList(List<T> list)
        {
            this.list = list;
        }

        //Properties
        public int Length
        {
            get
            {
                return list.Count;
            }
        }

        //Selectors
        public void Lock(Action<List<T>> action)
        {
            lock(list)
            {
                action(list);
            }
        }

        public List<R> Select<R>(Func<T, R> selector)
        {
            List<R> nList = new List<R>();
            lock (list)
            {
                foreach (T item in list)
                    nList.Add(selector(item));
            }
            return nList;
        }

        public List<T> Where(Func<T, bool> selector)
        {
            List<T> nList = new List<T>();
            lock (list)
            {
                foreach (T item in list)
                {
                    if (selector(item))
                        nList.Add(item);
                }
            }
            return nList;
        }

        public T Single(Func<T, bool> condition)
        {
            int count = Count(condition);
            if (count > 1)
                throw new Exception("Multiple entries found matching condition.");
            else if (count == 0)
                throw new Exception("No entry found matching condition.");
            else
            {
                lock (list)
                {
                    foreach (T item in list)
                    {
                        if (condition(item))
                            return item;
                    }
                }
            }

            throw new Exception("No entry found matching condition.");
        }

        public T FirstOrDefault(Func<T, bool> condition)
        {
            lock (list)
            {
                foreach (T item in list)
                {
                    if (condition(item))
                        return item;
                }
            }
            return default(T);
        }
        public T SingleOrDefault(Func<T, bool> condition)
        {
            int count = Count(condition);
            if (count > 1)
                throw new Exception("Multiple entries found matching condition.");
            else if (count == 0)
                return default(T);
            else
            {
                lock (list)
                {
                    foreach (T item in list)
                    {
                        if (condition(item))
                            return item;
                    }
                }
            }

            return default(T);
        }

        public T[] ToArray()
        {
            lock (list)
            {
                return list.ToArray();
            }
        }

        public List<T> ToList()
        {
            lock(list)
            {
                return list.ToList();
            }
        }

        public void ForEach(Action<T> feachAction)
        {
            lock (list)
            {
                foreach (T item in list)
                    feachAction(item);
            }
        }

        public int Count(Func<T, bool> selector)
        {
            int i = 0;
            lock (list)
            {
                foreach (T item in list)
                {
                    if (selector(item))
                        i++;
                }
            }
            return i;
        }

        public bool Contains(Func<T, bool> selector)
        {
            lock (list)
            {
                foreach (T item in list)
                {
                    if (selector(item))
                        return true;
                }
            }
            return false;
        }

        //Modifiers
        public void Add(T item)
        {
            lock (list)
            {
                list.Add(item);
            }
        }
        public void Add(List<T> items)
        {
            lock(list)
            {
                list.AddRange(items);
            }
        }

        public void Remove(T item)
        {
            lock (list)
            {
                list.Remove(item);
            }
        }
        public void Remove(Func<T, bool> selector)
        {
            lock (list)
            {
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    if (selector(list[i]))
                        list.RemoveAt(i);
                }
            }
        }

        public void Set(List<T> list)
        {
            lock (this.list)
            {
                this.list = list;
            }
        }

        public List<T> Query(Func<List<T>, List<T>> query)
        {
            lock(this.list)
            {
                return query(this.list);
            }
        }


        //Interface
        public IEnumerator<T> GetEnumerator()
        {
            return ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ToList().GetEnumerator();
        }
    }
}
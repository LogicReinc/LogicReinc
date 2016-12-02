using System;

namespace LogicReinc.Threading
{
    public class WorkTask<T>
    {
        public Action<T> Callback { get; set; }
        public T Data { get; set; }
        public Action<T> Work { get; set; }
    }
}
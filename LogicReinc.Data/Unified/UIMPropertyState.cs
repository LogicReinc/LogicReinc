using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.Unified
{
    public class UIMPropertyState
    {
        public UIMPropertyState(object val)
        {
            LastState = val;
        }
        private object _lock = new object(); 
        public object LastState { get; set; }

        public bool HasChanged(object val)
        {
            if (LastState != val)
                return true;
            return false;
        }

        public bool HasChangedAndUpdate(object val, Action<object, object> updateLock = null)
        {
            lock (_lock)
            {
                if (LastState != val)
                {
                    if(updateLock != null)
                    updateLock(LastState, val);
                    LastState = val;
                    return true;
                }
                return false;
            }
        }
    }
}

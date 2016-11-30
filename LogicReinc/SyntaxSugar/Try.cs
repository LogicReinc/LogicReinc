using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.SyntaxSugar
{
    public class Try
    {
        public static T Func<T>(Func<T> _try, Func<Exception, T> _catch = null)
        {
            try
            {
                return _try();
            }
            catch(Exception ex)
            {
                if (_catch != null) 
                    return _catch(ex);
                return default(T);
            }
        }

        public static void Action(Action _try, Action<Exception> _catch = null)
        {
            try
            {
                _try();
            }
            catch(Exception ex)
            {
                if (_catch != null)
                    _catch(ex);
            }
        }
    }
}

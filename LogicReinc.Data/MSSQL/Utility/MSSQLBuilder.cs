using LogicReinc.Data.SQL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.MSSQL.Utility
{
    public class MSSQLBuilder : QueryBuilder
    {
        private static MSSQLBuilder _static;
        public static MSSQLBuilder Static
        {
            get
            {
                if (_static == null)
                    _static = new MSSQLBuilder();
                return _static;
            }
        }
        


    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Data.SQL
{
    public class SQLTable
    {
        public string Name { get; set; }
        public List<ColumnProperty> Columns { get; set; } = new List<ColumnProperty>();
    }
}

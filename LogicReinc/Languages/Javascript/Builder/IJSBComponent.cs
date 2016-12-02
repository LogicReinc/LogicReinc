using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Languages.Javascript.Builder
{
    public interface IJSBComponent
    {
        string BuildCode(int indented = 0);
    }
}

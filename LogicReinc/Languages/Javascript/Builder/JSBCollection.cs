using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogicReinc.Languages.Javascript.Builder
{
    public class JSBCollection : IJSBComponent
    {
        public List<object> Collection { get; private set; }

        public JSBCollection()
        {
            Collection = new List<object>();
        }



        public string BuildCode(int indented = 0)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("[");

            int i = 0;
            foreach(object obj in Collection)
            {
                builder.Append(JSBObject.BuildValue(obj, indented));

                if (i < Collection.Count - 1)
                    builder.Append(", ");

                i++;
            }

            builder.Append("]");

            return builder.ToString();
        }






        public static JSBCollection FromCollection(IEnumerable collection)
        {
            JSBCollection col = new JSBCollection();
            foreach(object obj in collection)
                col.Collection.Add(JSBObject.FromAnonymous(obj));
            return col;
        }
    }
}

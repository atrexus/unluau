using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unluau.Utils
{
    public static class TypeExtentions
    {
        public static string ToString<T>(T? value)
        {
            if (typeof(T).IsArray && (value is Array array))
            {
                StringBuilder stringBuilder = new();

                stringBuilder.Append('[');

                for (int i = 0; i < array.Length; ++i)
                {
                    var item = array.GetValue(i);

                    if (i > 0)
                        stringBuilder.Append(", ");

                    stringBuilder.Append(ToString(item));
                }

                stringBuilder.Append(']');

                return stringBuilder.ToString();
            }
            if (value is null) 
                return "null";

            return value.ToString()!;
        }
    }
}

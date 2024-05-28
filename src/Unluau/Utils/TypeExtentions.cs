using System.Collections;
using System.Text;

namespace Unluau.Decompile.Utils
{
    public static class TypeExtensions
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.TypeExtensions
{
    public static class GenericTypeExtensions
    {
        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }

        public static string GetGenericTypeName(this Type type)
        {
            var typeName = string.Empty;

            if(type.IsGenericType)
            {
                var genericArgumentStr = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf("`"))}<{genericArgumentStr}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }
    }
}

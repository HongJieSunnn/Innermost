using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.Extensions
{
    public static class IEnumerableComparisonExtensions
    {
        public static bool EqualList<T>(this IEnumerable<T> list1,IEnumerable<T> list2) where T:Enumeration
        {
            var setOfList2 = list2.ToHashSet();
            foreach(T ele in list1)
            {
                if(!setOfList2.Contains(ele))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

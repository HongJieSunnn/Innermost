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
            foreach(T ele in list1)
            {
                if(!list2.Contains(ele))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

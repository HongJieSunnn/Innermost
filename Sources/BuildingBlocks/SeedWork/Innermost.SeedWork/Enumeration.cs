using System;
using System.Collections.Generic;
using System.Text;

namespace Innermost.SeedWork
{
    /// <summary>
    /// 枚举，可以派生各种状态。
    /// </summary>
    public abstract class Enumeration : IComparable
    {
        public int Id { get; private init; }
        public int CompareTo(object obj)
        {
            return Id.CompareTo(((Enumeration)obj).Id);
        }
    }
}

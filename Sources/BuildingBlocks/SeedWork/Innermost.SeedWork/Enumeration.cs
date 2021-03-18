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
        public string Name { get; private init; }
        public Enumeration(int id,string name)
        {
            Id = id;
            Name = name;
        }
        public override bool Equals(object obj)
        {
            var objEnumeration = obj as Enumeration;
            if (objEnumeration == null)
                return false;
            return this.GetType().Equals(obj.GetType()) && this.Id.Equals(objEnumeration.Id);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
        public int CompareTo(object obj)
        {
            return Id.CompareTo(((Enumeration)obj).Id);
        }
    }
}

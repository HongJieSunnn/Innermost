using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class TextType
        : Enumeration
    {
        static public TextType Essay = new TextType(1, "ESSAY");
        static public TextType Article = new TextType(2, "ARTICLE");
        public TextType(int id, string name) : base(id, name)
        {
        }
    }
}

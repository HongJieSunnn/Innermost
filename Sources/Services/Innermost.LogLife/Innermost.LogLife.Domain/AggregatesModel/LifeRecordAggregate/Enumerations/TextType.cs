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
        public static TextType Essay = new TextType(1, "ESSAY");
        public static TextType Article = new TextType(2, "ARTICLE");

        public static IEnumerable<TextType> TextTypeEnumerable 
        {
            get { yield return Essay;yield return Article; }
        }
        public TextType(int id, string name) : base(id, name)
        {
        }

        

        public static TextType GetFromName(string name)
        {
            var type= TextTypeEnumerable.FirstOrDefault(t => t.Name == name);
            if (type == null)
                throw new ArgumentException($"can not find TextType by name:{name}");
            return type;
        }

        public static TextType GetFromId(int id)
        {
            var type= TextTypeEnumerable.FirstOrDefault(t => t.Id == id);
            if (type == null)
                throw new ArgumentException($"can not find TextType by id:{id}");
            return type;
        }
    }
}

using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class Location
        : ValueObject
    {
        public string Province { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Town { get; set; }

        public Location()
        {

        }

        public Location(string province,string city,string county,string town)
        {
            Province = province;
            City = city;
            County = county;
            Town = town;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Province;
            yield return City;
            yield return County;
            yield return Town;
        }
    }
}

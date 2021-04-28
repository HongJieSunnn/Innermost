using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class Location
        : Entity
    {
        public string Province { get;private set; }
        public string City { get; private set; }
        public string County { get; private set; }
        public string Town { get; private set; }
        public string Place { get; private set; }

        public Location()
        {

        }
        
        public Location(Location location)
        {
            Id = location.Id;
            Province = location.Province;
            City = location.City;
            County = location.County;
            Town = location.Town;
            Place = location.Place;
        }
        /// <summary>
        /// Location 的 Id 从前端给，若非自定义，必定存在Id并且会带在Request中，若自定义必定不存在该Id，然后添加到 Location 表中。并且由于添加了候选键，所以一个 Location 一定会是唯一的，否则会报错。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="province"></param>
        /// <param name="city"></param>
        /// <param name="county"></param>
        /// <param name="town"></param>
        /// <param name="place"></param>
        public Location(int id,string province,string city,string county,string town,string place)
        {
            Id = id;
            Province = province;
            City = city;
            County = county;
            Town = town;
            Place = place;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Models
{
    public class DateTimeToFind
    {
        public const string FindByYear = "FindByYear";
        public const string FindByMonth = "FindByMonth";
        public const string FindByDay = "FindByDay";
        public const string FindByRangeDay = "FindByRangeDay";//TODO To be used
        /// <summary>
        /// 通过查询的类型（查整年、某个月、某天、某几天(TODO)）来调用对应的函数。
        /// </summary>
        private readonly Dictionary<string, Func<(DateTime, DateTime)>> _getTimePairDitionary = new Dictionary<string, Func<(DateTime, DateTime)>>();

        [RegularExpression(@"(2)\d{3}")]
        public string Year { get; init; }
        [RegularExpression(@"(1[0-2]|0[1-9])")]
        public string Month { get; init; }
        [RegularExpression(@"(0[1-9]|[1-2][0-9]|3[0-1])")]
        public string Day { get; init; }
        [Required]
        public string FindType { get; init; }
        public static IEnumerable<string> AllowedFindTypes => new List<string>() { FindByYear, FindByMonth, FindByDay };
        public DateTimeToFind(string year,string month,string day,string findType)
        {
            Year = year;
            Month = month;
            Day = day;
            FindType = AllowedFindTypes.Contains(findType)?findType:throw new ArgumentException($"find-type {findType} is not allowed");

            _getTimePairDitionary.Add(FindByYear, GetStartAndEndTimePairYearType);
            _getTimePairDitionary.Add(FindByMonth, GetStartAndEndTimePairMonthType);
            _getTimePairDitionary.Add(FindByDay, GetStartAndEndTimePairDayType);
        }
        /// <summary>
        /// 通过查询的类型来决定起始时间以及结束时间来模糊查询
        /// </summary>
        /// <returns></returns>
        public (DateTime,DateTime) GetStartAndEndTimePair()
        {
            return _getTimePairDitionary[FindType].Invoke();
        }

        private (DateTime, DateTime) GetStartAndEndTimePairYearType()
        {
            DateTime startTime = new DateTime(year: int.Parse(Year), 01, 01);
            DateTime endTime = new DateTime(year: int.Parse(Year), 12, 31,23,59,59);

            return (startTime, endTime);
        }

        private (DateTime, DateTime) GetStartAndEndTimePairMonthType()
        {
            DateTime startTime = new DateTime(year: int.Parse(Year), int.Parse(Month), 01);
            DateTime endTime = new DateTime(year: int.Parse(Year), int.Parse(Month)+1, 1, 23, 59, 59);
            endTime.AddDays(-1);

            return (startTime, endTime);
        }

        private (DateTime, DateTime) GetStartAndEndTimePairDayType()
        {
            DateTime startTime = new DateTime(year: int.Parse(Year), int.Parse(Month), int.Parse(Day));
            DateTime endTime = new DateTime(year: int.Parse(Year), int.Parse(Month), int.Parse(Day), 23, 59, 59);

            return (startTime, endTime);
        }
    }
}

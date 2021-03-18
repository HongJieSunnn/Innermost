using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class MusicRecord
        : ValueObject
    {
        public string MusicName { get; private set; }
        public string Singer { get; private set; }
        public string Album { get;private set; }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return MusicName;
            yield return Singer;
            yield return Album;
        }
    }
}

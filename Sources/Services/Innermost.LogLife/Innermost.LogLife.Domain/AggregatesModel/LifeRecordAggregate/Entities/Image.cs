using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class Image
        :Entity
    {
        public Guid Guid { get; set; }
        public string OriginName { get; private set; }
        public string Path { get; private set; }
        public PictureFormat PictureFormat { get; private set; }

        public Image(string name,string path)
        {
            
        }
    }
}

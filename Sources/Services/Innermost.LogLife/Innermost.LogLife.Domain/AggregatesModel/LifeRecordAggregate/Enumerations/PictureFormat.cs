using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class PictureFormat
        : Enumeration
    {
        private static PictureFormat JPEG = new PictureFormat(1, "JPEG");
        private static PictureFormat JPG = new PictureFormat(2, "JPG");
        private static PictureFormat GIF = new PictureFormat(3, "GIF");
        private static PictureFormat PNG = new PictureFormat(4, "PNG");
        private static PictureFormat BMP = new PictureFormat(5, "BMP");
        private static PictureFormat TIFF = new PictureFormat(6, "TIFF");

        public PictureFormat(int id, string name) : base(id, name)
        {
        }
    }
}

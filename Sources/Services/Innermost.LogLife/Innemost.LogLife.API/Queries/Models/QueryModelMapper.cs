using Innemost.LogLife.API.Queries.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Queries.Models
{
    public class QueryModelMapper
    {
        public static LifeRecord MapLifeRecordQueryModel([NotNull]dynamic record)
        {
            //TODO 可以继续修改，将新建各个 Model 分开来，但未必需要public 类似于 MapTextTypeQueryModel
            LifeRecord lifeRecord = new LifeRecord
            {
                Id = record.Id,
                Title = record.Title,
                Text = record.Text,
                TextType = MapTextTypeQueryModel(record),
                Location=MapLocationQueryModel(record),
                PublishTime = record.PublishTime,
                MusicRecord=MapMusicRecordQueryModel(record),
                Path=record.Path,
                IsShared=record.IsShared
            };
            return lifeRecord;
        }

        public static IEnumerable<LifeRecord> MapLifeRecordQueryModel([NotNull]IEnumerable<dynamic> records)
        {
            List<LifeRecord> ans = new List<LifeRecord>();
            var recordEnumerator = records.GetEnumerator();
            while (recordEnumerator.MoveNext())
            {
                var record = MapLifeRecordQueryModel(recordEnumerator.Current);
                ans.Add(record);
            }
            return ans;
        }

        static TextType MapTextTypeQueryModel([NotNull]dynamic record)
        {
            TextType textType = new TextType
            {
                TextTypeName = record.TextTypeName
            };

            return textType;
        }

        static Location MapLocationQueryModel([NotNull] dynamic record)
        {
            Location location = new Location
            {
                Province = record.Province,
                City = record.City,
                County = record.County,
                Town = record.Town,
                Place = record.Place
            };

            return location;
        }

        static MusicRecord MapMusicRecordQueryModel(dynamic record)
        {
            MusicRecord musicRecord = new MusicRecord
            {
                MusicName = record.MusicName,
                Singer = record.Singer,
                Album = record.Album
            };

            return musicRecord;
        }
    }
}

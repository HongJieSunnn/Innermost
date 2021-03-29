using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using MySqlConnector;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Innemost.LogLife.API.Models;

namespace Innemost.LogLife.API.Queries
{
    public class LifeRecordQueries
        : ILifeRecordQueries
    {
        private readonly string _connectionString = string.Empty;
        public LifeRecordQueries(string connectionString)
        {
            _connectionString = string.IsNullOrWhiteSpace(connectionString) ? connectionString : throw new ArgumentNullException(nameof(connectionString));
        }
        public Task<IEnumerable<LifeRecord>> FindRecordsByEmotionTags(string userId, IEnumerable<int> emotionTagIds)
        {
            //TODO 将标签系统放到Redis中
            //using (var conn=new MySqlConnection(_connectionString))
            //{
            //    conn.Open();

            //    var result = await conn.QueryAsync<LifeRecord>(
            //        @"SELECT l.id as recordid,l.Title as title,l.Text as text,l.TextTypeId as texttype,
            //            l.Province as province,l.City as city,l.County as county,l.Town as town,l.Place as place,
            //            l.PublishTime as publishtime,l.MusicRecordId as musicrecordid,l.Path as path,
            //            e.Name as emotiontagname,e.Emoji as emotiontagemoji,
            //            tt.Name as texttypename
            //            FROM LifeRecord l
            //            INNER JOIN EmotionTag e On recordid=e.LifeRecordId
            //            INNER JOIN TextType tt On "
            //        );
            //}
            throw new NotImplementedException();
        }

        public Task<IEnumerable<LifeRecord>> FindRecordsByKeywordAsync(string userId, string keyword)
        {
            //TODO 可能通过一些搜索引擎来完成
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LifeRecord>> FindRecordsByPathAsync(string userId, string path)
        {
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var records= await connection.QueryAsync<LifeRecord>(
                    @"SELECT l.id,l.Title,l.Text,l.TextTypeId,
                        l.Location_Province,l.Location_City,l.Location_County,l.Location_Town,l.Location_Place,
                        l.PublishTime,l.MusicRecordId
                        FROM LifeRecord l
                        WHERE l.Path=@path AND l.UserId=@userId"
                    , new {path=path,userId=userId}
                    );
                //TODO 去Redis找EmotionTags、去MongoDB找图片
                return records;
            }
        }

        public async Task<IEnumerable<LifeRecord>> FindRecordsByPublishTime(string userId, DateTimeToFind dateTime)
        {
            var timePair = dateTime.GetStartAndEndTimePair();
            var startTime = timePair.Item1.ToString("yyyy-MM-dd hh:mm:ss");
            var endTime = timePair.Item2.ToString("yyyy-MM-dd hh:mm:ss");
            using (var connection = new MySqlConnection(_connectionString))
            {
                connection.Open();

                var records= await connection.QueryAsync<LifeRecord>(
                    @"SELECT l.id,l.Title,l.Text,l.TextTypeId,
                        l.Province,l.City,l.County,l.Town,l.Place,
                        l.PublishTime,l.MusicRecordId
                        FROM LifeRecord l
                        WHERE l.DateTime>=@startTime AND l.DateTime<=@endTime
                        AND l.UserId=@userId"
                    , new { startTime = startTime,endTime=endTime, userId = userId }
                    );
                //TODO 去Redis找EmotionTags、去MongoDB找图片
                return records;
            }
        }

        public async Task<IEnumerable<IGrouping<string,LifeRecord>>> FindRecordsGroupByPathAsync(string userId)
        {
            using (var connetion=new MySqlConnection(_connectionString))
            {
                connetion.Open();

                var recordsBeforeGrouping = await connetion.QueryAsync<LifeRecord>(
                    @"SELECT l.id,l.Title,l.Text,l.TextTypeId,
                        l.Province,l.City,l.County,l.Town,l.Place,
                        l.PublishTime,l.MusicRecordId
                        FROM LifeRecord l
                        WHERE l.UserId=@userId"
                    , new {userId = userId }
                );
                //TODO 去Redis找EmotionTags、去MongoDB找图片

                var records = recordsBeforeGrouping.GroupBy(l => l.Path);

                return records;
            }
        }
    }
}

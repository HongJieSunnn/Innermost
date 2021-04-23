
using MySqlConnector;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Innemost.LogLife.API.Models;
using Innemost.LogLife.API.Queries.Model;

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

        public async Task<IEnumerable<string>> FindPathsOfUserByUserId(string userId)
        {
            using (var connection=new MySqlConnection(_connectionString))
            {
                connection.Open();

                var paths = await connection.QueryAsync<string>(
                    @"SELECT l.Path
                        FROM LifeRecord l
                        WHERE l.UserId=@userId"
                    , new { userId = userId });

                return paths;
            }
        }

        public async Task<LifeRecord> FindRecordByRecordId(int id)
        {
            using (var connection=new MySqlConnection(_connectionString))
            {
                connection.Open();

                var recordTask=connection.QueryFirstOrDefaultAsync<LifeRecord>(
                    @"SELECT l.id,l.Title,l.Text,l.TextTypeId,
                        l.Location_Province,l.Location_City,l.Location_County,l.Location_Town,l.Location_Place,
                        l.PublishTime,l.MusicRecordId
                        FROM LifeRecord l
                        WHERE l.id=@id"
                    , new { id=id }
                    );
                //TODO
                var record = await recordTask;
                return record;
            }
        }

        public Task<IEnumerable<LifeRecord>> FindRecordsByEmotionTagsAsync(string userId, IEnumerable<int> emotionTagIds)
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

                var recordsTask= connection.QueryAsync<LifeRecord>(
                    @"SELECT l.id,l.Title,l.Text,l.TextTypeId,
                        l.Location_Province,l.Location_City,l.Location_County,l.Location_Town,l.Location_Place,
                        l.PublishTime,l.MusicRecordId
                        FROM LifeRecord l
                        WHERE l.Path=@path AND l.UserId=@userId"
                    , new {path=path,userId=userId}
                    );
                //TODO 去Redis找EmotionTags、去MongoDB找图片
                var records = await recordsTask;
                return records;
            }
        }

        public async Task<IEnumerable<LifeRecord>> FindRecordsByPublishTimeAsync(string userId, DateTimeToFind dateTime)
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

        public async Task<bool> IsPathExistedAsync(string userId, string path)
        {
            using (var connection=new MySqlConnection(_connectionString))
            {
                connection.Open();

                var pathExisted =await connection.QueryFirstOrDefaultAsync<int?>(
                    @"SELECT 1 
                        FROM LifeRecord l 
                        WHERE l.Path=@path 
                        And UserId=@userId"
                    , new { path = path, userId = userId });

                return pathExisted.HasValue?true:false;
            }
        }
    }
}

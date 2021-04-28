
using MySqlConnector;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Innemost.LogLife.API.Models;
using Innemost.LogLife.API.Queries.Model;
using Innemost.LogLife.API.Queries.Models;

namespace Innemost.LogLife.API.Queries
{
    public class LifeRecordQueries
        : ILifeRecordQueries
    {
        private readonly string _connectionString = string.Empty;
        public LifeRecordQueries(string connectionString)
        {
            _connectionString = string.IsNullOrWhiteSpace(connectionString) ? throw new ArgumentNullException(nameof(connectionString)) : connectionString;
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

                var recordTask = connection.QueryFirstOrDefaultAsync<dynamic>(
                    @"SELECT l.Id,l.Title,l.Text,l.Path,l.PublishTime,l.IsShared,
                        lo.Province,lo.City,lo.County,lo.Town,lo.Place,
                        m.MusicName,m.Singer,m.Album,
                        t.TextTypeName
                        FROM LifeRecord l
                        INNER JOIN Location lo ON l.LocationId=lo.Id
                        INNER JOIN MusicRecord m ON l.MusicRecordId=m.Id
                        INNER JOIN TextType t ON l.TextTypeId=t.Id
                        WHERE l.Id=@id"
                    ,
                    param: new { id = id }
                    );
                //TODO
                var dynamicRecords = await recordTask;

                if (dynamicRecords == null)
                    return null;

                return QueryModelMapper.MapLifeRecordQueryModel(dynamicRecords);
            }
        }

        public Task<IEnumerable<LifeRecord>> FindRecordsByEmotionTagsAsync(string userId, IEnumerable<int> emotionTagIds)
        {
            //TODO 将标签系统放到MongoDB中
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

                var recordsTask= connection.QueryAsync<dynamic>(
                    @"SELECT l.Id,l.Title,l.Text,l.Path,l.PublishTime,l.IsShared,
                        lo.Province,lo.City,lo.County,lo.Town,lo.Place,
                        m.MusicName,m.Singer,m.Album,
                        t.TextTypeName
                        FROM LifeRecord l
                        INNER JOIN Location lo ON l.LocationId=lo.Id
                        INNER JOIN MusicRecord m ON l.MusicRecordId=m.Id
                        INNER JOIN TextType t ON l.TextTypeId=t.Id
                        WHERE l.UserId=@userId AND l.Path=@path"
                    , new {path=path,userId=userId}
                    );
                //TODO 去Redis找EmotionTags、去MongoDB找图片
                var dynamicRecords = await recordsTask;

                if (dynamicRecords == null)
                    return null;

                return QueryModelMapper.MapLifeRecordQueryModel(dynamicRecords);
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

                var recordsTask= connection.QueryAsync<LifeRecord>(
                    @"SELECT l.Id,l.Title,l.Text,l.Path,l.PublishTime,l.IsShared,
                        lo.Province,lo.City,lo.County,lo.Town,lo.Place,
                        m.MusicName,m.Singer,m.Album,
                        t.TextTypeName
                        FROM LifeRecord l
                        INNER JOIN Location lo ON l.LocationId=lo.Id
                        INNER JOIN MusicRecord m ON l.MusicRecordId=m.Id
                        INNER JOIN TextType t ON l.TextTypeId=t.Id
                        WHERE l.UserId=@userId
                        AND l.DateTime>=@startTime AND l.DateTime<=@endTime"
                    , new { startTime = startTime,endTime=endTime, userId = userId }
                    );

                var dynamicRecords = await recordsTask;
                //TODO 去Redis找EmotionTags、去MongoDB找图片
                if (dynamicRecords == null)
                    return null;

                return QueryModelMapper.MapLifeRecordQueryModel(dynamicRecords);
            }
        }

        public async Task<IEnumerable<IGrouping<string,LifeRecord>>> FindRecordsGroupByPathAsync(string userId)
        {
            using (var connetion=new MySqlConnection(_connectionString))
            {
                connetion.Open();

                var recordsBeforeGroupingTask = connetion.QueryAsync<LifeRecord>(
                    @"SELECT l.Id,l.Title,l.Text,l.Path,l.PublishTime,l.IsShared,
                        lo.Province,lo.City,lo.County,lo.Town,lo.Place,
                        m.MusicName,m.Singer,m.Album,
                        t.TextTypeName
                        FROM LifeRecord l
                        INNER JOIN Location lo ON l.LocationId=lo.Id
                        INNER JOIN MusicRecord m ON l.MusicRecordId=m.Id
                        INNER JOIN TextType t ON l.TextTypeId=t.Id
                        WHERE l.UserId=@userId"
                    , new {userId = userId }
                );
                var dynamicRecords = await recordsBeforeGroupingTask;
                //TODO 去Redis找EmotionTags、去MongoDB找图片
                if (dynamicRecords == null)
                    return null;

                var records = QueryModelMapper.MapLifeRecordQueryModel(dynamicRecords);

                return records.GroupBy(l => l.Path);
            }
        }

        /// <summary>
        /// 貌似没啥用，因为不存在path，一定不会有对应的record
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="path"></param>
        /// <returns></returns>
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

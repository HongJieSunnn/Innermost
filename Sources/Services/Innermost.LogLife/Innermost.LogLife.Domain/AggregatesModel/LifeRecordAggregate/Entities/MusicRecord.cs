using Innermost.LogLife.Domain.Events;
using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    /// <summary>
    /// we get music record in front-end when we create a record.
    /// </summary>
    public class MusicRecord
        : Entity
    {
        //Id即对应 Innermost.Music.Music.Id
        public string MusicName { get; private set; }
        public string Singer { get; private set; }
        public string Album { get; private set; }

        public MusicRecord(MusicRecord musicRecord)
        {
            Id = musicRecord.Id;
            MusicName = musicRecord.MusicName;
            Singer = musicRecord.Singer;
            Album = musicRecord.Album;
        }

        /// <summary>
        /// 构造时就要传入Id，因为该Id一定对应MusicHub.Music
        /// </summary>
        /// <param name="id"></param>
        /// <param name="musicName"></param>
        /// <param name="singer"></param>
        /// <param name="album"></param>
        public MusicRecord(int id,string musicName,string singer,string album)
        {
            Id = id;
            MusicName = musicName;
            Singer = singer;
            Album = album;
        }
        /// <summary>
        /// when we want a music detail like story of music and so on we call this method.
        /// </summary>
        public MusicDetail GetMusicDetail()
        {
            var toGetMusicDetailDomainEvent = new ToGetMusicDetailDomainEvent(Id);
            AddDomainEvent(toGetMusicDetailDomainEvent);//TODO 发送该领域事件，通过gRPC获得详情，放在ToGetMusicDetailDomainEvent里
            return toGetMusicDetailDomainEvent.MusicDetail ?? throw new NullReferenceException(nameof(toGetMusicDetailDomainEvent.MusicDetail));
        }
    }
}

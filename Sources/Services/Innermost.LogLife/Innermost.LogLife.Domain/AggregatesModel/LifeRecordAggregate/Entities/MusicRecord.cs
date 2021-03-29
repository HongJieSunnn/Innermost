using Innermost.LogLife.Domain.Events;
using Innermost.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate
{
    public class MusicRecord
        : Entity
    {
        //Id即对应 Innermost.Music.Music.Id
        public string MusicName { get; private set; }
        public string Singer { get; private set; }
        public string Album { get; private set; }

        public MusicRecord(string musicName,string singer,string album)
        {
            MusicName = musicName;
            Singer = singer;
            Album = album;
        }

        public void GetMusicDetail()
        {
            AddDomainEvent(new ToGetMusicDetailDomainEvent(Id));//TODO 发送该领域事件，发出集成事件去Innermost.Music获得数据。然后Innermost.Music处理后也发送一个集成事件让ReceiveMusicDetailHandler来处理。
        }
    }
}

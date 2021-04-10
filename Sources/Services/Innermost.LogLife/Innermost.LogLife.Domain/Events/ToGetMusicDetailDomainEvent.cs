using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.LogLife.Domain.Events
{
    public class ToGetMusicDetailDomainEvent
        :INotification
    {
        public int MusicId { get;private set; }
        public MusicDetail MusicDetail { get; set; }
        public ToGetMusicDetailDomainEvent(int musicId)
        {
            MusicId = musicId;
        }
    }

    public record MusicDetail
    {
        public string Introduction { get; }

        public string ReleaseTime { get; }

        public List<string> Tags { get; }

        //TODO SharedLifeRecordDTO
    }
}

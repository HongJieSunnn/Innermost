using Innermost.LogLife.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Services.GprcServices
{
    public interface IMusicHubGrpcService
    {
        Task<MusicDetail> GetMusicDetailByMusicId(int id);
    }
}

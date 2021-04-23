using AutoMapper;
using Innermost.GrpcMusicHub;
using Innermost.LogLife.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Innermost.GrpcMusicHub.MusicHubGrpc;

namespace Innemost.LogLife.API.Services.GprcServices
{
    public class MusicHubGrpcService : IMusicHubGrpcService
    {
        private readonly MusicHubGrpcClient _musicHubGrpcClient;
        private readonly IMapper _mapper;
        public MusicHubGrpcService(MusicHubGrpcClient musicHubGrpcClient,IMapper mapper)
        {
            _musicHubGrpcClient = musicHubGrpcClient ?? throw new ArgumentNullException(nameof(musicHubGrpcClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        public async Task<MusicDetail> GetMusicDetailByMusicId(int id)
        {
            var musicDetailDto = await _musicHubGrpcClient.GetMusicDetailAsync(new MusicRecordDTO() { Id = id });
            var musicDetail = _mapper.Map<MusicDetail>(musicDetailDto);
            return musicDetail ?? throw new NullReferenceException(nameof(musicDetail));
        }
    }
}

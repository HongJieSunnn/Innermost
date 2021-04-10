using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innermost.GrpcMusicHub
{
    public class MusicHubGrpcService:MusicHubGrpc.MusicHubGrpcBase
    {
        public override Task<MusicDetailDTO> GetMusicDetail(MusicRecordDTO request, ServerCallContext context)
        {
            //TODO
            return base.GetMusicDetail(request, context);
        }
    }
}

using Innemost.LogLife.API.Services.GprcServices;
using Innermost.LogLife.Domain.Events;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Innermost.GrpcMusicHub.MusicHubGrpc;

namespace Innemost.LogLife.API.Application.DomainEventHandlers.ToGetMusicDetail
{
    public class ToGetMusicDetailDomainEventHandler
        : INotificationHandler<ToGetMusicDetailDomainEvent>
    {
        private readonly IMusicHubGrpcService _musicDetailGrpcService;
        public ToGetMusicDetailDomainEventHandler(IMusicHubGrpcService musicDetailGrpcService)
        {
            _musicDetailGrpcService = musicDetailGrpcService ?? throw new ArgumentNullException(nameof(musicDetailGrpcService));
        }
        public async Task Handle(ToGetMusicDetailDomainEvent notification, CancellationToken cancellationToken)
        {
            notification.MusicDetail =await _musicDetailGrpcService.GetMusicDetailByMusicId(notification.MusicId);
        }
    }
}

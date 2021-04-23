using Innemost.LogLife.API.Application.Commands;
using Innemost.LogLife.API.Services.IdentityServices;
using Innermost.LogLife.Domain.AggregatesModel.LifeRecordAggregate;
using Innermost.LogLife.Infrastructure.Idempotency;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.CommandHandlers
{
    public class DeleteRecordsByPathCommandHandler : IRequestHandler<DeleteRecordsByPathCommand, bool>
    {
        private readonly ILifeRecordRepository _lifeRecordRepository;
        private readonly IIdentityService _identityService;
        private readonly ILogger<DeleteOneRecordCommand> _logger;
        public DeleteRecordsByPathCommandHandler(ILifeRecordRepository lifeRecordRepository,IIdentityService identityService, ILogger<DeleteOneRecordCommand> logger)
        {
            _lifeRecordRepository = lifeRecordRepository ?? throw new ArgumentNullException(nameof(lifeRecordRepository));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(DeleteRecordsByPathCommand request, CancellationToken cancellationToken)
        {
            var userId = _identityService.GetUserId();
            _logger.LogInformation($"Delete records under path {request.Path} of User with UserId {userId}");
            var recordsDeleted=_lifeRecordRepository.DeleteRecordsUnderPath(request.Path);

            if (recordsDeleted == null)
                return false;

            return await _lifeRecordRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }

    public class IdempotencyDeleteRecordsByPathCommandHandler:IdempotencyCommandHandler<DeleteRecordsByPathCommand, bool>
    {
        public IdempotencyDeleteRecordsByPathCommandHandler(IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdempotencyCommandHandler<DeleteRecordsByPathCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {

        }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}

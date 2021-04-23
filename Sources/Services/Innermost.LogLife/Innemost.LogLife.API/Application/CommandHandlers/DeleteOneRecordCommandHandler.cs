using Innemost.LogLife.API.Application.Commands;
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
    public class DeleteOneRecordCommandHandler : IRequestHandler<DeleteOneRecordCommand, bool>
    {
        private readonly ILifeRecordRepository _lifeRecordRepository;
        private readonly ILogger<DeleteOneRecordCommand> _logger;
        public DeleteOneRecordCommandHandler(ILifeRecordRepository lifeRecordRepository,ILogger<DeleteOneRecordCommand> logger)
        {
            _lifeRecordRepository = lifeRecordRepository ?? throw new ArgumentNullException(nameof(lifeRecordRepository));
            _logger=logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<bool> Handle(DeleteOneRecordCommand request, CancellationToken cancellationToken)
        {
            var recordToDelete =await _lifeRecordRepository.GetRecordByIdAsync(request.Id);

            if (recordToDelete == null)
                return false;

            _logger.LogInformation("Delete Record with Id{@LifeRecord}", recordToDelete);
            _lifeRecordRepository.Delete(recordToDelete);
            return await _lifeRecordRepository.UnitOfWork.SaveEntitiesAsync();
        }
    }

    public class IdempotencyDeleteOneRecordCommandHandler:IdempotencyCommandHandler<DeleteOneRecordCommand,bool>
    {
        public IdempotencyDeleteOneRecordCommandHandler(IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdempotencyCommandHandler<DeleteOneRecordCommand, bool>> logger)
            : base(mediator, requestManager, logger)
        {

        }

        protected override bool CreateDefaultResult()
        {
            return true;
        }
    }
}

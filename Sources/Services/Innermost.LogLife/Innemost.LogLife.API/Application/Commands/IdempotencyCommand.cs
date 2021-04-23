using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Commands
{
    public class IdempotencyCommand<TCommand,TResult>:IRequest<TResult>
        where TCommand:IRequest<TResult>
    {
        public TCommand Command { get; private set; }
        public Guid Id { get; private set; }
        public IdempotencyCommand(TCommand command,Guid id)
        {
            Command = command;
            Id = id;
        }
    }
}

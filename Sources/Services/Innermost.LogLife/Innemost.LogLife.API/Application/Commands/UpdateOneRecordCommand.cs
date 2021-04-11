using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Commands
{
    public class UpdateOneRecordCommand:IRequest<bool>
    {
        public int Id { get; private set; }
        public UpdateOneRecordCommand(int id)
        {
            Id = id;
        }
    }
}

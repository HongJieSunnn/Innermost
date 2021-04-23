using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Commands
{
    [DataContract]
    public class DeleteOneRecordCommand:IRequest<bool>
    {
        [DataMember]
        public int Id { get; private set; }

        [JsonConstructor]
        public DeleteOneRecordCommand(int id)
        {
            Id = id;
        }
    }
}

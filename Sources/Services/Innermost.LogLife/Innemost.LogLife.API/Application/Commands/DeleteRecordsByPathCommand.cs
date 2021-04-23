﻿using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Innemost.LogLife.API.Application.Commands
{
    [DataContract]
    public class DeleteRecordsByPathCommand:IRequest<bool>
    {
        [DataMember]
        public string Path { get;private set; }

        [JsonConstructor]
        public DeleteRecordsByPathCommand(string path)
        {
            Path = path;
        }
    }
}

using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.EventBusServiceBus
{
    public interface IServiceBusPersisterConnection:IAsyncDisposable
    {
        string ServiceBusConnectionString { get; }
        ServiceBusClient CreateModel();
        ServiceBusAdministrationClient CreateAdministrationModel();
    }
}

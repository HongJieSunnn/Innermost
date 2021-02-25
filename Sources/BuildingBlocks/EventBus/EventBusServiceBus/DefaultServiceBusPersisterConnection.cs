using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Innermost.EventBusServiceBus
{
    public class DefaultServiceBusPersisterConnection : IServiceBusPersisterConnection
    {
        private readonly string _serviceBusConnectionString;
        private readonly ILogger<DefaultServiceBusPersisterConnection> _logger;
        private ServiceBusClient _serviceBusClient;
        private ServiceBusAdministrationClient _serviceBusAdministrationClient;

        private bool _disposed;
        public DefaultServiceBusPersisterConnection(string serviceBusConnectionString,ILogger<DefaultServiceBusPersisterConnection> logger)
        {
            _logger = logger ?? throw new ArgumentException(nameof(logger));
            _serviceBusConnectionString = serviceBusConnectionString ?? throw new ArgumentException(nameof(serviceBusConnectionString));
            _serviceBusClient = new ServiceBusClient(_serviceBusConnectionString);//TODO 我不知道默认情况下是否会配置Retry，若不会则需通过ServiceBusClientOptions来配置
            _serviceBusAdministrationClient = new ServiceBusAdministrationClient(_serviceBusConnectionString);
        }
        public string ServiceBusConnectionString => _serviceBusConnectionString;

        public ServiceBusClient CreateModel()
        {
            if(_serviceBusClient.IsClosed)
            {
                return new ServiceBusClient(_serviceBusConnectionString);
            }
            return _serviceBusClient;
        }

        public ServiceBusAdministrationClient CreateAdministrationModel()
        {
            if(_serviceBusAdministrationClient==null)
            {
                return new ServiceBusAdministrationClient(_serviceBusConnectionString);
            }
            return _serviceBusAdministrationClient;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                await _serviceBusClient.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex.ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationEventRecord.Services
{
    public class IntegrationEventRecordServiceFactory
    {
        public IIntegrationEventRecordService NewService(DbConnection connection)
        {
            return new IntegrationEventRecordService(connection);
        }
    }
}

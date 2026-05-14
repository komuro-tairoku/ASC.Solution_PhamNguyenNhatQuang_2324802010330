using ASC.Buisiness.Interfaces;
using ASC.Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Buisiness
{
    public class ServiceRequestMessageOperations :
        IServiceRequestMessageOperations
    {
        private static readonly
            List<ServiceRequestMessage> _messages = new();

        public async Task CreateServiceRequestMessageAsync(
            ServiceRequestMessage message)
        {
            _messages.Add(message);

            await Task.CompletedTask;
        }

        public async Task<List<ServiceRequestMessage>>
            GetServiceRequestMessageAsync(
                string serviceRequestId)
        {
            return await Task.FromResult(
                _messages
                    .Where(x =>
                        x.PartitionKey == serviceRequestId)
                    .ToList());
        }
    }
}

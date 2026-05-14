using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Model.Models
{
    public class ServiceRequestMessage
    {
        public string Message { get; set; }

        public string FromEmail { get; set; }

        public string FromDisplayName { get; set; }

        public DateTime? MessageDate { get; set; }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }
    }
}

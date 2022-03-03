using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSource
{
    interface IAwesomeEvent
    {
        public DateTime FiredAt { get; set; }
        public Guid Id { get; set; }

        public string ToJson();
    }
}

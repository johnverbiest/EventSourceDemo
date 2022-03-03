using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventSource.Events
{
    abstract class BaseEvent : IAwesomeEvent
    {
        public DateTime FiredAt { get; set; }
        public Guid Id { get; set; }

        public BaseEvent()
        {
            FiredAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public string ToJson()
        {
            return $"// {this.GetType().Name}\n{Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented)}";
        }
    }
}

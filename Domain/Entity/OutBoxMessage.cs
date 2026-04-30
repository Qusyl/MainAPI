using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entity
{
    public class OutBoxMessage : IAppEntity
    {
        public Guid Id { get; set; }

        public string Type { get;  set; }

        public string Payload { get;  set; }

        public int Version { get; set; }

        public DateTime OccurredOn { get;  set; }

        public DateTime? ProcessedOn {get; set; }

        public string? Error { get; set; }
        private List<IDomainEvent> _events => new();
        public IReadOnlyCollection<IDomainEvent> Events => _events;

        public OutBoxMessage(
             string type, string payload, int version, DateTime occurredOn, DateTime? processedOn)
        {
            Id = Guid.NewGuid();
            Type = type;
            Payload = payload;
            Version = version;
            OccurredOn = occurredOn;
            ProcessedOn = processedOn;
        }
        public void MarkProcessed (DateTime timeProcessed)
        {
            ProcessedOn = timeProcessed;
        }
        public void SetError(string error)
        {
            Error = error;
        }
        public void ClearEvents()
        {
            _events.Clear();
        }

    }
}

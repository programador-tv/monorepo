using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IRabbitMQAdapter
    {
        public void Publish(string queue, byte[] message);
        public void Notify(Notification notification);
    }
}

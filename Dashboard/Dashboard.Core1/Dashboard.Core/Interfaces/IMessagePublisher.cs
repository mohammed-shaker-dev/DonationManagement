using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Core.Interfaces
{
    public interface IMessagePublisher
    {
        Task Publish(Object eventToPublish);
    }
}

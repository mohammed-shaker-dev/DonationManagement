
using Dashboard.Core.Interfaces;
using MediatR;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure
{
    public class MessagePublisher : IMessagePublisher
    {
        private readonly IMediator _mediator;

        public MessagePublisher(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Publish(object eventToPublish)
        {
            await _mediator.Publish(eventToPublish);
        }
    }
}

using Dashboard.Core.Events;
using Dashboard.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Dashboard.Infrastructure.EventHandlers
{
    public class TransactionCreatedEventHandler : INotificationHandler<TransactionCreatedEvent>
    {
        private readonly ILogger<TransactionCreatedEventHandler> _logger;
        private readonly IEmailSender _emailSender;

        public TransactionCreatedEventHandler(
            ILogger<TransactionCreatedEventHandler> logger,
            IEmailSender emailSender)
        {
            _logger = logger;
            _emailSender = emailSender;
        }

        public async Task Handle(TransactionCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Transaction created: {TransactionId}", notification.TransactionCreated.Id);

            //// Send email notification if email is provided
            //if (!string.IsNullOrEmpty(notification.TransactionCreated.Email))
            //{
            //    await _emailSender.SendEmailAsync(
            //        to: notification.TransactionCreated.Email,
            //        from: "noreply@donationapp.com",
            //        subject: "Donation Receipt",
            //        body: $"Thank you for your donation! Your transaction code is: {notification.TransactionCreated.Code}"
            //    );
            //}
        }
    }

    public class TransactionUpdatedEventHandler : INotificationHandler<TransactionUpdatedEvent>
    {
        private readonly ILogger<TransactionUpdatedEventHandler> _logger;

        public TransactionUpdatedEventHandler(ILogger<TransactionUpdatedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TransactionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Transaction updated: {TransactionId}", notification.TransactionUpdated.Id);

            // Add any additional logic for transaction updates

            return Task.CompletedTask;
        }
    }

    public class TransactionDeletedEventHandler : INotificationHandler<TransactionDeletedEvent>
    {
        private readonly ILogger<TransactionDeletedEventHandler> _logger;

        public TransactionDeletedEventHandler(ILogger<TransactionDeletedEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(TransactionDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Transaction deleted: {TransactionId}", notification.TransactionDeleted.Id);

            // Add any additional logic for transaction deletions

            return Task.CompletedTask;
        }
    }
}

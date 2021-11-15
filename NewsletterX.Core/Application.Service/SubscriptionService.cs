using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using NewsletterX.Core.Domain.Model;
using NewsletterX.Core.Domain.Service;

namespace NewsletterX.Core.Application.Service
{
    public class SubscriptionService
    {
        private readonly IEmailService _emailService;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriptionService(
            IEmailService emailService,
            ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _emailService = emailService;
        }

        public async Task<bool> Subscribe(Subscription request)
        {
            var subscription = new Subscription(request.Name, request.Email);
            var isCreated = await _subscriptionRepository.Create(subscription);
            if (!isCreated) return false;
            var email = new ConfirmSubscriptionEmail(
                request.Email, "NewsletterX@mail.com", subscription.VerificationCode);
            var isSent = await _emailService.Send(email);
            return isSent;
        }

        public async Task<bool> Verify(Subscription verificationRequest)
        {
            var subscription = await _subscriptionRepository.ReadByEmail(verificationRequest.Email);
            if (subscription == null || verificationRequest.VerificationCode != subscription.VerificationCode)
            {
                return false;
            }
            subscription.IsVerified = true;
            var hasUpdated = await _subscriptionRepository.Update(subscription);
            return hasUpdated;
        }
    }
}

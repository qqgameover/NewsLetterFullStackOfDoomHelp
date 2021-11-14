using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsletterX.Core.Application.Service;
using NewsletterX.Core.Domain.Model;

namespace NewsletterX.Infrastructure.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : ControllerBase
    {
        private readonly SubscriptionService _subscriptionService;

        public SubscribeController(SubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }
        [HttpPut]
        public async Task<bool> Verify(string email, string code)
        {
            var subscription = new Subscription { Email = email, VerificationCode = code };
            return await _subscriptionService.Verify(subscription);
        }

        [HttpPost]
        public async Task<bool> Subscribe(string email)
        {
            var subscription = new Subscription { Email = email};
            return await _subscriptionService.Subscribe(subscription);
        }
    }
}
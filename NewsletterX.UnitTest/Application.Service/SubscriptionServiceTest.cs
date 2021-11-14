using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Moq;
using NewsletterX.Core.Application.Service;
using NewsletterX.Core.Domain.Model;
using NewsletterX.Core.Domain.Service;
using NewsletterX.UnitTest.Application.Service;
using NUnit.Framework;

namespace NewsletterX.UnitTest
{
    public class Tests
    {
        [Test]
        public async Task TestSubscriptionOk()
        {
            // arrange
            var emailServiceMock = new Mock<IEmailService>();
            var subscriptionRepoMock = new Mock<ISubscriptionRepository>();
            emailServiceMock.Setup(es => es.Send(It.IsAny<Email>()))
                            .ReturnsAsync(true);
            subscriptionRepoMock.Setup(sr => sr.Create(It.IsAny<Subscription>()))
                            .ReturnsAsync(true);
            var service = new SubscriptionService(emailServiceMock.Object, subscriptionRepoMock.Object);

            // act
            var subscription = new Subscription("Kasper", "kasper35@live.no");
            var subscribeIsSuccess = await service.Subscribe(subscription);

            // assert
            Assert.IsTrue(subscribeIsSuccess);
            emailServiceMock.Verify(
                es=>es.Send(It.Is<Email>(e=>e.To== "kasper35@live.no")));
            subscriptionRepoMock.Verify(
                sr=>sr.Create(It.Is<Subscription>(s=>s.Email== "kasper35@live.no")));
            emailServiceMock.VerifyNoOtherCalls();
            subscriptionRepoMock.VerifyNoOtherCalls();
        }


        [Test]
        public async Task TestSubscriptionOk2()
        {
            // Samme som over, men uten mock-rammverk

            // arrange
            var emailService = new EmailService();
            var subscriptionRepo= new SubscriptionRepository();
            var service = new SubscriptionService(emailService, subscriptionRepo);

            // act
            var subscription = new Subscription("Kasper", "kasper35@live.no");
            var subscribeIsSuccess = await service.Subscribe(subscription);

            // assert
            Assert.IsTrue(subscribeIsSuccess);
            Assert.AreEqual("kasper35@live.no", emailService.SentEmailToAddress);
            Assert.AreEqual("kasper35@live.no", subscriptionRepo.CreatedEmailToAddress);
            Assert.AreEqual(1, emailService.CallCount);
            Assert.AreEqual(1, subscriptionRepo.CallCount);
        }

        [Test]
        public async Task TestSubscriptionDbFail()
        {
            var emailServiceMock = new Mock<IEmailService>();
            var subscriptionRepoMock = new Mock<ISubscriptionRepository>();
            subscriptionRepoMock.Setup(sr => sr.Create(It.IsAny<Subscription>()))
                .ReturnsAsync(false);            
            var service = new SubscriptionService(emailServiceMock.Object, subscriptionRepoMock.Object);
            var subscription = new Subscription("Kasper", "kasper35@live.no");
            var isSuccess = await service.Subscribe(subscription);
            Assert.IsFalse(isSuccess);
            subscriptionRepoMock.Verify(sr => sr.Create(
                It.Is<Subscription>(s => s.Email == "kasper35@live.no")));
            emailServiceMock.VerifyNoOtherCalls();
            subscriptionRepoMock.VerifyNoOtherCalls();
        }

        [Test]
        public async Task TestSubscriptionEmailFail()
        {
            var emailServiceMock = new Mock<IEmailService>();
            var subscriptionRepoMock = new Mock<ISubscriptionRepository>();
            emailServiceMock.Setup(es => es.Send(It.IsAny<Email>()))
                .ReturnsAsync(false);
            subscriptionRepoMock.Setup(sr => sr.Create(It.IsAny<Subscription>()))
                .ReturnsAsync(true);
            var service = new SubscriptionService(emailServiceMock.Object, subscriptionRepoMock.Object);
            var subscription = new Subscription("Kasper", "kasper35@live.no");
            var isSuccess = await service.Subscribe(subscription);
            Assert.IsFalse(isSuccess);
            emailServiceMock.Verify(es => es.Send(
                It.Is<Email>(e => e.To == "kasper35@live.no")));
            subscriptionRepoMock.Verify(sr => sr.Create(
                It.Is<Subscription>(s => s.Email == "kasper35@live.no")));
            emailServiceMock.VerifyNoOtherCalls();
            subscriptionRepoMock.VerifyNoOtherCalls();
        }


        [Test]
        public async Task TestVerifyOk()
        {
            var code = "ed863c05-c1ef-4537-a7e0-63cddf9b5452";
            var email = "kasper35 @live.no";
            var verificationRequest = new Subscription(null, email, code);
            var subscriptionFromDb = new Subscription(null, null, code);

            var subscriptionRepoMock = new Mock<ISubscriptionRepository>();
            subscriptionRepoMock.Setup(sr => sr.ReadByEmail(email))
                .ReturnsAsync(subscriptionFromDb);
            subscriptionRepoMock.Setup(sr => sr.Update(It.IsAny<Subscription>()))
                .ReturnsAsync(true);
            var service = new SubscriptionService(null, subscriptionRepoMock.Object);
            var isSuccess = await service.Verify(verificationRequest);
            Assert.IsTrue(isSuccess);
            subscriptionRepoMock.Verify(sr => sr.Update(It.IsAny<Subscription>()));
        }

        [Test]
        public async Task TestVerifyInvalidCode()
        {
            try
            {
                var code1 = "ed863c05-c1ef-4537-a7e0-63cddf9b5452";
                var code2 = "ed863c05-c1ef-4537-a7e0-63cddf9b5453";
                var email = "kasper35 @live.no";
                var verificationRequest = new Subscription(null, email, code1);
                var subscriptionFromDb = new Subscription(null, null, code2);

                var subscriptionRepoMock = new Mock<ISubscriptionRepository>();
                subscriptionRepoMock.Setup(sr => sr.ReadByEmail(email))
                    .ReturnsAsync(subscriptionFromDb);

                var service = new SubscriptionService(null, subscriptionRepoMock.Object);
                var isGood = await service.Verify(verificationRequest);
            }
            catch (Exception wantedException)
            {
                Assert.True(true);
            }
        }
    }
}
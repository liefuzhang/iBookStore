using System;
using NUnit.Framework;
using UserManagement.API.Service;

namespace UserManagement.UnitTests.Service
{
    [TestFixture]
    public class NewsletterServiceTests
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("invalid_email")]
        [TestCase("dd@dd")]
        [TestCase("dd.dd@dd")]
        [TestCase("dd@dd@dd")]
        public void AddEmailToBag_ThrowsException_WhenEmailInvalid(string email)
        {
            var service = new NewsletterService();

            Assert.Throws<ArgumentException>(() => service.AddEmailToBag(email));
        }

        [Test]
        [TestCase("valid_email@dd.dd")]
        [TestCase("valid_email@dd.dd.dd")]
        [TestCase("dd.dd@dd.dd")]
        public void AddEmailToBag_NotThrowException_WhenEmailValid(string email)
        {
            var service = new NewsletterService();

            service.AddEmailToBag(email);
        }
    }
}

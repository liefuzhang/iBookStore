using System;
using NUnit.Framework;
using UserManagement.API.Models;
using UserManagement.API.Service;

namespace UserManagement.UnitTests.Service
{
    [TestFixture]
    public class NewsletterSubsriptionTests
    {
        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("invalid_email")]
        [TestCase("dd@dd")]
        [TestCase("dd.dd@dd")]
        [TestCase("dd@dd@dd")]
        public void NewsletterSubscription_ThrowsException_WhenEmailInvalid(string email)
        {
            Assert.Throws<ArgumentException>(() => new NewsletterSubscription(email));
        }

        [Test]
        [TestCase("valid_email@dd.dd")]
        [TestCase("valid_email@dd.dd.dd")]
        [TestCase("dd.dd@dd.dd")]
        public void NewsletterSubscription_NotThrowException_WhenEmailValid(string email)
        {
            var subscription = new NewsletterSubscription(email);
        }
    }
}

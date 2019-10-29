﻿using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using Ordering.Domain.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class Buyer : Entity, IAggregateRoot
    {
        public string IdentityGuid { get; private set; }

        public string Name { get; private set; }

        private List<PaymentMethod> _paymentMethods;
        public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

        protected Buyer() {
            _paymentMethods = new List<PaymentMethod>();
        }

        public Buyer(string identity, string name) : this() {
            IdentityGuid = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));
            Name = !string.IsNullOrWhiteSpace(name) ? name : throw new ArgumentNullException(nameof(name));
        }

        public PaymentMethod VerifyOrAddPaymentMethod(CardType cardType, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration, int orderId) {
            var existingPayment = _paymentMethods
                .SingleOrDefault(p => p.IsEqualTo(cardType, cardNumber, expiration));

            if (existingPayment != null) {
                AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

                return existingPayment;
            } else {
                var payment = new PaymentMethod(cardType, cardNumber, securityNumber, cardHolderName, expiration);
                _paymentMethods.Add(payment);

                AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

                return payment;
            }
        }
    }
}

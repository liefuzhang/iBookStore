using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ordering.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod : Entity
    {
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolderName;
        private DateTime _expiration;

        // remove after use Dapper
        public string CardNumber => _cardNumber;
        public string SecurityNumber => _securityNumber;
        public string CardHolderName => _cardHolderName;
        public DateTime Expiration => _expiration;

        public CardType CardType { get; private set; }

        protected PaymentMethod() { }

        public PaymentMethod(CardType cardType, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration) {

            CardType = cardType;
            _cardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new ArgumentException(nameof(cardNumber));
            _securityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new ArgumentException(nameof(securityNumber));
            _cardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new ArgumentException(nameof(cardHolderName));
            _expiration = expiration;
        }

        public bool IsEqualTo(CardType cardType, string cardNumber, DateTime expiration) {
            return CardType == cardType
                && _cardNumber == cardNumber
                && _expiration == expiration;
        }
    }
}

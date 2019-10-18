using System;
using System.ComponentModel.DataAnnotations;

namespace iBookStoreMVC.ViewModels.Annotations
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
    public class CardExpirationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value) {
            if (value == null)
                return false;

            var monthString = value.ToString().Split('/')[0];
            var yearString = $"20{value.ToString().Split('/')[1]}";
            // Use the 'out' variable initializer to simplify 
            // the logic of validating the expiration date
            if ((int.TryParse(monthString, out var month)) &&
                (int.TryParse(yearString, out var year))) {
                var dateTime = new DateTime(year, month, 1);

                return dateTime > DateTime.UtcNow;
            }

            return false;
        }
    }
}
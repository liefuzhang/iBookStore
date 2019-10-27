using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace iBookStoreMVC.Models
{
    public enum OrderStatus
    {
        Submitted,
        AwaitingValidation,
        StockConfirmed,
        Paid,
        Shipped,
        Cancelled,
        StockRejected
    }
}

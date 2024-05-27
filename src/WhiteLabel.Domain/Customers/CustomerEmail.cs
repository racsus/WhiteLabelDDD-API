using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Customers
{
    public class CustomerEmail : BaseEntity<int>
    {
        public int CustomerId { get; set; }
        public string EmailAddress { get; set; }
    }
}

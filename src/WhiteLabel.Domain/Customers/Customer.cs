using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Generic;

namespace WhiteLabel.Domain.Customers
{
    public class Customer : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}

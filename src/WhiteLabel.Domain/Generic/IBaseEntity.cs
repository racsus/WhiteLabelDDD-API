using System;
using System.Collections.Generic;
using System.Text;

namespace WhiteLabel.Domain.Generic
{
    public interface IBaseEntity<T>
    {
        T Id { get; set; }
    }
}

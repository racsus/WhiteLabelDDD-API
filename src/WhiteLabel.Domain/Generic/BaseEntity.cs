using System;
using System.Collections.Generic;
using System.Text;
using WhiteLabel.Domain.Pagination;

namespace WhiteLabel.Domain.Generic
{
    public abstract class BaseEntity<T>: BaseEntityWithEvents, IBaseEntity<T>
    {
        public T Id { get; set; }

    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel
{
    public abstract class BaseEntity <TId>
    {
        public TId Id { get; set; }
        public DateTime CreatedDate { get; set; }=DateTime.UtcNow;
        public List<BaseDomainEvent> Events = new List<BaseDomainEvent>();
    }
}

using System;
using System.Collections.Generic;

namespace JPP.Models.CustomerEvent.Request
{
    public class CustomerEventSaveRequest
    {
        public int CustomerId { get; set; }
        public List<int> EventIds { get; set; } = new List<int>();
    }
}
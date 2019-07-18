using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.MatchingDbModel
{
    public class Customer
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }

        public string FullName => $"[{Name}] {Address}";
    }
}

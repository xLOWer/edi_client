﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.MatchingDbModel
{
    public class Matches
    {
        public string CUSTOMER_GLN { get; set; }
        public string EAN { get; set; }
        public string ID_GOOD { get; set; }
        public string NAME { get; set; }
        public string INSERT_DATETIME { get; set; }
    }
}
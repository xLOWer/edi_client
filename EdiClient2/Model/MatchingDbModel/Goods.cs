﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdiClient.Model.MatchingDbModel
{
    public class Goods
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string GoodSize { get; set; }
        public string Manufacturer { get; set; }
        public string ManufacturerId { get; set; }
    }
}

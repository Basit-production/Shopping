using System;
using System.Collections.Generic;
using System.Linq;


namespace Ahmed_mart.Dtos.v1.PrefixesDto
{
    public class AddPrefixesDto
    {
        public int TransactionType { get; set; }
        public int TransactionLength { get; set; }
        public string Prefix { get; set; }
        public int StartNumber { get; set; }
        public int CurrentNumber { get; set; }
        public bool Status { get; set; } = true;
    }
}

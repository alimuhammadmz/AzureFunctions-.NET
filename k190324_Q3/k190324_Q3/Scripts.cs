using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace k190324_Q3
{
    public class scriptData
    {
        public string lastUpdatedOn { get; set; }
        public List<Script> scripts { get; set; }
    }
    public class Script
    {
        public string date { get; set; }
        public string price { get; set; }
    }
}
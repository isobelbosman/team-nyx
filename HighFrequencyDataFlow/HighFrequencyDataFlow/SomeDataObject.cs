using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HighFrequencyDataFlow
{
    public class SomeDataObject
    {
        public SomeDataObject(string someAttributeOne, string someAttributeTwo, string someAttributeTwe, string someAttributeFou)
        {
            SomeAttributeOne = someAttributeOne;
            SomeAttributeTwo = someAttributeTwo;
            SomeAttributeTwe = someAttributeTwe;
            SomeAttributeFou = someAttributeFou;
        }

        public string SomeAttributeOne { get; set; }
        public string SomeAttributeTwo { get; set; }
        public string SomeAttributeTwe { get; set; }
        public string SomeAttributeFou { get; set; }
    }
}

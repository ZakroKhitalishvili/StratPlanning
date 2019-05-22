using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class ValueAnswer : AbstractAnswer
    {
        public ValueAnswer()
        {

        }

        public string Value { get; set; }

        public string Definition { get; set; }

        public string Description { get; set; }
    }
}

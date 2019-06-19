using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Setting
    {
        public int Id { get; set; }

        public string Index { get; set; }

        public string Value { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}

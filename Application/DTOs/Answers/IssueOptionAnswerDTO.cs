using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class IssueOptionAnsweDTO
    {
        public int IssueId { get; set; }

        public string Option { get; set; }

        public bool IsBestOption { get; set; }

        public string Actors { get; set; }
    }
}

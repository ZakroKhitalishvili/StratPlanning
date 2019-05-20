using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class TextAnswerDTO
    {
        [Required]
        public string Text { get; set; }

        public bool IsIssue { get; set; }

        public bool IsStakeholder { get; set; }

    }
}

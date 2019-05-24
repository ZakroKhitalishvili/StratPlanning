using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class StakeholderAnswerDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public bool IsInternal { get; set; }

        public int? UserId { get; set; }

        public int? CategoryId { get; set; }

        public string Category { get; set; }
    }
}

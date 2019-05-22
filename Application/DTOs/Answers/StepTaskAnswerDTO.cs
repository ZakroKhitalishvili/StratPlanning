using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class StepTaskAnswerDTO
    {
        public int Id { get; set; }

        public string Step { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? UserToPlanId { get; set; }
    }
}

using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class StepTaskAnswerDTO
    {
        public int Id { get; set; }

        public string Step { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string Email { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string FirstName { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string LastName { get; set; }

        public int? UserToPlanId { get; set; }
    }
}

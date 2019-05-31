using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class StakeholderAnswerDTO
    {
        public int Id { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string FirstName { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string LastName { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        public string Email { get; set; }

        public bool IsInternal { get; set; }

        public int? UserId { get; set; }

        public int? CategoryId { get; set; }

        public string Category { get; set; }
    }
}

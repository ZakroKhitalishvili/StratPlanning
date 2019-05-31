using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class NewUserDTO
    {
        [Required]
        [EmailAddress]
        [MaxLength(EntityConfigs.TextMaxLength)]
        public string Email { get; set; }

        [Required]
        [MaxLength(EntityConfigs.TextMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(EntityConfigs.TextMaxLength)]
        public string LastName { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        [Required]
        public int? PositionId { get; set; }
    }
}

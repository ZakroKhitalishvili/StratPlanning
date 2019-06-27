using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class UserEditDTO
    {
        [Required]
        public int Id { get; set; }

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

        [Required]
        public string Role { get; set; }

        public int? PositionId { get; set; }
    }
}

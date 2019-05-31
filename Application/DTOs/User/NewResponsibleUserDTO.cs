using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class NewResponsibleUserDTO
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

        [Required]
        public string Step { get; set; }

    }
}

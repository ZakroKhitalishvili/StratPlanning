using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Core.Constants;
namespace Application.DTOs
{
    public class UserLoginDTO
    {
        
        [Required]
        [EmailAddress]
        [MaxLength(EntityConfigs.TextMaxLength)]
        public string Email { get; set; }

        [Required]
        [MaxLength(EntityConfigs.TextMaxLength)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
    }
}

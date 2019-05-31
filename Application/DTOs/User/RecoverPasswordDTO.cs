using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class RecoverPasswordDTO
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [MaxLength(EntityConfigs.TextMaxLength)]
        public string NewPassword { get; set; }

        [Required]
        [MaxLength(EntityConfigs.TextMaxLength)]
        [Compare(nameof(NewPassword),ErrorMessage = "Passwords are not equal")]
        public string ConfirmNewPassword { get; set; }
    }
}

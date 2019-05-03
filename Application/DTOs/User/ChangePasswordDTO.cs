﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class ChangePasswordDTO
    {
        public int Id { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords are not equal")]
        public string ConfirmNewPassword { get; set; }
    }
}

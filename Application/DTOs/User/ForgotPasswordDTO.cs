using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class ForgotPasswordDTO
    {
        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [EmailAddress(ErrorMessageResourceName = "validateEmail", ErrorMessageResourceType = typeof(sharedResource))]
        public string Email { get; set; }
    }
}

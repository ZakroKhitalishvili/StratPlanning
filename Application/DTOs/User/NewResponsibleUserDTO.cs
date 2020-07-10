using Core.Constants;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class NewResponsibleUserDTO
    {
        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [EmailAddress(ErrorMessageResourceName = "validateEmail", ErrorMessageResourceType = typeof(sharedResource))]
        [MaxLength(EntityConfigs.TextMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Email { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [MaxLength(EntityConfigs.TextMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [MaxLength(EntityConfigs.TextMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        public string Step { get; set; }

    }
}

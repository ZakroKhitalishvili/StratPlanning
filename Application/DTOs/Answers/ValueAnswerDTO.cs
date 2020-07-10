using Core.Constants;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class ValueAnswerDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [MaxLength(EntityConfigs.TextMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Value { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [MaxLength(EntityConfigs.TextAreaMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Definition { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [MaxLength(EntityConfigs.TextAreaMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Description { get; set; }
    }
}

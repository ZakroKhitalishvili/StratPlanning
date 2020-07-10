using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Resources;
namespace Application.DTOs
{
    public class SettingDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        public string Index { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [StringLength(EntityConfigs.LargeTextMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]
        public string Value { get; set; }
    }
}

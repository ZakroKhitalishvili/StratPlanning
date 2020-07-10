using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Resources;

namespace Application.DTOs
{
    public class DictionaryDTO
    {
        public int Id { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]
        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        public string Title { get; set; }

        public bool HasPosition { get; set; }

        public bool HasValue { get; set; }

        public bool HasStakeholderCategory { get; set; }

        public bool HasStakeholderCriteria { get; set; }

        public bool IsActive { get; set; }
    }
}

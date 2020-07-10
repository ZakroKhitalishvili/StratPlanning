using Core.Constants;
using Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class IssueOptionAnsweDTO
    {
        public int Id { get; set; }

        public int IssueId { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        [MaxLength(EntityConfigs.TextMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Option { get; set; }

        public bool IsBestOption { get; set; }
        [MaxLength(EntityConfigs.TextAreaMaxLength, ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateMaxStringLength")]

        public string Actors { get; set; }

        public string Resources { get; set; }

        public string IssueName { get; set; }
    }

    public class ResourceDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Resources;

namespace Application.DTOs
{
    public class AddResponsibleUserToStepDTO
    {
        public ExistingResponsibleUserDTO ExistingResponsibleUser { get; set; }

        public NewResponsibleUserDTO NewResponsibleUser { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        public int PlanId { get; set; }

        [Required(ErrorMessageResourceType = typeof(sharedResource), ErrorMessageResourceName = "validateRequired")]
        public string Step { get; set; }

    }
}

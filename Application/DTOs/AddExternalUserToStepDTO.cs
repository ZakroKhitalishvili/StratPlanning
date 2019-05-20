using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class AddExternalUserToStepDTO
    {
        public ExistingExternalUserDTO ExistingExternalUser { get; set; }

        public NewExternalUserDTO NewExternalUser { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public string Step { get; set; }

    }
}

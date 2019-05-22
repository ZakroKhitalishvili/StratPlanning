using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class AddResponsibleUserToStepDTO
    {
        public ExistingResponsibleUserDTO ExistingResponsibleUser { get; set; }

        public NewResponsibleUserDTO NewResponsibleUser { get; set; }

        [Required]
        public int PlanId { get; set; }

        [Required]
        public string Step { get; set; }

    }
}

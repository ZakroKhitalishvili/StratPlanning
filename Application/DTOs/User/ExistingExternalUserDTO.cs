using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class ExistingExternalUserDTO
    {
        [Required]
        public int? Id { get; set; }

        [Required]
        public string Step { get; set; }
    }
}

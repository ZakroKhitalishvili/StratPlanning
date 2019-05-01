using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class ExistingUserAddDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int PositionId { get; set; }

        [Required]
        public int PlanId { get; set; }
    }
}

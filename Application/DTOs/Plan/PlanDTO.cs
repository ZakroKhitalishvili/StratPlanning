using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class PlanDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(EntityConfigs.TextMaxLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(EntityConfigs.TextAreaMaxLength)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public bool? IsWithActionPlan { get; set; }

        public bool IsCompleted { get; set; }

    }
}

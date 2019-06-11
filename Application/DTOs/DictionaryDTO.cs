using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class DictionaryDTO
    {
        public int Id { get; set; }

        [MaxLength(EntityConfigs.TextMaxLength)]
        [Required]
        public string Title { get; set; }

        public bool HasPosition { get; set; }

        public bool HasCriterion { get; set; }

        public bool HasValue { get; set; }

        public bool HasStakeholderCategory { get; set; }

        public bool HasStakeholderCriteria { get; set; }
    }
}

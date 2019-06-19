using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class SettingDTO
    {
        public int Id { get; set; }

        [Required]
        public string Index { get; set; }

        [Required]
        [StringLength(EntityConfigs.LargeTextMaxLength)]
        public string Value { get; set; }
    }
}

using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class BlockEditDTO
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(EntityConfigs.TextMaxLength)]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        public string Instruction { get; set; }

        public string Step { get; set; }

    }
}

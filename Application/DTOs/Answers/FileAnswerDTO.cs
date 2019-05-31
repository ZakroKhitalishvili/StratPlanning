using Core.Constants;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class FileAnswerDTO
    {
        public int FileId { get; set; }

        [MaxLength(EntityConfigs.TextAreaMaxLength)]
        public string Name { get; set; }

        [MaxLength(EntityConfigs.TextAreaMaxLength)]
        public string Ext { get; set; }

        [MaxLength(EntityConfigs.TextAreaMaxLength)]
        public string Path { get; set; }
    }
}

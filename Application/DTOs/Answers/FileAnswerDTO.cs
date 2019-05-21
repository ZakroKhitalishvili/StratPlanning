using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class FileAnswerDTO
    {
        public int FileId { get; set; }
        public string Name { get; set; }
        public string Ext { get; set; }
        public string Path { get; set; }
    }
}

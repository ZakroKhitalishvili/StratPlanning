using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class BlockDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Instruction { get; set; }

        public string Step { get; set; }

        public int Order { get; set; }

    }
}

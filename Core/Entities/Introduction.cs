using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities
{
    public class Introduction
    {
        public int Id { get; set; }

        public int PlanId { get; set; }

        public int VideoId { get; set; }

        public string Step { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int? CreatedBy { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Plan Plan { get; set; }

        public virtual File Video { get; set; }

    }
}

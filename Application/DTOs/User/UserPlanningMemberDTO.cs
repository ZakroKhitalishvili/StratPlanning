using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class UserPlanningMemberDTO
    {
        public int Id { get; set; }

        public int UserToPlanId { get; set; }

        public string FullName { get; set; }

        public string Position { get; set; }

    }
}

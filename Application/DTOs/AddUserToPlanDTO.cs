using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class AddUserToPlanDTO
    {
        public ExistingUserAddDTO ExistingUser { get; set; }

        public PlanNewUserDTO NewUser { get; set; }
    }
}

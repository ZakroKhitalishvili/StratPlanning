using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public string Position { get; set; }
    }
}

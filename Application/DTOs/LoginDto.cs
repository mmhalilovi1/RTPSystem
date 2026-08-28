using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Application.DTOs
{
    public class LoginDto
    {       
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

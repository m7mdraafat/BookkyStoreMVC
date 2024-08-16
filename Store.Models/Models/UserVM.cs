﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Models.Models
{
    
    public class UserVM
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Company { get; set; }
        public string Role { get; set; }
        public DateTimeOffset? LockoutEnd { get; set; }
    }
}

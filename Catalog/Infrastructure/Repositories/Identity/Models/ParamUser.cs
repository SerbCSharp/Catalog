﻿using System.ComponentModel.DataAnnotations;

namespace Catalog.Infrastructure.Repositories.Identity.Models
{
    public class ParamUser
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

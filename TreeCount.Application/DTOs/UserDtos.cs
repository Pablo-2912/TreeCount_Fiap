using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Application.DTOs
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "As senhas não conferem.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }

    public class UserReadDto
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }

        public DateTime DataCriacao { get; set; }

        public DateTime DataAlteracao { get; set; }

        public DateTime? DeletedAt { get; set; }
    }

    public class UserDeleteDto
    {
        [Required]
        public string Id { get; set; }
    }

    public class UserUpdateDto
    {
        public string Id { get; set; } = "";

        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }
    }


}

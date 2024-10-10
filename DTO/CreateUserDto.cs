using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.DTO
{
    public class CreateUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        [Required(ErrorMessage = "PhoneNumber is required.")]
        public string PhoneNumber { get; set; } = string.Empty;
        /*[Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;*/
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        /*[Required]
        [Range(1, int.MaxValue, ErrorMessage = "Role must be great than 1")]
        public int RoleId { get; set; }*/
    }
}
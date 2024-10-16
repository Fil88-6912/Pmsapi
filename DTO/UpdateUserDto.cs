using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTO;

public class UpdateUserDto
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; } = string.Empty;
    [Required(ErrorMessage = "FirstName is required.")]
    public string FirstName { get; set; } = string.Empty;
    [Required(ErrorMessage = "LastName is required.")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "PhoneNumber is required.")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string Email { get; set; } = string.Empty;
    /*[Required]
    [Range(1, int.MaxValue, ErrorMessage = "Role must be great than 1.")]
    public int RoleId { get; set; }*/
}
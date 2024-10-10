using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTO
{
    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "End date is required")]
        public DateOnly EndDate { get; set; }
        [Required(ErrorMessage = "category is required")]
        [Range(1, int.MaxValue, ErrorMessage = "CartegoryId must be great oe equal than 1")]
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Manager is required")]
        public string ManagerId { get; set; } = string.Empty;
    }
}
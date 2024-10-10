using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace PmsApi.DTO
{
    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Task Title is required")]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        [Required(ErrorMessage = "StatusId Title is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Role must be great or equal than 1")]
        public int StatusId { get; set; }
        [Required(ErrorMessage = "PriorityId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Role must be great or equal than 1")]
        public int PriorityId { get; set; }
        [Required(ErrorMessage = "DueDate is required")]
        public DateOnly DueDate { get; set; }
        [Required(ErrorMessage = "StartDate is required")]
        public DateOnly StartDate { get; set; }
        [Required(ErrorMessage = "ProjectId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Role must be great or equal than 1")]
        public int ProjectId { get; set; }
        [Required(ErrorMessage = "AssignedUserId is required")]
        public string AssignedUserId { get; set; }  = string.Empty;
    }
}
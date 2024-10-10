using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmsApi.Models
{
    public partial class Task
    { 
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public Status? Status { get; set; } = null!;
        public Priority? Priority { get; set; } = null!;
        public DateOnly? DueDate { get; set; }
        public DateOnly? StartDate { get; set; }
        public int ProjectId { get; set; }
        public string? AssignedUserId { get; set; }
        public Project? Project { get; set; }
        public User? User { get; set; }
        public ICollection<TaskAttachement>? Attachement { get; set; } = new List<TaskAttachement>();
    }
}
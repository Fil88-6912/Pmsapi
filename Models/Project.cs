using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmsApi.Models
{
    public partial class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int CategoryId { get; set; }
        public string ManagerId { get; set; } = null!;
        public ProjectCategory? Category { get; set; }
        public User? Manager { get; set; }
        public ICollection<Task>? Tasks { get; set; } = new List<Task>();
    }
}
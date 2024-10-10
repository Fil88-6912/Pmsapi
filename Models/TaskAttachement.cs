using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PmsApi.Models
{
    public partial class TaskAttachement
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string FileData { get; set; } = null!;
        public DateOnly? CreationDate { get; set; }
        public int TaskId { get; set; }
        public Task Task { get; set; } = null!;
    }
}
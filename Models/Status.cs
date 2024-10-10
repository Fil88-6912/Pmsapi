using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.Models
{
    public partial class Status
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
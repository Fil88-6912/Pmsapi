using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace PmsApi.Models
{
    public partial class Role : IdentityRole
    {
       public ICollection<User>? Users { get; set; } = new List<User>(); 
    }
}
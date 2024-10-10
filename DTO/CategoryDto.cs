using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.DTO
{
    public record CategoryDto(
        int Id,
        string Name
    );
}
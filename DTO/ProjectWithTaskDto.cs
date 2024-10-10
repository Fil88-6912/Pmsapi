using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.DTO
{
    public record ProjectWithTaskDto(
        int Id,
        string Name,
        string? Description,
        DateOnly StartDate,
        DateOnly EndDate,
        int CategoryId,
        string ManagerId,
        ICollection<TaskDto> Tasks,
        UserOnlyDto Manager,
        CategoryDto Category
    );
}

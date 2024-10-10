using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.DTO
{
    public record TaskAllDto(
        int Id, 
        string Title,
        string? Description,
        int StatusId,
        int PriorityId,
        DateOnly DueDate,
        DateOnly StartDate,
        int ProjectId,
        string AssignedUserId,
        ProjectDto? Project,
        UserOnlyDto? User,
        ICollection<TaskAttachementDto>? Attachement
    );
}
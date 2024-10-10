using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.DTO
{
    public record TaskDto(
        int Id,
        string Title,
        string? Description,
        int StatusId,
        int PriorityId,
        DateOnly DueDate,
        DateOnly StartDate,
        int ProjectId,
        string AssignedUserId
    );
}
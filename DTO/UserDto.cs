using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PmsApi.DTO
{
    public record UserDto(
        string Id,
        string UserName,
        string FirstName,
        string LastName,
        string PhoneNumber,
        string Email,
        List<TaskDto> Tasks,
        List<ProjectDto> Projects
    );
}
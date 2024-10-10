using System.Runtime.InteropServices;
using AutoMapper;
using PmsApi.DTO;
using PmsApi.Models;
using Task = PmsApi.Models.Task;

namespace PmsApi.Utilities;

class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<CreateUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserOnlyDto>();
        CreateMap<User, UserDto>()
        .ForMember(d => d.Projects, opt => opt.MapFrom(src => src.Projects))
        .ForMember(d => d.Tasks, opt => opt.MapFrom(src => src.Tasks));
        CreateMap<Project, ProjectDto>();
        CreateMap<CreateProjectDto, Project>();
        CreateMap<Project, ProjectWithTaskDto>()
        .ForMember(d => d.Manager, opt => opt.MapFrom(src => src.Manager));
        CreateMap<Task, TaskDto>();
        CreateMap<CreateTaskDto, Task>();
        CreateMap<Task, TaskAllDto>();
        CreateMap<TaskAttachement, TaskAttachementDto>();
        CreateMap<TaskAttachement, AttachmentWithTaskDto>()
        .ForMember(d => d.Task, opt => opt.MapFrom(src => src.Task)); //si può anche non mettere in quanto si mappa lo stesso oggetto (task)
        CreateMap<Priority, PriorityDto>();
        CreateMap<CreatePriorityDto, Priority>();
        CreateMap<ProjectCategory, CategoryDto>();
        CreateMap<CreateCategoryDto, ProjectCategory>();
        CreateMap<Status, StatusDto>();
        CreateMap<CreateStatusDto, Status>();
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PmsApi.DTO;

namespace PmsApi.Services
{
    public interface IProjectService
    {
        public Task<ProjectWithTaskDto?> GetProjectTaskAsync(int projectId, string userId, bool isAdmin);
        public Task<IEnumerable<ProjectWithTaskDto>> GetProjectsAsync(string userId, bool isAdmin, string include = "");
        public Task<ProjectWithTaskDto?> GetProjectAsync(int projectId, string userId, bool isAdmin, string include = "");
        public Task<ProjectDto> CreateProjectAsync(string userId, bool isAdmin, CreateProjectDto projectDto);
        public Task<bool?> UpdateProjectAsync(int projectId, string userId, bool isAdmin, CreateProjectDto projectDto);
        public Task<bool?> DeleteProjectAsync(int projectId, string userId, bool isAdmin);
    }
}
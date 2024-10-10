using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PmsApi.DTO;
using PmsApi.Utilities;
using PmsApi.DataContexts;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsApi.Models;
using MySqlConnector;

namespace PmsApi.Services
{
    public class ProjectService : ControllerBase, IProjectService
    {
        private readonly PmsContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextHelper _userContextHelper;

        public ProjectService(PmsContext context, IMapper mapper, IUserContextHelper userContextHelper){
            _context = context;
            _mapper = mapper;
            _userContextHelper = userContextHelper;
        }

        public async Task<ProjectDto> CreateProjectAsync(string userId, bool isAdmin, CreateProjectDto projectDto)
        {
            if(!isAdmin){
                Console.WriteLine($"Id user: {userId}");
                projectDto.ManagerId = userId;
            }

            var project = _mapper.Map<Project>(projectDto);

            _context.Projects.Add(project);

            await _context.SaveChangesAsync();
            var newProjectDto = _mapper.Map<ProjectDto>(project);

            return newProjectDto;
        }

        public async Task<ProjectWithTaskDto?> GetProjectAsync(int projectId, string userId, bool isAdmin, string include = "")
        {
            var projectsQuery = QueryHelper.ApplyIncludesProject(_context.Projects.AsQueryable(), include);

            if(!isAdmin){
                Console.WriteLine($"Id user: {userId}");
                projectsQuery = projectsQuery.Where(p => p.ManagerId == userId);
            }

            var project = await projectsQuery.FirstOrDefaultAsync(p => p.Id == projectId);

            if(project == null){
                return null;
            }

            var projectDto = _mapper.Map<ProjectWithTaskDto>(project);
            return projectDto;
        }

        public async Task<IEnumerable<ProjectWithTaskDto>> GetProjectsAsync(string userId, bool isAdmin, string include = "")
        {
            var projectsQuery = QueryHelper.ApplyIncludesProject(_context.Projects.AsQueryable(), include);
            
            if(!isAdmin){
                Console.WriteLine($"Id user: {userId}");
                projectsQuery = projectsQuery.Where(p => p.ManagerId == userId);
            }

            var projects = await projectsQuery.ToListAsync();
            var projectDto = _mapper.Map<IEnumerable<ProjectWithTaskDto>>(projects);

            return projectDto;
        }

        public async Task<ProjectWithTaskDto?> GetProjectTaskAsync(int projectId, string userId, bool isAdmin)
        {
            var projectQuery = _context.Projects.AsQueryable();
            //.Include(p => p.Tasks).Where(p => p.Id == projectId);

            if(!isAdmin){
                projectQuery = projectQuery.Where(p => p.ManagerId == userId);
            }

            var project = await projectQuery.Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == projectId);

            if(project is null){
                return null;
            }

            //var projectDto = new[] { _mapper.Map<ProjectWithTaskDto>(project) };
            var projectDto = _mapper.Map<ProjectWithTaskDto>(project);
            return projectDto;
        } 

        public async Task<bool?> UpdateProjectAsync(int projectId, string userId, bool isAdmin, CreateProjectDto projectDto){
            var project = await _context.Projects.FindAsync(projectId);

            if (project is null || (!isAdmin && project.ManagerId != userId)){
                return null;
            }

            _mapper.Map(projectDto, project);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool?> DeleteProjectAsync(int projectId, string userId, bool isAdmin){
            var project = await _context.Projects.FindAsync(projectId);

            if (project is null || (!isAdmin && project.ManagerId != userId)){
                return null;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
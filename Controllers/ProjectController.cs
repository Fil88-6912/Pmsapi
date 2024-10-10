using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using PmsApi.DataContexts;
using PmsApi.Models;
using PmsApi.DTO;
using PmsApi.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using PmsApi.Services;

namespace PmsApi.Controllers
{
    [ApiController]
    [Route("api/projects"), Authorize]

    public class ProjectController : ControllerBase
    {
        private readonly PmsContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextHelper _userContextHelper;
        private readonly IProjectService _projectService;

        public ProjectController(PmsContext context, IMapper mapper, IUserContextHelper userContextHelper, IProjectService projectService){
            _context = context;
            _mapper = mapper;
            _userContextHelper = userContextHelper;
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProjectWithTaskDto>>> GetProjects([FromQuery] string include = "")
        {  //public List<User> GetUsers(){
           //var projectsQuery = QueryHelper.ApplyIncludesProject(_context.Projects.AsQueryable(), include);

            /*var projectsQuery = _context.Projects.AsQueryable();

            if(include.Contains("tasks", StringComparison.OrdinalIgnoreCase)){
                projectsQuery = projectsQuery.Include(p => p.Tasks);
            }

            if(include.Contains("manager", StringComparison.OrdinalIgnoreCase)){
                projectsQuery = projectsQuery.Include(p => p.Manager);
            }

            if(include.Contains("category", StringComparison.OrdinalIgnoreCase)){
                projectsQuery = projectsQuery.Include(p => p.Category);
            }*/

            /*if(!_userContextHelper.IsAdmin()){
                Console.WriteLine($"Id user: { _userContextHelper.GetUserId()}");
                projectsQuery = projectsQuery.Where(p => p.ManagerId == _userContextHelper.GetUserId());
            }
            var projects = await projectsQuery.ToListAsync();
            var projectDto = _mapper.Map<IEnumerable<ProjectWithTaskDto>>(projects);*/

            /*string userId;
            bool isAdmin;*/
            GetUserCredentials(out string userId, out bool isAdmin);

            var projects = await _projectService.GetProjectsAsync(userId, isAdmin, include);

            return Ok(projects);
        }

        private void GetUserCredentials(out string userId, out bool isAdmin)
        {
            userId = _userContextHelper.GetUserId();
            isAdmin = _userContextHelper.IsAdmin();
        }

        [HttpGet("{id}")] 
        public async Task<ActionResult<ProjectWithTaskDto>> GetProject(int id, [FromQuery] string include = ""){ 
            /*var projectsQuery = QueryHelper.ApplyIncludesProject(_context.Projects.AsQueryable(), include);

            Project? project = await projectsQuery.FirstOrDefaultAsync(p => p.Id == id);

            if(project == null){
                return NotFound();
            }
            var projectDto = _mapper.Map<ProjectWithTaskDto>(project);*/

            GetUserCredentials(out string userId, out bool isAdmin);

            var projectDto = await _projectService.GetProjectAsync(id, userId, isAdmin, include);

            if(projectDto is null){
                return NotFound($"Project with ID {id} not found.");
            }
            return Ok(projectDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateProject([FromBody] CreateProjectDto projectDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            /*if(!_userContextHelper.IsAdmin()){
                Console.WriteLine($"Id user: { _userContextHelper.GetUserId()}");
                projectDto.ManagerId = _userContextHelper.GetUserId();
            }

            var project = _mapper.Map<Project>(projectDto);

            _context.Projects.Add(project);*/

            GetUserCredentials(out string userId, out bool isAdmin);

            try
            {
                /*await _context.SaveChangesAsync();
                var newProjectDto = _mapper.Map<ProjectDto>(project);*/
                var newProjectDto = await _projectService.CreateProjectAsync(userId, isAdmin, projectDto);

                return CreatedAtAction(nameof(GetProject), new { id = newProjectDto.Id }, newProjectDto);
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Project name already taken.");
            }
            catch(Exception ex){
                return StatusCode(500, $"An error as occurred {ex.Message}");
            }
        }

        [HttpPut("{id:int}")]  //Patch per modificare solo alcuni date della tabella, Put se si modificano tutti
        public async Task<ActionResult> UpdateProject([FromRoute] int id, [FromBody] CreateProjectDto projectDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            } 

            /*Project? project = await _context.Projects.FindAsync(id);

            if(project is null){
                return NotFound($"Project with ID {id} not found.");
            }

            if (!_userContextHelper.IsAdmin() && project.ManagerId != _userContextHelper.GetUserId()){
                return Unauthorized();
            }

            _mapper.Map(projectDto, project);*/

            GetUserCredentials(out string userId, out bool isAdmin);

            try
            {
                //await _context.SaveChangesAsync();
                var result = await _projectService.UpdateProjectAsync(id, userId, isAdmin, projectDto);

                if(result is null){
                    return NotFound();
                }
                
                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Project name already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProject(int id) { 
            /*Project? project = await _context.Projects.FindAsync(id);

            if(project is null){
                return NotFound($"Project with ID {id} not exist.");
            }

            if (!_userContextHelper.IsAdmin() && project.ManagerId != _userContextHelper.GetUserId()){
                return Unauthorized();
            }*/

            GetUserCredentials(out string userId, out bool isAdmin);

            try
            {
                //_context.Projects.Remove(project);
                //await _context.SaveChangesAsync();

                var result = await _projectService.DeleteProjectAsync(id, userId, isAdmin);

                if(result is null){
                    return NotFound();
                }
                
                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException){
                return BadRequest("Project has other records, please delete assigned tasks.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }

        }

        /*[HttpGet("{id}/tasks")]
        public async Task<ActionResult<List<ProjectWithTaskDto>>> GetProjectsTasks(int id){
            var project = await _context.Projects.Include(p => p.Tasks).Where(p => p.Id == id).ToListAsync();

            if(project is null || project.Count == 0){
                return NotFound();
            }
            var projectDto = _mapper.Map<IEnumerable<ProjectWithTaskDto>>(project);
            return Ok(projectDto);
        }*/
        [HttpGet("{id}/tasks")]
        public async Task<ActionResult<IEnumerable<ProjectWithTaskDto>>> GetProjectsTasks(int id){
            /*var projectQuery = _context.Projects.Include(x => x.Tasks).Where(x => x.Id == id);

            if(!_userContextHelper.IsAdmin()){
                projectQuery = projectQuery.Where(p => p.ManagerId == _userContextHelper.GetUserId());
            }

            var project = await projectQuery.ToListAsync();

            if(project is null || project.Count == 0){
                return NotFound();
            }

            var projectDto = _mapper.Map<IEnumerable<ProjectWithTaskDto>>(project);*/

            GetUserCredentials(out string userId, out bool isAdmin);

            var projectTasks = await _projectService.GetProjectTaskAsync(id, userId, isAdmin);

            if(projectTasks is null){
                return NotFound();
            }
            return Ok(projectTasks);
        }
        
    }
}
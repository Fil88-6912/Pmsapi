using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.DTO;
using PmsApi.Models;
using PmsApi.Utilities;
using AutoMapper;
using MySqlConnector;
using Microsoft.AspNetCore.Authorization;
using Task = PmsApi.Models.Task;

namespace PmsApi.Controllers
{
    [ApiController]
    [Route("api/tasks"), Authorize]
    public class TaskController : ControllerBase
    {
        private readonly PmsContext _context;
        private readonly IMapper _mapper;
        private readonly IUserContextHelper _userContextHelper;

        public TaskController(PmsContext context, IMapper mapper, IUserContextHelper userContextHelper){
            _context = context;
            _mapper = mapper;
            _userContextHelper = userContextHelper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAllDto>>> GetTasks([FromQuery] string include = ""){
            var tasksQuery = QueryHelper.ApplyIncludes(_context.Tasks.AsQueryable(), include);

            if(!_userContextHelper.IsAdmin()){
                Console.WriteLine($"Id user: { _userContextHelper.GetUserId()}");
                tasksQuery.Where(t => t.AssignedUserId == _userContextHelper.GetUserId());
            }

            var tasks = await tasksQuery.ToListAsync();
            var taskDto = _mapper.Map<IEnumerable<TaskAllDto>>(tasks);

            return Ok(taskDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskAllDto>> GetTask(int id, [FromQuery] string include = ""){
            var tasksQuery = QueryHelper.ApplyIncludes(_context.Tasks.AsQueryable(), include);

            if(!_userContextHelper.IsAdmin()){
                Console.WriteLine($"Id user: { _userContextHelper.GetUserId()}");
                tasksQuery.Where(t => t.AssignedUserId == _userContextHelper.GetUserId());
            }

            var task = await tasksQuery.FirstOrDefaultAsync(t => t.Id == id);

            if(task == null) return NotFound();

            var taskDto = _mapper.Map<TaskAllDto>(task);
            return Ok(taskDto);
        }

        [HttpPost,  Authorize(Roles = "Admin, Editor")]
        public async Task<ActionResult> CreateTask([FromBody] CreateTaskDto taskDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            if(!_userContextHelper.IsAdmin()){
                taskDto.AssignedUserId = _userContextHelper.GetUserId();
            }

            var task = _mapper.Map<Task>(taskDto);
            task.StartDate = DateOnly.FromDateTime(DateTime.Now);

            _context.Tasks.Add(task);

            try
            {
                await _context.SaveChangesAsync();
                var newTaskDto = _mapper.Map<TaskDto>(task);

                return CreatedAtAction(nameof(GetTask), new { id = task.Id }, newTaskDto);
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Task title already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> UpdateTask([FromRoute] int id, [FromBody] CreateTaskDto taskDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            } 

            var task = await _context.Tasks.FindAsync(id);

            if(task is null){
                return NotFound($"Task with ID {id} not found.");
            }

            if(!_userContextHelper.IsAdmin() && task.AssignedUserId != _userContextHelper.GetUserId()){
                return Unauthorized();
            }

            _mapper.Map(taskDto, task);

            try
            {
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Task title already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteTask(int id) { 
            Task? task = await _context.Tasks.FindAsync(id);

            if(task is null){
                return NotFound($"Task with ID {id} not exist.");
            }

            if(!_userContextHelper.IsAdmin() && task.AssignedUserId != _userContextHelper.GetUserId()){
                return Unauthorized();
            }

            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException){
                return BadRequest("Task has other records, please delete assigned attachments.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpGet("{id}/attachements")]
        public async Task<ActionResult<IEnumerable<AttachmentWithTaskDto>>> GetTasksAttachments(int id){
            var task = await _context.Tasks.Include(x => x.Attachement).Where(x => x.Id == id).FirstOrDefaultAsync();

            if(task is null){
                return NotFound();
            }

            if (!_userContextHelper.IsAdmin() && task.AssignedUserId != _userContextHelper.GetUserId()){
                return Unauthorized();
            }

            var taskAttachments = task.Attachement;
            var taskAttachmentsDto = _mapper.Map<IEnumerable<AttachmentWithTaskDto>>(taskAttachments);
            return Ok(taskAttachmentsDto);
        }
    }
}
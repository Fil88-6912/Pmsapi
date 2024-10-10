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
using Task = PmsApi.Models.Task;
using Microsoft.AspNetCore.Authorization;

namespace PmsApi.Controllers
{
    [ApiController]
    [Route("api/priorities"), Authorize(Policy = "IsAdmin")]
    public class PriorityController  : ControllerBase
    {
        private readonly PmsContext _context;
        private readonly IMapper _mapper;

        public PriorityController(PmsContext context, IMapper mapper){
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PriorityDto>>> GetPriorities(){ 
            var priorities = await _context.Priorities.ToListAsync();
            var priorityDto = _mapper.Map<IEnumerable<PriorityDto>>(priorities);

            return Ok(priorityDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PriorityDto>> GetPriority(int id){
            Priority? priority = await _context.Priorities.FirstOrDefaultAsync(x => x.Id == id);

            if(priority == null){
                return NotFound();
            }
            var priorityDto = _mapper.Map<PriorityDto>(priority);
            return Ok(priorityDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreatePriority([FromBody] CreatePriorityDto priorityDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var priority = _mapper.Map<Priority>(priorityDto);

            _context.Priorities.Add(priority);

            try
            {
                await _context.SaveChangesAsync();
                var newPriorityDto = _mapper.Map<PriorityDto>(priority);

                return CreatedAtAction(nameof(GetPriority), new { id = priority.Id }, newPriorityDto);
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Priority name already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> UpdatePriority([FromRoute] int id, [FromBody] CreatePriorityDto priorityDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            } 

            Priority? priority = await _context.Priorities.FindAsync(id);

            if(priority is null){
                return NotFound($"Priority with ID {id} not found.");
            }

            _mapper.Map(priorityDto, priority);

            try
            {
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Priority name already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeletePriority(int id) { 
            Priority? priority = await _context.Priorities.FindAsync(id);

            if(priority is null){
                return NotFound($"Priority with ID {id} not exist.");
            }

            try
            {
                _context.Priorities.Remove(priority);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }
    }
}
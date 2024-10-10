using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PmsApi.DataContexts;
using PmsApi.DTO;
using PmsApi.Models;
using AutoMapper;
using MySqlConnector;
using Microsoft.AspNetCore.Authorization;

namespace PmsApi.Controllers
{
    [ApiController]
    [Route("api/statuses"), Authorize(Policy = "IsAdmin")]
    public class StatusController : ControllerBase
    {
        private readonly PmsContext _context;
        private readonly IMapper _mapper;

        public StatusController(PmsContext context, IMapper mapper){
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StatusDto>>> GetStatuses(){ 
            var statuses = await _context.Statuses.ToListAsync();
            var statusDto = _mapper.Map<IEnumerable<StatusDto>>(statuses);

            return Ok(statusDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StatusDto>> GetStatus(int id){
            Status? status = await _context.Statuses.FirstOrDefaultAsync(x => x.Id == id);

            if(status == null){
                return NotFound();
            }
            var statusDto = _mapper.Map<StatusDto>(status);
            return Ok(statusDto);
        }

        [HttpPost]
        public async Task<ActionResult> Createstatus([FromBody] CreateStatusDto statusDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var status = _mapper.Map<Status>(statusDto);

            _context.Statuses.Add(status);

            try
            {
                await _context.SaveChangesAsync();
                var newStatusDto = _mapper.Map<StatusDto>(status);

                return CreatedAtAction(nameof(GetStatus), new { id = status.Id }, newStatusDto);
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Status name already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> UpdateStatus([FromRoute] int id, [FromBody] CreateStatusDto statusDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            } 

            Status? status = await _context.Statuses.FindAsync(id);

            if(status is null){
                return NotFound($"Status with ID {id} not found.");
            }

            _mapper.Map(statusDto, status);

            try
            {
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Status name already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteStatus(int id) { 
            Status? status = await _context.Statuses.FindAsync(id);

            if(status is null){
                return NotFound($"Status with ID {id} not exist.");
            }

            try
            {
                _context.Statuses.Remove(status);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }
    }
}
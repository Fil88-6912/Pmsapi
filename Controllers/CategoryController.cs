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
    [Route("api/categories"), Authorize(Policy = "IsAdmin")]
    public class CategoryController : ControllerBase
    {
        private readonly PmsContext _context;
        private readonly IMapper _mapper;

        public CategoryController(PmsContext context, IMapper mapper){
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories(){ 
            var categories = await _context.ProjectCategories.ToListAsync();
            var categoryDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return Ok(categoryDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id){
            ProjectCategory? category = await _context.ProjectCategories.FirstOrDefaultAsync(x => x.Id == id);

            if(category == null){
                return NotFound();
            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCategory([FromBody] CreateCategoryDto categoryDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<ProjectCategory>(categoryDto);

            _context.ProjectCategories.Add(category);

            try
            {
                await _context.SaveChangesAsync();
                var newCategoryDto = _mapper.Map<CategoryDto>(category);

                return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, newCategoryDto);
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Category name already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> UpdateCategory([FromRoute] int id, [FromBody] CreateCategoryDto categoryDto){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            } 

            ProjectCategory? category = await _context.ProjectCategories.FindAsync(id);

            if(category is null){
                return NotFound($"Category with ID {id} not found.");
            }

            _mapper.Map(categoryDto, category);

            try
            {
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("Category name already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteCategory(int id) { 
            ProjectCategory? category = await _context.ProjectCategories.FindAsync(id);

            if(category is null){
                return NotFound($"Category with ID {id} not exist.");
            }

            try
            {
                _context.ProjectCategories.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }
    }
}
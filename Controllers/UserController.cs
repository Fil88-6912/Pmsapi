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
using AutoMapper;
using Microsoft.VisualBasic;
using PmsApi.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Cors;

namespace PmsApi.Controllers
{
    [ApiController]
    [Route("api/users"), Authorize(Policy = "IsAdmin")] //[Route("api/[controller]")]

    public class UserController : ControllerBase
    {
        private readonly PmsContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserController(PmsContext context, IMapper mapper, UserManager<User> userManager){
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet, AllowAnonymous]  //AllowAnonymous: anche gli utenti non loggati potranno vedere la lista
        public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] string include = ""){  //public List<User> GetUsers(){
            var usersQuery = QueryHelper.ApplyIncludesUser(_context.Users.AsQueryable(), include);

            var users = await usersQuery.ToListAsync();
            var userDto = _mapper.Map<IEnumerable<UserDto>>(users);

            return Ok(userDto);
        }

        [HttpGet("{id}"), AllowAnonymous]  //[HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetUser(string id){  //public async Task<IActionResult> GetUser(int id){
            //User? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            var isAdmin = HttpContext.User.IsInRole("Admin");
            Console.WriteLine($"Il ruolo è Admin?: {isAdmin}");
            User? user = await _userManager.FindByIdAsync(id);

            if(user == null){
                return NotFound();
            }
            return Ok(user);
        }

        //[HttpPost, Authorize(Roles = "Admin, Editor")]
        [HttpPost, AllowAnonymous]
        public async Task<ActionResult> CreateUser([FromBody] CreateUserDto userDto){  //public async Task<ActionResult> CreateUser([FromBody] User user){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            /*var user = new User
            {
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Password = userDto.Password,
                Email = userDto.Email,
                RoleId = userDto.RoleId
            };*/

            var user = _mapper.Map<User>(userDto);

            //_context.Users.Add(user);

            try
            {
                var result = await _userManager.CreateAsync(user, "12345Abcd&");
                if(!result.Succeeded){
                    return StatusCode(500, "An error as occurred creating user.");
                }
                //await _context.SaveChangesAsync();
                var newUserDto = _mapper.Map<UserDto>(user);

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, newUserDto);
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("UserName or Password already taken.");
            }
            catch(Exception ex){
                return StatusCode(500, $"An error as occurred: {ex.Message}");
            }
        }

        [HttpPatch("{id}"), AllowAnonymous]
        public async Task<ActionResult> UpdateUser([FromRoute] string id, [FromBody] UpdateUserDto userDto){  //public async Task<ActionResult> CreateUser([FromBody] User user){
            if(!ModelState.IsValid){
                return BadRequest(ModelState);
            }

            //User? user = await _context.Users.FindAsync(id);
            User? user = await _userManager.FindByIdAsync(id);

            if(user is null){
                return NotFound($"User with ID {id} not found.");
            }

            /*user.UserName = userDto.UserName;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Email = userDto.Email;*/
            //user.RoleId = userDto.RoleId;

            _mapper.Map(userDto, user);
            
            try
            {
                //await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
                
                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException && mySqlException.Number == 1062){
                return BadRequest("UserName or Password already taken.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }
        }

        [HttpDelete("{id}"), AllowAnonymous]
        public async Task<ActionResult> DeleteUser(string id) { 
            //User? user = await _context.Users.FindAsync(id);
            User? user = await _userManager.FindByIdAsync(id);

            if(user is null){
                return NotFound($"User with ID {id} not exist.");
            }

            try
            {
                /*_context.Users.Remove(user);
                await _context.SaveChangesAsync();*/
                await _userManager.DeleteAsync(user);

                return NoContent();
            }
            catch(DbUpdateException e) when (e.InnerException is MySqlException mySqlException){
                return BadRequest("User has other records, please delete assigned tasks.");
            }
            catch(Exception){
                return StatusCode(500, "An error as occurred.");
            }

        }
    }
}
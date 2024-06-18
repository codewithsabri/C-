using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using userProject.Data;
using userProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Cryptography;
using System.Text;
using userProject.Models; // Add this line to import the 'User' class

namespace userProject.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;

        public UserController(UserContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserViewModel>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserViewModel>> CreateUser(UserViewModel model)
        {
            // Check if the username already exists
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            // Check if the email already exists
            var existingEmail = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (existingEmail != null)
            {
                return BadRequest("Email already exists");
            }

            // Hash the password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

            var user = new UserViewModel
            {
                Username = model.Username,
                Password = passwordHash,
                Email = model.Email, // Make sure to set the Email field
                FirstName = model.FirstName, // Make sure to set the FirstName field
                LastName = model.LastName // Make sure to set the LastName field
            };

            // Save the user to the database
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<UserViewModel>> PostUser(UserViewModel model)
        {
            // Check if the username already exists
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            // Check if the email already exists
            var existingEmail = await _context.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            if (existingEmail != null)
            {
                return BadRequest("Email already exists");
            }

            // Hash the password
            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.Password));

            var user = new UserViewModel
            {
                Username = model.Username,
                Password = BitConverter.ToString(passwordHash).Replace("-", "").ToLower(),
                Email = model.Email, // Make sure to set the Email field
                FirstName = model.FirstName, // Make sure to set the FirstName field
                LastName = model.LastName // Make sure to set the LastName field
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}

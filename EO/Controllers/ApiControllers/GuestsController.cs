using EO.Models;
using EO.WebContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace EO.Controllers.ApiControllers
{
    [Route("api/guests")]
    [ApiController]
    public class GuestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GuestsController(AppDbContext context)
        {
            _context = context;
        }

 
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var guests = await _context.Guests.ToListAsync();

            return Ok(new
            {
                success = true,
                data = guests
            });
        }

   
        [HttpPost]
        public async Task<IActionResult> Create(CreateGuestDto dto)
        {
            var guest = new Guests
            {
                Name = dto.Name,
                Designation = dto.Designation,
                Avatar = dto.Avatar
            };

            _context.Guests.Add(guest);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, guest });
        }

 
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateGuestDto dto)
        {
            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
                return NotFound();

            guest.Name = dto.Name;
            guest.Designation = dto.Designation;
            guest.Avatar = dto.Avatar;

            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var guest = await _context.Guests.FindAsync(id);

            if (guest == null)
                return NotFound();

            _context.Guests.Remove(guest);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}

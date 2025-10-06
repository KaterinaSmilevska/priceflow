using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PriceFlowApp.Models;

namespace PriceFlowApp.Controllers
{
    [Route("/api[controller]")]
    [ApiController]
    public class DnevenPrometController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DnevenPrometController(AppDbContext context)
        {
            _context = context;
        }

        [Route("/DnevenPromet")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DnevenPromet>>> GetAllDnevenPromet()
        {
            return await _context.DnevenPromets.ToListAsync();
        }

        [Route("/DnevenPromet/{id}")]
        [HttpGet]
        public async Task<ActionResult<DnevenPromet>> GetById(int id)
        {
            var item = await _context.DnevenPromets.FindAsync(id);
            if (item == null) return NotFound();
            return item;
        }

        [Route("/Izdavachi")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Izdavach>>> GetAllIzdavach()
        {
            return await _context.Izdavaches.ToListAsync();
        }
    }
}

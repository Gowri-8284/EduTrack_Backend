using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EduTrackAcademics.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EduTrackAcademics.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProgramsController : ControllerBase
    {
        private readonly DbContext _context;

        public ProgramsController(DbContext context)
        {
            _context = context;
        }

        // GET: api/Programs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Programs>>> GetPrograms()
        {
            return await _context.Set<Programs>().ToListAsync();
        }

        // GET: api/Programs/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Programs>> GetProgram(string id)
        {
            var program = await _context.Set<Programs>().FindAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            return program;
        }

        // POST: api/Programs
        [HttpPost]
        public async Task<ActionResult<Programs>> CreateProgram(Programs program)
        {
            _context.Set<Programs>().Add(program);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProgram), new { id = program.ProgramId }, program);
        }

        // PUT: api/Programs/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgram(string id, Programs program)
        {
            if (id != program.ProgramId)
            {
                return BadRequest();
            }

            _context.Entry(program).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Programs/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(string id)
        {
            var program = await _context.Set<Programs>().FindAsync(id);
            if (program == null)
            {
                return NotFound();
            }

            _context.Set<Programs>().Remove(program);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProgramExists(string id)
        {
            return _context.Set<Programs>().Any(e => e.ProgramId == id);
        }
    }
}

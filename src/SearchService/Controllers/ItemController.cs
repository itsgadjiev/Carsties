using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.QuickInfo;
using Microsoft.EntityFrameworkCore;
using SearchService.Data;
using SearchService.Entities;
using SearchService.RequestHelpers;

namespace SearchService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly SearchDbContext _context;

        public ItemController(SearchDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Item>>> GetItems([FromQuery] SearchParams? searchParams)
        {
            var query = _context.Items.AsQueryable();


            if (!string.IsNullOrEmpty(searchParams.SearchTerm))
            {
                query = searchParams.OrderBy switch
                {
                    "make" => query = query.OrderBy(x => x.Make),
                    "new" => query = query.OrderByDescending(x => x.CreatedAt),
                    _ => query = query.OrderBy(x => x.AuctionEnd)
                };

                query = searchParams.FilterBy switch
                {
                    "finished" => query = query.Where(x => x.AuctionEnd < DateTime.UtcNow),
                    "endingSoon" => query = query.Where(x => x.AuctionEnd < DateTime.UtcNow.AddHours(6)
                                            && x.AuctionEnd > DateTime.UtcNow),
                    _ => query = query.Where(x => x.AuctionEnd > DateTime.UtcNow)
                };
            }

            if (!string.IsNullOrEmpty(searchParams.Seller))
            {
                query = query.Where(x => x.Seller == searchParams.Seller);
            }

            if (!string.IsNullOrEmpty(searchParams.Winner))
            {
                query = query.Where(x => x.Seller == searchParams.Winner);
            }


            return await query.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(Guid id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(Guid id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemExists(id))
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


        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.Id }, item);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(Guid id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ItemExists(Guid id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}

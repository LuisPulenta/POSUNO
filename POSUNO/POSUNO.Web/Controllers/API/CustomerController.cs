using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using POSUNO.Web.Data;
using POSUNO.Web.Data.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POSUNO.Web.Controllers.API
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly DataContext _context;

        public CustomersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomer(int id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest();
            }

            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == customer.User.Email);
            if (user == null)
            {
                return BadRequest("Usuario no existe.");
            }

            customer.User = user;
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == customer.User.Email);
            if (user == null)
            {
                return BadRequest("Usuario no existe.");
            }

            Customer oldCustomer = await _context.Customers.FirstOrDefaultAsync(c => c.Email == customer.Email);
            if (oldCustomer != null)
            {
                return BadRequest("Ya hay un cliente registrado con ese email.");
            }

            customer.User = user;
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, customer);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            Customer customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
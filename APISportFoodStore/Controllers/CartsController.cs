using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISportFoodStore.Models;

namespace APISportFoodStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly FoodStoreDbContext _context;

        public CartsController(FoodStoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Carts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCarts()
        {
            return await _context.Carts.ToListAsync();
        }

        // GET: api/Carts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cart>> GetCart(int? id)
        {
            var cart = await _context.Carts.FindAsync(id);

            if (cart == null)
            {
                return NotFound();
            }

            return cart;
        }

        // PUT: api/Carts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCart(int? id, Cart cart)
        {
            if (id != cart.IdCart)
            {
                return BadRequest();
            }

            _context.Entry(cart).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CartExists(id))
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

        // POST: api/Carts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Cart>> PostCart([FromBody] CartDto dto)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.IdProduct == dto.ProductId && !p.Deleted);

            if (product == null)
                return BadRequest("Товар не найден.");

            if (dto.Quantity < 1)
                return BadRequest("Количество должно быть не менее 1.");

            var existingCartItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == dto.UserId && c.ProductId == dto.ProductId);

            if (existingCartItem != null)
            {
                int newTotal = existingCartItem.Quantity + dto.Quantity;
                if (newTotal > product.Quantity)
                    return BadRequest($"Нельзя добавить больше {product.Quantity} единиц.");

                existingCartItem.Quantity = newTotal;
                existingCartItem.Price = product.Price * newTotal;
                _context.Entry(existingCartItem).State = EntityState.Modified;
            }
            else
            {
                if (dto.Quantity > product.Quantity)
                    return BadRequest($"Нельзя добавить больше {product.Quantity} единиц.");

                var cart = new Cart
                {
                    UserId = dto.UserId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    Price = product.Price * dto.Quantity
                };
                _context.Carts.Add(cart);
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        public class CartDto
        {
            public int UserId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }
        // DELETE: api/Carts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCart(int? id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Carts/User/5
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Cart>>> GetCartsByUser(int userId)
        {
            var carts = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            return carts;
        }


        private bool CartExists(int? id)
        {
            return _context.Carts.Any(e => e.IdCart == id);
        }

        // DELETE: api/Carts/User/5
        [HttpDelete("User/{userId}")]
        public async Task<IActionResult> DeleteCartByUser(int userId)
        {
            var userCartItems = await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!userCartItems.Any())
                return NotFound("Корзина пользователя пуста.");

            _context.Carts.RemoveRange(userCartItems);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}

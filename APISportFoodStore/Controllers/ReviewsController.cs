using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APISportFoodStore.Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly FoodStoreDbContext _context;

        public ReviewsController(FoodStoreDbContext context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviews()
        {
            return await _context.Reviews.ToListAsync();
        }

        // GET: api/Reviews/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            return review;
        }

        // PUT: api/Reviews/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReview(int id, Review review)
        {
            if (id != review.IdReview)
            {
                return BadRequest();
            }

            _context.Entry(review).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReviewExists(id))
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

        // POST: api/Reviews
        [HttpPost]
        public async Task<ActionResult<Review>> PostReview(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReview", new { id = review.IdReview }, review);
        }

        // DELETE: api/Reviews/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            review.Deleted = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.IdReview == id);
        }

        // GET: api/Reviews/product/5
        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetProductReviews(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        // POST: api/Reviews/WithVerification
        // POST: api/Reviews/WithVerification
        [HttpPost("WithVerification")]
        public async Task<ActionResult<Review>> PostReviewWithVerification(CreateReviewDto reviewDto)
        {
            try
            {
                // Проверяем, был ли куплен товар и заказ завершен
                var hasCompletedOrder = await _context.OrderDetails
                    .Include(od => od.Order)
                    .AnyAsync(od => od.ProductId == reviewDto.ProductId
                                    && od.Order.UserId == reviewDto.UserId
                                    && od.Order.OrderStatusId == 7); // только статус "Завершен"

                if (!hasCompletedOrder)
                {
                    return BadRequest(new { message = "Вы можете оставить отзыв только после завершения заказа с этим товаром." });
                }

                // Проверяем, не оставлял ли пользователь уже отзыв
                var existingReview = await _context.Reviews
                    .AnyAsync(r => r.UserId == reviewDto.UserId && r.ProductId == reviewDto.ProductId);

                if (existingReview)
                {
                    return BadRequest(new { message = "Вы уже оставляли отзыв на этот товар" });
                }

                // Создаем новый отзыв
                var review = new Review
                {
                    UserId = reviewDto.UserId,
                    ProductId = reviewDto.ProductId,
                    Rating = reviewDto.Rating,
                    Comment = reviewDto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                // Загружаем связанные данные для возврата
                await _context.Entry(review).Reference(r => r.User).LoadAsync();
                await _context.Entry(review).Reference(r => r.Product).LoadAsync();

                return CreatedAtAction("GetReview", new { id = review.IdReview }, review);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostReviewWithVerification: {ex.Message}");
                return StatusCode(500, new { message = "Внутренняя ошибка сервера" });
            }
        }

        // GET: api/Reviews/CanReview/{userId}/{productId}
        [HttpGet("CanReview/{userId}/{productId}")]
        public async Task<ActionResult<bool>> CanUserReview(int userId, int productId)
        {
            try
            {
                var hasCompletedOrder = await _context.Orders
                    .Include(o => o.OrderDetails)
                    .AnyAsync(o => o.UserId == userId
                                  && o.OrderStatusId == 7 // статус "Завершен"
                                  && o.OrderDetails.Any(od => od.ProductId == productId));

                var hasReviewed = await _context.Reviews
                    .AnyAsync(r => r.UserId == userId && r.ProductId == productId);

                return hasCompletedOrder && !hasReviewed;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CanUserReview: {ex.Message}");
                return false;
            }
        }


        // GET: api/Reviews/User/{userId}
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetUserReviews(int userId)
        {
            return await _context.Reviews
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }

    }
}
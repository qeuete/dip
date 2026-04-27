using APISportFoodStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace APISportFoodStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserCardsController : ControllerBase
    {
        private readonly FoodStoreDbContext _context;

        public UserCardsController(FoodStoreDbContext context)
        {
            _context = context;
        }

        // GET: api/UserCards
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserCard>>> GetUserCards()
        {
            return await _context.UserCards
                                 .Where(c => !c.Deleted)
                                 .ToListAsync();
        }

        // GET: api/UserCards/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserCard>> GetUserCard(int? id)
        {
            var userCard = await _context.UserCards
                                         .Where(c => !c.Deleted && c.IdUserCard == id)
                                         .FirstOrDefaultAsync();

            if (userCard == null)
            {
                return NotFound();
            }

            return userCard;
        }

        // PUT: api/UserCards/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserCard(int? id, UserCard userCard)
        {
            if (id != userCard.IdUserCard)
            {
                return BadRequest();
            }

            _context.Entry(userCard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserCardExists(id))
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
        [HttpGet("byUser/{userId}")]
        public async Task<ActionResult<IEnumerable<UserCard>>> GetUserCardsByUser(int userId)
        {
            return await _context.UserCards
                .Where(c => !c.Deleted && c.UserId == userId)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<UserCardDto>> PostUserCard([FromBody] AddCardDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🔹 ВСТАВИТЬ СЮДА
            var parts = dto.Expiry.Split('/');
            if (parts.Length != 2 || !int.TryParse(parts[1], out var year))
                return BadRequest("Неверный формат даты");

            if (year > 40)
                return BadRequest("Год не может быть больше 40");

            // 🔹 дальше твой код
            var card = new UserCard
            {
                UserId = dto.UserId,
                CardNumber = dto.CardNumber?.Replace(" ", ""),
                ExpiryDate = dto.Expiry,
                Cvv = dto.CVV,
                Deleted = false
            };

            _context.UserCards.Add(card);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserCard),
                new { id = card.IdUserCard },
                new UserCardDto
                {
                    IdUserCard = card.IdUserCard ?? 0,
                    CardNumberMasked = MaskCardNumber(card.CardNumber)
                });
        }
        private string MaskCardNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                return "Карта без номера";

            if (number.Length < 4)
                return number;

            // Оставляем последние 4 цифры, остальное маскируем
            return "**** **** **** " + number[^4..];
        }

        // DTO для безопасного приёма с фронта
        public class AddCardDto
        {
            public int UserId { get; set; }
            public string CardNumber { get; set; }
            public string Expiry { get; set; }     // MM/YY
            public string CVV { get; set; }
        }

        // DTO
        public class UserCardDto
        {
            [JsonPropertyName("idUserCard")] // Укажите точное имя из JSON
            public int IdUserCard { get; set; }

            [JsonPropertyName("cardNumberMasked")]
            public string CardNumberMasked { get; set; } = "";
        }

        // DELETE: api/UserCards/5 (логическое удаление)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserCard(int? id)
        {
            var userCard = await _context.UserCards.FindAsync(id);
            if (userCard == null || userCard.Deleted)
            {
                return NotFound();
            }

            userCard.Deleted = true;
            _context.Entry(userCard).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserCardExists(int? id)
        {
            return _context.UserCards.Any(e => e.IdUserCard == id && !e.Deleted);
        }
    }
}

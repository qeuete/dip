using System.ComponentModel.DataAnnotations;

namespace APISportFoodStore.Models
{
    public class AddCardDto
    {
        public int UserId { get; set; }

        [Required]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Номер карты должен быть 16 символов")]
        public string CardNumber { get; set; }

        [Required]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/(\d{2})$", ErrorMessage = "MM/YY")]
        public string Expiry { get; set; }

        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV должен быть 3 символа")]
        public string CVV { get; set; }
    }
}

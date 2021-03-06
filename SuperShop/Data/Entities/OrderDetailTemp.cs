using System.ComponentModel.DataAnnotations;

namespace SuperShop.Data.Entities
{
    public class OrderDetailTemp : IEntity // Quando estamos a fazer a encomenda, antes de confirmar a mesma
    {
        public int Id { get; set; }


        [Required]
        public User User { get; set; }


        [Required]
        public Product Product { get; set; }


        [DisplayFormat(DataFormatString = "{0:C2}")]
        public decimal Price { get; set; }


        [DisplayFormat(DataFormatString = "{0:N2}")]
        public double Quantity { get; set; }


        public decimal Value => Price * (decimal)Quantity;
    }
}

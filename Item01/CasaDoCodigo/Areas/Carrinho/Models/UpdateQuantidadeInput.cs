using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Carrinho.Model
{
    public class UpdateQuantidadeInput
    {
        public UpdateQuantidadeInput()
        {

        }

        public UpdateQuantidadeInput(string productId, int quantity)
        {
            ProductId = productId;
            Quantity = quantity;
        }

        [Required]
        public string ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}

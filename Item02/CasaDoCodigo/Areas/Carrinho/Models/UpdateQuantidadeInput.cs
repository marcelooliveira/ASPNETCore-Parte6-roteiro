using System.ComponentModel.DataAnnotations;

namespace MVC.Areas.Carrinho.Model
{
    public class UpdateQuantidadeInput
    {
        public UpdateQuantidadeInput()
        {

        }

        public UpdateQuantidadeInput(string id, int quantity)
        {
            Id = id;
            Quantity = quantity;
        }

        [Required]
        public string Id { get; set; }
        [Required]
        public int Quantity { get; set; }
    }
}

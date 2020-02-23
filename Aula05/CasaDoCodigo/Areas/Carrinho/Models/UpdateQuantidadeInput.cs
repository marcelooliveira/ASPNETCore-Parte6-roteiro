using System.ComponentModel.DataAnnotations;

namespace CasaDoCodigo.Models
{
    public class UpdateQuantidadeInput
    {
        public UpdateQuantidadeInput()
        {

        }

        public UpdateQuantidadeInput(int id, int quantidade)
        {
            Id = id;
            Quantidade = quantidade;
        }

        [Required]
        public int Id { get; set; }
        [Required]
        public int Quantidade { get; set; }
    }
}

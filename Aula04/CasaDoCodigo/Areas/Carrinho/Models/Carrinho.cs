using System.Collections.Generic;
using System.Linq;

namespace CasaDoCodigo.Models
{
    public class Carrinho
    {
        public Carrinho()
        {
        }

        public Carrinho(string clienteId)
        {
            ClienteId = clienteId;
            Itens = new List<ItemCarrinho>();
        }

        public Carrinho(Carrinho carrinhoCliente)
        {
            this.ClienteId = carrinhoCliente.ClienteId;
            this.Itens = carrinhoCliente.Itens;
        }

        public string ClienteId { get; set; }
        public List<ItemCarrinho> Itens { get; set; }
        public decimal Total => Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
    }
}
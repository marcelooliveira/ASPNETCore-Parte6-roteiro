using System.Linq;

namespace CasaDoCodigo.Models
{
    public class UpdateQuantidadeResponse
    {
        public UpdateQuantidadeResponse()
        {

        }

        public UpdateQuantidadeResponse(ItemPedido itemPedidoDB, Pedido pedido)
        {
            ItemId = itemPedidoDB.Id;
            ItemQuantidade = itemPedidoDB.Quantidade;
            ItemSubtotal = itemPedidoDB.Subtotal;
            NumeroItens = pedido.Itens.Count;
            Total = pedido.Itens.Sum(i => i.Quantidade * i.PrecoUnitario);
        }

        public UpdateQuantidadeResponse(int itemId, int itemQuantidade, decimal itemSubtotal, int numeroItens, decimal total)
        {
            ItemId = itemId;
            ItemQuantidade = itemQuantidade;
            ItemSubtotal = itemSubtotal;
            NumeroItens = numeroItens;
            Total = total;
        }

        public int ItemId { get; set; }
        public int ItemQuantidade { get; set; }
        public decimal ItemSubtotal { get; set; }
        public int NumeroItens { get; set; }
        public decimal Total { get; set; }
    }
}

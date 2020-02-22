namespace CasaDoCodigo.Models
{
    public class UpdateQuantidadeOutput
    {
        public UpdateQuantidadeOutput(ItemCarrinho itemCarrinho, Carrinho carrinho)
        {
            ItemCarrinho = itemCarrinho;
            Carrinho = carrinho;
        }

        public ItemCarrinho ItemCarrinho { get; }
        public Carrinho Carrinho { get; }

        public string ItemId { get { return ItemCarrinho.Id; } }
        public int ItemQuantidade { get { return ItemCarrinho.Quantidade; } }
        public decimal ItemSubtotal { get { return ItemCarrinho.Subtotal; } }
        public int NumeroItens { get { return Carrinho.Itens.Count; } }
        public decimal Total { get { return Carrinho.Total; } }

    }
}

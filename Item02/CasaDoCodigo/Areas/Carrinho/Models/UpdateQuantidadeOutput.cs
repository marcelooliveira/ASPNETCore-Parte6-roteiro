namespace MVC.Areas.Carrinho.Model
{
    public class UpdateQuantidadeOutput
    {
        public UpdateQuantidadeOutput(ItemCarrinho basketItem, CarrinhoCliente customerBasket)
        {
            BasketItem = basketItem;
            CustomerBasket = customerBasket;
        }

        public ItemCarrinho BasketItem { get; }
        public CarrinhoCliente CustomerBasket { get; }
    }
}

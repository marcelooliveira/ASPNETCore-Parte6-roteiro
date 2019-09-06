using MVC.Areas.Carrinho.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Areas.Carrinho.Data
{
    public interface ICarrinhoRepository
    {
        Task<CarrinhoCliente> GetCarrinhoAsync(string clienteId);
        IEnumerable<string> GetUsuarios();
        Task<CarrinhoCliente> UpdateCarrinhoAsync(CarrinhoCliente carrinho);
        Task<CarrinhoCliente> AddItemCarrinhoAsync(string clienteId, ItemCarrinho item);
        Task<UpdateQuantidadeOutput> UpdateItemCarrinhoAsync(string clienteId, UpdateQuantidadeInput item);
        Task<bool> DeleteCarrinhoAsync(string id);
    }
}

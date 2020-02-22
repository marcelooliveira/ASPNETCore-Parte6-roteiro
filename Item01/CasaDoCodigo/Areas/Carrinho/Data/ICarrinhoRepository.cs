using MVC.Areas.Carrinho.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CasaDoCodigo.Areas.Carrinho.Data
{
    public interface ICarrinhoRepository
    {
        Task<MVC.Areas.Carrinho.Model.CarrinhoCliente> GetCarrinhoAsync(string clienteId);
        IEnumerable<string> GetUsuarios();
        Task<MVC.Areas.Carrinho.Model.CarrinhoCliente> UpdateCarrinhoAsync(MVC.Areas.Carrinho.Model.CarrinhoCliente carrinho);
        Task<MVC.Areas.Carrinho.Model.CarrinhoCliente> AddItemCarrinhoAsync(string clienteId, ItemCarrinho item);
        Task<UpdateQuantidadeOutput> UpdateItemCarrinhoAsync(string clienteId, UpdateQuantidadeInput item);
        Task<bool> DeleteCarrinhoAsync(string id);
    }
}

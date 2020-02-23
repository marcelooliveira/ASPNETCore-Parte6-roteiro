using System.Collections.Generic;
using System.Threading.Tasks;
using CasaDoCodigo.Areas.Carrinho.Data;
using CasaDoCodigo.Areas.Catalogo.Data.Repositories;
using CasaDoCodigo.Areas.Identity.Data;
using CasaDoCodigo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CasaDoCodigo.Areas.Carrinho.Controllers
{
    [Area("Carrinho")]
    public class HomeController : Controller
    {
        private readonly ICarrinhoRepository carrinhoRepository;
        private readonly IProdutoRepository produtoRepository;
        private readonly UserManager<AppIdentityUser> userManager;

        public HomeController(ICarrinhoRepository carrinhoRepository,
            IProdutoRepository produtoRepository,
            UserManager<AppIdentityUser> userManager)
        {
            this.carrinhoRepository = carrinhoRepository;
            this.produtoRepository = produtoRepository;
            this.userManager = userManager;
        }

        [Authorize]
        public async Task<IActionResult> Index(string codigo)
        {
            string clienteId = userManager.GetUserId(this.User);

            Models.Carrinho carrinhoCliente;
            if (int.TryParse(codigo, out int produtoId))
            {
                var product = await produtoRepository.GetProdutoAsync(produtoId);
                if (product == null)
                {
                    return RedirectToAction("ProdutoNaoEncontrado", "Carrinho", codigo);
                }

                var item = new ItemCarrinho(product.Id, product.Codigo, product.Nome, product.Preco, 1);
                carrinhoCliente = await carrinhoRepository.AddItemCarrinhoAsync(clienteId, item);
            }
            else
            {
                carrinhoCliente = await carrinhoRepository.GetCarrinhoAsync(clienteId);
            }
            return base.View(carrinhoCliente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<ActionResult<UpdateQuantidadeResponse>> UpdateQuantidade([FromBody]UpdateQuantidadeInput input)
        {
            string clienteId = userManager.GetUserId(this.User);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var output = await carrinhoRepository.UpdateItemCarrinhoAsync(clienteId, input);
                return Ok(output);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(clienteId);
            }
        }
    }
}

using CasaDoCodigo.Areas.Catalogo.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CasaDoCodigo.Areas.Catalogo.Data
{
    public interface ILivroService
    {
        IEnumerable<Produto> GetProdutos();
    }

    public class LivroService : ILivroService
    {
        public IEnumerable<Produto> GetProdutos()
        {
            var livros = GetLivros();

            var categorias = livros
                    .Select((l) => l.Categoria)
                    .Distinct()
                    .Select((nomeCategoria, i) =>
                    {
                        var c = new Categoria(nomeCategoria);
                        c.Id = i + 1;
                        return c;
                    });

            var produtos =
                (from livro in livros
                 join categoria in categorias
                     on livro.Categoria equals categoria.Nome
                 select new Produto(livro.Codigo, livro.Nome, livro.Preco, categoria))
                .Select((p, i) =>
                {
                    p.Id = i + 1;
                    return p;
                })
                .ToList();

            return produtos;
        }

        private List<Livro> GetLivros()
        {
            var json = File.ReadAllText("livros.json");
            return JsonConvert.DeserializeObject<List<Livro>>(json);
        }
    }

    public class Livro
    {
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Categoria { get; set; }
        public string Subcategoria { get; set; }
        public decimal Preco { get; set; }
    }
}

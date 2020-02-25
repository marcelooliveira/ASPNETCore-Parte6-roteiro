using CasaDoCodigo.Areas.Catalogo.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CasaDoCodigo.Areas.Catalogo.Data
{
    public class CatalogoDbContext : DbContext
    {
        public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options)
            : base(options)
        {
        }

        public CatalogoDbContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=CasaDoCodigo.Catalogo;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            var produtos = GetProdutos();
            var categorias =
                produtos.Select(p => p.Categoria).Distinct();

            builder.Entity<Categoria>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasData(categorias);
            });

            builder.Entity<Produto>(b =>
            {
                b.HasKey(t => t.Id);
                b.HasData(
                produtos.Select(p =>
                        new
                        {
                            p.Id,
                            p.Codigo,
                            p.Nome,
                            p.Preco,
                            CategoriaId = p.Categoria.Id
                        }
                    ));
            });
            builder.Entity<Produto>();
        }

        private IEnumerable<Produto> GetProdutos()
        {
            var livros = GetLivros();

            var categorias = livros
                    .Select((l) => l.Categoria)
                    .Distinct()
                    .Select((nomeCategoria, i) =>
                    {
                        var c = new Categoria(nomeCategoria);
                        c.Id = i + 1;
                        return c; });

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

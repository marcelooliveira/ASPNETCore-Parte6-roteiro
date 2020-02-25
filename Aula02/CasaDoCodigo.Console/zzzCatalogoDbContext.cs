//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations.Schema;
//using System.Diagnostics.CodeAnalysis;
//using System.IO;
//using System.Linq;
//using CasaDoCodigo.Console.Models;
//using JetBrains.Annotations;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.EntityFrameworkCore;
//using Newtonsoft.Json;

//namespace EFCoreSQLite
//{
//    //public class BlogContext : DbContext
//    //{
//    //    public BlogContext()
//    //    {
//    //        Database.EnsureCreated();
//    //    }

//    //    public DbSet<Blog> Blogs { get; set; }
//    //    public DbSet<Post> Posts { get; set; }

//    //    protected override void OnConfiguring(DbContextOptionsBuilder options)
//    //          => options.UseSqlite($"Data Source=blog.db");
//    //}

//    //public class Blog
//    //{
//    //    public int Id { get; set; }
//    //    public string Url { get; set; }

//    //    public virtual List<Post> Posts { get; set; } = new List<Post>();
//    //}

//    //public class Post
//    //{
//    //    public int Id { get; set; }
//    //    public string Title { get; set; }
//    //    public string Content { get; set; }

//    //    public int BlogId { get; set; }
//    //    public virtual Blog Blog { get; set; }
//    //}





//    public class CatalogoDbContext : DbContext
//    {
//        public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options)
//            : base(options)
//        {
//        }

//        public CatalogoDbContext()
//        {
//        }

//        protected override void OnModelCreating(ModelBuilder builder)
//        {

//            base.OnModelCreating(builder);

//            var produtos = GetProdutos();
//            var categorias =
//                produtos.Select(p => p.Categoria).Distinct();

//            builder.Entity<Categoria>(b =>
//            {
//                b.HasKey(t => t.Id);
//                b.HasData(categorias);
//            });

//            builder.Entity<Produto>(b =>
//            {
//                b.HasKey(t => t.Id);
//                b.HasData(
//                produtos.Select(p =>
//                        new
//                        {
//                            p.Id,
//                            p.Codigo,
//                            p.Nome,
//                            p.Preco,
//                            CategoriaId = p.Categoria.Id
//                        }
//                    ));
//            });
//            builder.Entity<Produto>();
//        }

//        protected override void OnConfiguring(DbContextOptionsBuilder options)
//              => options.UseSqlite($"Data Source=catalogo.db");

//        private IEnumerable<Produto> GetProdutos()
//        {
//            var livros = GetLivros();

//            var categorias = livros
//                    .Select((l) => l.Categoria)
//                    .Distinct()
//                    .Select((nomeCategoria, i) =>
//                    {
//                        var c = new Categoria(nomeCategoria)
//                        {
//                            Id = i + 1
//                        };
//                        return c;
//                    });

//            var produtos =
//                (from livro in livros
//                 join categoria in categorias
//                     on livro.Categoria equals categoria.Nome
//                 select new Produto(livro.Codigo, livro.Nome, livro.Preco, categoria))
//                .Select((p, i) =>
//                {
//                    p.Id = i + 1;
//                    return p;
//                })
//                .ToList();

//            return produtos;
//        }

//        private List<Livro> GetLivros()
//        {
//            var json = File.ReadAllText("livros.json");
//            return JsonConvert.DeserializeObject<List<Livro>>(json);
//        }
//    }

//    public class Livro
//    {
//        public string Codigo { get; set; }
//        public string Nome { get; set; }
//        public string Categoria { get; set; }
//        public string Subcategoria { get; set; }
//        public decimal Preco { get; set; }
//    }
//}

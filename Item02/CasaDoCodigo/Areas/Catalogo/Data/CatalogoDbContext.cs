using CasaDoCodigo.Areas.Catalogo.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CasaDoCodigo.Areas.Catalogo.Data
{
    //***OBS***
    //PARA A AULA 5, 
    //DROPAR O BANCO
    //APAGAR AS MIGRAÇÕES
    //RODAR NOVAMENTE AS MIGRAÇÕES ABAIXO

    //PM> Add-Migration "AreaCatalogo" -Context CatalogoDbContext -o "Areas/Catalogo/Data/Migrations"
    //PM> Update-Database -verbose -Context CatalogoDbContext 
    public class CatalogoDbContext : DbContext
    {
        private readonly ILivroService livroService;

        public CatalogoDbContext(DbContextOptions<CatalogoDbContext> options,
            ILivroService livroService)
            : base(options)
        {
            this.livroService = livroService;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            var produtos = livroService.GetProdutos();
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
    }
}

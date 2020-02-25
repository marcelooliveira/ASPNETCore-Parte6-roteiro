using CasaDoCodigo.Areas.Catalogo.Data;
using CasaDoCodigo.Areas.Catalogo.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CasaDoCodigo.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var contexto = new CatalogoDbContext())
            {
                contexto.Database.Migrate();

                if (!contexto.Set<Categoria>()
                    .Where(c => c.Nome == "Arquitetura de Software").Any())
                {
                    var novaCategoria = new Categoria("Arquitetura de Software");
                    contexto.Set<Produto>().Add(new Produto("XXX", "ASP.NET Core com Múltiplas Bases de Dados", 100, novaCategoria));
                    contexto.SaveChanges();
                }

                ListarCategorias(contexto);
                ListarProdutos(contexto);

                string pesquisa = string.Empty;
                do
                {
                    System.Console.WriteLine();
                    System.Console.WriteLine("Digite um texto de busca e tecle ENTER...");
                    pesquisa = System.Console.ReadLine();

                    var resultado =
                        contexto.Set<Produto>()
                            .Where(q =>
                            q.Nome.ToLower().Contains(pesquisa)
                            || q.Categoria.Nome.ToLower().Contains(pesquisa))
                            .ToList();

                    System.Console.WriteLine();
                    System.Console.WriteLine("Resultado:");
                    System.Console.WriteLine("==========");
                    foreach (var produto in resultado)
                    {
                        System.Console.WriteLine("ID: {0}, Codigo: {1}, CategoriaId: {2}, Nome: {3}", produto.Id, produto.Codigo, produto.CategoriaId, produto.Nome);
                    }
                } while (!string.IsNullOrEmpty(pesquisa));

                System.Console.WriteLine("Tecle ENTER para sair...");
                System.Console.ReadLine();
            }
        }

        private static void ListarProdutos(CatalogoDbContext contexto)
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Produtos:");
            System.Console.WriteLine("=========");
            foreach (var produto in contexto.Set<Produto>())
            {
                System.Console.WriteLine("ID: {0}, Codigo: {1}, CategoriaId: {2}, Nome: {3}", produto.Id, produto.Codigo, produto.CategoriaId, produto.Nome);
            }
        }

        private static void ListarCategorias(CatalogoDbContext contexto)
        {
            System.Console.WriteLine("Categorias:");
            System.Console.WriteLine("===========");
            foreach (var categoria in contexto.Set<Categoria>())
            {
                System.Console.WriteLine("ID: {0}, Nome: {1}", categoria.Id, categoria.Nome);
            }
        }
    }
}

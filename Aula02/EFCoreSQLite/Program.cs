using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EFCoreSQLite
{
    class Program
    {
        static void Main()
        {
            using (var context = new BlogContext())
            {
                // Create
                Console.WriteLine("Inserindo um novo blog");
                var blog = new Blog { Url = "https://www.caelum.com.br/artigos" };
                context.Add(blog);
                context.SaveChanges();
                Console.WriteLine("Tecle enter para prosseguir...");
                Console.ReadLine();

                // Read
                Console.WriteLine("Consultando um blog");
                var blogs = context.Blogs
                    .OrderBy(b => b.Id)
                    .First();
                Console.WriteLine("Tecle enter para prosseguir...");
                Console.ReadLine();

                // Update
                Console.WriteLine("Atualizando URL do blog e adicionando um novo post");
                blog.Url = "https://www.alura.com.br/artigos";
                Post post = new Post
                {
                    Title = "Microsserviços com .NET Core: Comunicação Entre Serviços",
                    Content = "Comunicação assíncrona e desacoplada em serviços .NET Core"
                };
                blog.Posts.Add(post);
                context.SaveChanges();
                Console.WriteLine("Tecle enter para prosseguir...");
                Console.ReadLine();
                
                // Delete
                Console.WriteLine("Excluindo o blog");
                context.Remove(blog);
                context.SaveChanges();
                Console.WriteLine("Tecle enter para sair...");
                Console.ReadLine();
            }

        }
    }
}
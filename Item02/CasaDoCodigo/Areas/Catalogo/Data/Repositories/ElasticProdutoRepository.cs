using CasaDoCodigo.Areas.Catalogo.Models;
using CasaDoCodigo.Areas.Catalogo.Models.ViewModels;
using Microsoft.Extensions.Configuration;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CasaDoCodigo.Areas.Catalogo.Data.Repositories
{
    public class ElasticProdutoRepository : IProdutoRepository
    {
        private const string ProdutoIndexName = "produto-index";
        private readonly IConfiguration configuration;
        private readonly ILivroService livroService;
        private readonly ConnectionSettings settings;
        private readonly ElasticClient client;

        public ElasticProdutoRepository(IConfiguration configuration, ILivroService livroService)
        {
            this.configuration = configuration;
            this.livroService = livroService;
            var node = new Uri(configuration.GetConnectionString("ElasticSearchNode"));
            settings = CreateConnectionSettings(node);
            client = CreateElasticClient();
        }

        public void Initialize()
        {
            var produtos = livroService.GetProdutos();
            client.IndexMany(produtos, ProdutoIndexName);
        }

        private static ConnectionSettings CreateConnectionSettings(Uri node)
        {
            var settings = new ConnectionSettings(node)
                .DefaultIndex(ProdutoIndexName)
                .DefaultMappingFor<Produto>(d => d
                    .IndexName(ProdutoIndexName)
                );

            return settings;
        }

        public ElasticClient CreateElasticClient()
        {
            var client = new ElasticClient(settings);
            if ((client.Indices.Exists(ProdutoIndexName)).Exists)
            {
                client.Indices.Delete(ProdutoIndexName);
            }

            client.Indices.Create(ProdutoIndexName
                , descriptor => descriptor
                .Map<Produto>(m => m
                    .AutoMap()
                    .Properties(ps => ps
                        .Text(s => s
                            .Name(n => n.Codigo)
                            .Analyzer("substring_analyzer")
                        )
                    )
                )
                .Settings(s => s
                    .Analysis(a => a
                        .Analyzers(analyzer => analyzer
                            .Custom("substring_analyzer", analyzerDescriptor => analyzerDescriptor
                                .Tokenizer("standard")
                                .Filters("lowercase", "substring")
                            )
                        )
                        .TokenFilters(tf => tf
                            .NGram("substring", filterDescriptor => filterDescriptor
                                .MinGram(2)
                                .MaxGram(15)
                            )
                        )
                    )
                )
            );
            return client;
        }

        public async Task<Produto> GetProdutoAsync(string code)
        {
            // returns an IGetResponse mapped 1-to-1 with the Elasticsearch JSON response
            var response =
                await client
                        .GetAsync<Produto>(code,
                            idx => idx.Index(ProdutoIndexName));
            // the original document
            return response.Source;
        }

        public async Task<IList<Produto>> GetProdutosAsync()
        {
            var response =
                await client
                .SearchAsync<Produto>(s => s
                    .Index("produto-index")
                    .Size(1000));

            return response.Documents.ToList();
        }

        public async Task<BuscaProdutosViewModel> GetProdutosAsync(string searchText)
        {
            var response =
                await client
                .SearchAsync<Produto>(s => s
                .Index("produto-index")
                .Query(q =>
                       q.Match(mq => mq.Field(f => f.Nome).Query(searchText))
                    || q.Match(mq => mq.Field(f => f.Categoria.Nome).Query(searchText))
                )
                .Size(1000));

            var produtos = response.Documents.ToList();

            return new BuscaProdutosViewModel(produtos, searchText);
        }
    }
}

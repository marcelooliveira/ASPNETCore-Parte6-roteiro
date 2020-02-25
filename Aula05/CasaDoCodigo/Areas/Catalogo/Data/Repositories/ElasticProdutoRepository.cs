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

            client.Indices.Create(ProdutoIndexName, s =>
            s.Settings(s2 => s2
                .Analysis(a => a
                    .TokenFilters(t => t
                        .Stop("stop", st => st
                            .StopWords("_portuguese_")
                            .RemoveTrailing()
                        )
                        .Synonym("sinonimos", st => st
                            .Synonyms(
                                "csharp, c#"
                            )
                        )
                        .Snowball("snowball", st => st
                            .Language(SnowballLanguage.Portuguese)
                        )
                    )
                    .Analyzers(an => an
                        .Custom("meu_analisador", ca => ca
                            .Tokenizer("standard")
                            .Filters(
                                "lowercase",
                                "stop",
                                "snowball",
                                "sinonimos"
                            )
                        )
                    )
                )
            )
            .Map<Produto>(m => m
                .Properties(p => p
                    .Text(t => t
                        .Name(n => n.Nome)
                        .Analyzer("meu_analisador")
                    )
                )
            ));
            return client;
        }

        public async Task<Produto> GetProdutoAsync(int produtoId)
        {
            // returns an IGetResponse mapped 1-to-1 with the Elasticsearch JSON response
            var response =
                await client
                        .GetAsync<Produto>(produtoId,
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

using CasaDoCodigo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CasaDoCodigo.Repositories
{
    public abstract class BaseRepository<T> where T : BaseModel
    {
        protected readonly IConfiguration configuration;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
    }
}

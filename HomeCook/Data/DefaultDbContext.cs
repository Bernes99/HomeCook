using Microsoft.EntityFrameworkCore;

namespace HomeCook.Data
{
    public partial class DefaultDbContext : BaseDbContext
    {
        public DefaultDbContext()
        {
        }

        public DefaultDbContext(DbContextOptions<DefaultDbContext> options)
            : base(options)
        {
        }
    }
}

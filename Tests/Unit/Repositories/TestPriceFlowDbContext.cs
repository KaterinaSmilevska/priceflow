using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Tests.Unit.Repositories
{
    public class TestPriceFlowDbContext: PriceFlowDbContext
    {
        public TestPriceFlowDbContext(DbContextOptions<PriceFlowDbContext> options): base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    }
}

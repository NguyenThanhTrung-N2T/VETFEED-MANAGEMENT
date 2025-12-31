using Microsoft.EntityFrameworkCore;
namespace VETFEED.Backend.API.Data
{
    public class VetFeedManagementContext : DbContext
    {
        public VetFeedManagementContext(DbContextOptions<VetFeedManagementContext> options) : base(options) { }
    }
}

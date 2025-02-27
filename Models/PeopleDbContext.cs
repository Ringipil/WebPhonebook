using Microsoft.EntityFrameworkCore;

namespace WebPhonebook.Models
{
    public class PeopleDbContext : DbContext
    {
        public PeopleDbContext(DbContextOptions<PeopleDbContext> options) : base(options) { }

        public DbSet<Person> People { get; set; }
    }
}

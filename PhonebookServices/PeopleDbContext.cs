using Microsoft.EntityFrameworkCore;
using WebPhonebook.Models;

namespace WebPhonebook
{
    public class PeopleDbContext : DbContext
    {
        public PeopleDbContext(DbContextOptions<PeopleDbContext> options) : base(options) { }

        public DbSet<Person> People { get; set; }
    }
}

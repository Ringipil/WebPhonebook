using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;

namespace task0
{
    public class EfDatabaseHandler : IDatabaseHandlerBase
    {
        public void InitializeDatabase()
        {
            throw new NotSupportedException("InitializeDatabase is not supported in Entity Framework mode.");
        }

        public class PeopleDbContext : DbContext
        {
            public DbSet<Person> People { get; set; }

            public PeopleDbContext()
                : base(CreateConnection(), true)
            {
            }

            private static DbConnection CreateConnection()
            {
                return new SQLiteConnection("Data Source=people.db;Version=3;");
            }

            protected void OnModelCreating(DbModelBuilder modelBuilder)
            {
                Database.SetInitializer<PeopleDbContext>(null);
                base.OnModelCreating(modelBuilder);
            }
        }

        public List<Person> LoadPeople(string filterByName = "", string filterByContact = "")
        {
            using (var db = new PeopleDbContext())
            {
                IQueryable<Person> query = db.People.AsNoTracking();

                if (!string.IsNullOrEmpty(filterByName))
                {
                    string lowerFilterName = filterByName.ToLower();
                    query = query.Where(p => p.Name.ToLower().Contains(lowerFilterName));
                }

                if (!string.IsNullOrEmpty(filterByContact))
                {
                    string lowerFilterContact = filterByContact.ToLower();
                    query = query.Where(p => p.PhoneNumber.ToLower().Contains(lowerFilterContact) ||
                                             p.Email.ToLower().Contains(lowerFilterContact));
                }

                return query.Take(10000).ToList();
            }
        }

        public void AddPerson(Person person)
        {
            using (var db = new PeopleDbContext())
            {
                db.People.Add(person);
                db.SaveChanges();
            }
        }

        public void UpdatePerson(Person person)
        {
            using (var db = new PeopleDbContext())
            {
                var existingPerson = db.People.Find(person.Id);
                if (existingPerson != null)
                {
                    existingPerson.Name = person.Name;
                    existingPerson.PhoneNumber = person.PhoneNumber;
                    existingPerson.Email = person.Email;
                    db.SaveChanges();
                }
            }
        }

        public void DeletePerson(int id)
        {
            using (var db = new PeopleDbContext())
            {
                var person = db.People.Find(id);
                if (person != null)
                {
                    db.People.Remove(person);
                    db.SaveChanges();
                }
            }
        }

        public async Task GenerateUniqueNames(List<string> firstNames, List<string> middleNames, List<string> lastNames, CancellationToken cancellationToken,
    Action<string> updateStatus, Action<int, double, bool> afterGeneration)
        {
            DateTime startTime = DateTime.Now;
            int countAdded = 0;
            int countChecked = 0;
            int lastReportedCount = 0;
            bool stopRequested = false;
            int nextUpdateThreshold = 1000;

            using (var db = new PeopleDbContext())
            {
                db.Configuration.AutoDetectChangesEnabled = false;

                foreach (var first in firstNames)
                {
                    foreach (var middle in middleNames)
                    {
                        foreach (var last in lastNames)
                        {
                            if (cancellationToken.IsCancellationRequested)
                            {
                                stopRequested = true;
                                break;
                            }

                            string fullName = $"{first} {middle} {last}";
                            countChecked++;

                            bool exists = db.People.Any(p => p.Name == fullName);
                            if (!exists)
                            {
                                db.People.Add(new Person { Name = fullName, PhoneNumber = "", Email = "" });
                                await db.SaveChangesAsync();
                                countAdded++;
                            }

                            if (countChecked >= nextUpdateThreshold)
                            {
                                updateStatus($"{countAdded} added, {countChecked} checked");
                                nextUpdateThreshold += 1000;
                            }
                        }
                        if (stopRequested) break;
                    }
                    if (stopRequested) break;
                }
            }
            TimeSpan elapsedTime = DateTime.Now - startTime;
            afterGeneration(countAdded, elapsedTime.TotalSeconds, stopRequested);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using WebPhonebook;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;

public class EfDatabaseHandler : IDatabaseHandler
{
    private readonly PeopleDbContext _dbContext;

    public EfDatabaseHandler(PeopleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public void InitializeDatabase()
    {
        throw new NotSupportedException("InitializeDatabase is not supported in Entity Framework mode.");
    }

    public List<Person> LoadPeople(string filterByName = "", string filterByContact = "")
    {
        var query = _dbContext.People.AsQueryable();

        if (!string.IsNullOrEmpty(filterByName))
        {
            query = query.Where(p => p.Name.Contains(filterByName));
        }

        if (!string.IsNullOrEmpty(filterByContact))
        {
            query = query.Where(p => p.PhoneNumber.Contains(filterByContact) || p.Email.Contains(filterByContact));
        }

        return query.ToList();
    }

    public void AddPerson(Person person)
    {
        _dbContext.People.Add(person);
        _dbContext.SaveChanges();
    }

    public void UpdatePerson(Person person)
    {
        _dbContext.People.Update(person);
        _dbContext.SaveChanges();
    }

    public void DeletePerson(int id)
    {
        var person = _dbContext.People.Find(id);
        if (person != null)
        {
            _dbContext.People.Remove(person);
            _dbContext.SaveChanges();
        }
    }

    public async Task GenerateUniqueNames(List<string> firstNames, List<string> middleNames, List<string> lastNames, CancellationToken cancellationToken,
    Action<string> updateStatus, Action<int, double, bool> afterGeneration)
    {
        DateTime startTime = DateTime.Now;
        int countAdded = 0;
        int countChecked = 0;
        bool stopRequested = false;
        int nextUpdateThreshold = 1000;

        try
        {
            foreach (var first in firstNames)
            {
                if (stopRequested) break;

                foreach (var middle in middleNames)
                {
                    if (stopRequested) break;

                    foreach (var last in lastNames)
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            stopRequested = true;
                            break;
                        }

                        string fullName = $"{first} {middle} {last}";
                        countChecked++;

                        bool exists = await _dbContext.People.AsNoTracking()
                            .AnyAsync(p => p.Name == fullName, cancellationToken);

                        if (!exists)
                        {
                            _dbContext.People.Add(new Person { Name = fullName, PhoneNumber = "", Email = "" });
                            await _dbContext.SaveChangesAsync(cancellationToken);
                            countAdded++;
                        }

                        if (countChecked >= nextUpdateThreshold)
                        {
                            updateStatus($"{countAdded} added, {countChecked} checked");
                            nextUpdateThreshold += 1000;
                        }
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            stopRequested = true;
        }
        finally
        {
            afterGeneration(countAdded, (DateTime.Now - startTime).TotalSeconds, stopRequested);
        }
    }
}

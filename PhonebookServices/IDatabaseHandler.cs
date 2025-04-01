using PhonebookServices;
using WebPhonebook.Models;

namespace WebPhonebook.Interfaces
{
    public interface IDatabaseHandler
    {
        void InitializeDatabase();
        List<Person> LoadPeople(string filterByName = "", string filterByContact = "");
        Person LoadPerson(int id);
        void AddPerson(Person person);
        void UpdatePerson(Person person);
        void DeletePerson(int id);
        Task GenerateUniqueNames(NameLoader loadnames, CancellationToken cancellationToken,
    Action<string> updateStatus, Action<int, double, bool> afterGeneration);
    }
}

using WebPhonebook.Models;

namespace WebPhonebook.Interfaces
{
    public interface IDatabaseHandler
    {
        void InitializeDatabase();
        List<Person> LoadPeople(string filterByName = "", string filterByContact = "");
        void AddPerson(Person person);
        void UpdatePerson(Person person);
        void DeletePerson(int id);
        Task GenerateUniqueNames(List<string> firstNames, List<string> middleNames, List<string> lastNames, CancellationToken cancellationToken,
    Action<string> updateStatus, Action<int, double, bool> afterGeneration);
    }
}

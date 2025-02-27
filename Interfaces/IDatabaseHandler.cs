using WebPhonebook.Models;

namespace WebPhonebook.Interfaces
{
    public interface IDatabaseHandler
    {
        List<Person> LoadPeople(string filterByName = "", string filterByContact = "");
        void AddPerson(Person person);
        void UpdatePerson(Person person);
        void DeletePerson(int id);
    }
}
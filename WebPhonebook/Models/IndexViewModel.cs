using System.ComponentModel.DataAnnotations;
using WebPhonebook.Models;

public class IndexViewModel
{
    public List<Person> People { get; set; } = new List<Person>();

    [Display(Name = "Search by Name")]
    public string SearchName { get; set; } = "";

    [Display(Name = "Search by Contact")]
    public string SearchContact { get; set; } = "";

    public Person NewPerson { get; set; } = new Person();
}

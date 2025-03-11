using System.ComponentModel.DataAnnotations;
using WebPhonebook.Models;

public class IndexViewModel
{
    private decimal _quantity;

    public decimal Quantity
    {
        get
        {
            return _quantity;
        }
        set
        {
            _quantity = Math.Min(Math.Max(value, 1), 100000);
        }
    }

    public List<Person> People { get; set; } = new List<Person>();

    [Display(Name = "Search by Name")]
    public string SearchName { get; set; } = "";

    [Display(Name = "Search by Contact")]
    public string SearchContact { get; set; } = "";

    public string SelectedHandler { get; set; } = "ef";

    public Person NewPerson { get; set; } = new Person();
}

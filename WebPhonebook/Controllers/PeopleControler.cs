using Microsoft.AspNetCore.Mvc;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;

public class PeopleController : Controller
{
    private readonly SqlDatabaseHandler _sqlHandler;
    private readonly EfDatabaseHandler _efHandler;

    public PeopleController(SqlDatabaseHandler sqlHandler, EfDatabaseHandler efHandler, IHttpContextAccessor httpContextAccessor)
    {
        _sqlHandler = sqlHandler;
        _efHandler = efHandler;
    }

    private IDatabaseHandler GetDbHandler(string selectedhandler)
    {
        return selectedhandler == "sql" ? _sqlHandler : _efHandler;
    }

    public IActionResult Index(IndexViewModel model)
    {
        model.People = GetDbHandler(model.SelectedHandler).LoadPeople(model.SearchName, model.SearchContact);
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(IndexViewModel model, string selectedHandler)
    {
        Person person = model.NewPerson;
        if (string.IsNullOrEmpty(person.Name) || string.IsNullOrEmpty(person.PhoneNumber) || string.IsNullOrEmpty(person.Email))
        {
            TempData["Error"] = "All fields must be filled.";
            return RedirectToAction("Index");
        }

        GetDbHandler(selectedHandler).AddPerson(person);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Update(Person person, string selectedHandler)
    {
        if (person.Id == 0)
        {
            TempData["Error"] = "Invalid person ID.";
            return RedirectToAction("Index");
        }

        GetDbHandler(selectedHandler).UpdatePerson(person);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id, string selectedHandler)
    {
        var person = GetDbHandler(selectedHandler).LoadPeople().FirstOrDefault(p => p.Id == id);

        if (person == null)
        {
            TempData["Error"] = "Person not found.";
            return RedirectToAction("Index");
        }
        return View(person);
    }

    public IActionResult Delete(int id, string selectedHandler)
    {
        var personExists = GetDbHandler(selectedHandler).LoadPeople().Any(p => p.Id == id);

        if (!personExists)
        {
            TempData["Error"] = "Person not found.";
            return RedirectToAction("Index");
        }

        GetDbHandler(selectedHandler).DeletePerson(id);
        return RedirectToAction("Index");
    }
}

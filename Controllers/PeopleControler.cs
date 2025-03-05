using Microsoft.AspNetCore.Mvc;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;

public class PeopleController : Controller
{
    private readonly IDatabaseHandler _dbHandler;

    public PeopleController(IDatabaseHandler dbHandler)
    {
        _dbHandler = dbHandler;
    }

    public IActionResult Index(IndexViewModel model)
    {
        var filteredPeople = _dbHandler.LoadPeople(model.SearchName, model.SearchContact);

        model.People = filteredPeople;

        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Add(Person person)
    {
        if (string.IsNullOrEmpty(person.Name) || string.IsNullOrEmpty(person.PhoneNumber) || string.IsNullOrEmpty(person.Email))
        {
            TempData["Error"] = "All fields must be filled.";
            return RedirectToAction("Index");
        }

        _dbHandler.AddPerson(person);
        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Update(Person person)
    {
        if (person.Id == 0)
        {
            TempData["Error"] = "Invalid person ID.";
            return RedirectToAction("Index");
        }

        _dbHandler.UpdatePerson(person);
        return RedirectToAction("Index");
    }

    public IActionResult Edit(int id)
    {
        var person = _dbHandler.LoadPeople().FirstOrDefault(p => p.Id == id);
        if (person == null)
        {
            TempData["Error"] = "Person not found.";
            return RedirectToAction("Index");
        }
        return View(person);
    }

    public IActionResult Delete(int id)
    {
        _dbHandler.DeletePerson(id);
        return RedirectToAction("Index");
    }
}

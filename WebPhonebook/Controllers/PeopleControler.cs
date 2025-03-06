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

    private IDatabaseHandler _dbHandler
    {
        get
        {
            string selectedHandler = HttpContext.Session.GetString("SelectedHandler") ?? "ef";
            return selectedHandler == "sql" ? _sqlHandler : _efHandler;
        }
    }

    public IActionResult Index(IndexViewModel model)
    {
        model.People = _dbHandler.LoadPeople(model.SearchName, model.SearchContact);

        ViewData["CurrentHandler"] = HttpContext.Session.GetString("SelectedHandler") ?? "ef";

        return View(model);
    }

    [HttpPost]
    public IActionResult SwitchDatabaseHandler(string selectedHandler)
    {
        HttpContext.Session.SetString("SelectedHandler", selectedHandler == "sql" ? "sql" : "ef");
        return RedirectToAction("Index");
    }

    public IActionResult GetSwitchHandlerPartial()
    {
        ViewData["CurrentHandler"] = HttpContext.Session.GetString("SelectedHandler") ?? "ef";
        return PartialView("SwitchDatabaseHandler");
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
        var personExists = _dbHandler.LoadPeople().Any(p => p.Id == id);

        if (!personExists)
        {
            TempData["Error"] = "Person not found.";
            return RedirectToAction("Index");
        }

        _dbHandler.DeletePerson(id);
        return RedirectToAction("Index");
    }
}

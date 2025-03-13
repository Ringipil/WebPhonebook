using Microsoft.AspNetCore.Mvc;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;
using PhonebookServices;
using WebPhonebook.Services;
using System;
using System.Threading;

public class PeopleController : Controller
{
    private readonly NameLoader _nameLoader;
    private readonly SqlDatabaseHandler _sqlHandler;
    private readonly EfDatabaseHandler _efHandler; 
    private static string _generationStatus = " ";
    private readonly NameGenerationService _nameGenerationService;
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public PeopleController(SqlDatabaseHandler sqlHandler, EfDatabaseHandler efHandler,NameLoader nameLoader,
        IHttpContextAccessor httpContextAccessor, NameGenerationService nameGenerationService)
    {
        _sqlHandler = sqlHandler;
        _efHandler = efHandler;
        _nameLoader = nameLoader;
        _nameGenerationService = nameGenerationService;
    }

    public IActionResult Generate()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Generate(string selectedHandler)
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }

        _cancellationTokenSource = new CancellationTokenSource();
        var dbHandler = GetDbHandler(selectedHandler);

        string message = "Generation started...";

        await _nameGenerationService.StartGeneration(dbHandler, _cancellationTokenSource.Token,
            (countAdded, elapsedTime, wasStopped) =>
            {
                message = wasStopped
                    ? $"Generation stopped. {countAdded} names added in {elapsedTime:F2} seconds."
                    : $"Generation complete. {countAdded} names added in {elapsedTime:F2} seconds.";

                TempData["Message"] = message;
            });

        return RedirectToAction("Generate");
    }

    [HttpPost]
    public IActionResult StopGeneration()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            TempData["Message"] = "Generation stopped.";
        }
        else
        {
            TempData["Message"] = "No generation process is running.";
        }

        return RedirectToAction("Generate");
    }

    [HttpGet]
    public IActionResult GetGenerationStatus()
    {
        return Content(_nameGenerationService.GetGenerationStatus());
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

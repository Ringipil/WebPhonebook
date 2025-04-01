using Microsoft.AspNetCore.Mvc;
using WebPhonebook.Interfaces;
using WebPhonebook.Models;
using PhonebookServices;
using WebPhonebook.Services;
using System;
using System.Threading;
using WebPhonebook;

public class PeopleController : Controller
{
    private readonly NameLoader _nameLoader;
    private readonly SqlDatabaseHandler _sqlHandler;
    private readonly EfDatabaseHandler _efHandler; 
    private readonly NameGenerationService _nameGenerationService;
    private static CancellationTokenSource? _cancellationTokenSource = null;

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
        var model = new GenerateViewModel
        {
            SelectedHandler = HttpContext.Session.GetString("SelectedHandler") ?? "ef"
        };
        return View(model);
    }

    [HttpPost]
    public IActionResult Generate(GenerateViewModel model)
    {
        if (_cancellationTokenSource != null)
        {
            TempData["Message"] = "Generation is already running.";
            return RedirectToAction("Generate", new { selectedHandler = model.SelectedHandler });
        }

        _cancellationTokenSource = new CancellationTokenSource();
        
        Task.Run(async () =>
        {
            using (var scope = HttpContext.RequestServices.CreateScope())
            {
                IDatabaseHandler dbHandler = model.SelectedHandler == "ef"
                    ? scope.ServiceProvider.GetRequiredService<SqlDatabaseHandler>()
                    : scope.ServiceProvider.GetRequiredService<EfDatabaseHandler>();

                await _nameGenerationService.StartGeneration(dbHandler, _cancellationTokenSource.Token,
                    (countAdded, elapsedTime, wasStopped) =>
                    {
                        TempData["Message"] = _nameGenerationService.FormatGenerationMessage(countAdded, elapsedTime, wasStopped);
                        _cancellationTokenSource = null;
                    });
            }
        });

        TempData["Message"] = "Generation started...";
        return RedirectToAction("Generate", new { selectedHandler = model.SelectedHandler });
    }

    [HttpPost]
    public IActionResult StopGeneration(GenerateViewModel model)
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        else
        {
            TempData["Message"] = "No generation process is running.";
        }

        return RedirectToAction("Generate", new { selectedHandler = model.SelectedHandler });
    }

    [HttpPost]
    public IActionResult HandlerSwitch(GenerateViewModel model)
    {
        if (model.SelectedHandler != "sql" && model.SelectedHandler != "ef")
        {
            model.SelectedHandler = "sql";
        }

        HttpContext.Session.SetString("SelectedHandler", model.SelectedHandler);

        return RedirectToAction("Generate", new { selectedHandler = model.SelectedHandler });
    }

    [HttpGet]
    public IActionResult GetGenerationStatus()
    {
        return Content(_nameGenerationService.GetGenerationStatus());
    }

    private IDatabaseHandler GetDbHandler(string selectedHandler)
    {
        return selectedHandler == "sql" ? _sqlHandler : _efHandler;
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
        var person = GetDbHandler(selectedHandler).LoadPerson(id);
        if (person == null)
        {
            TempData["Error"] = "Person not found.";
            return RedirectToAction("Index");
        }
        return View(person);
    }

    public IActionResult Delete(int id, string selectedHandler)
    {
        var person = GetDbHandler(selectedHandler).LoadPerson(id);
        if (person == null)
        {
            TempData["Error"] = "Person not found.";
            return RedirectToAction("Index");
        }
        GetDbHandler(selectedHandler).DeletePerson(id);
        return RedirectToAction("Index");
    }
}

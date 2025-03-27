using Microsoft.AspNetCore.Mvc;
using PhonebookServices;
using WebPhonebook.Models;

namespace WebPhonebook.Controllers
{
    public class UploadController : Controller
    {
        private readonly NameLoader _nameLoader;

        public UploadController(NameLoader nameLoader)
        {
            _nameLoader = nameLoader;
        }

        public IActionResult Index()
        {
            return View("Index", new UploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            try
            {
                if (!Directory.Exists(_nameLoader.NameFilesDirectory))
                {
                    Directory.CreateDirectory(_nameLoader.NameFilesDirectory);
                }

                await SaveFile(model.FirstNamesFile, "firstNames.txt");
                await SaveFile(model.MiddleNamesFile, "middleNames.txt");
                await SaveFile(model.LastNamesFile, "lastNames.txt");

                ViewBag.Message = "Files uploaded successfully!";
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Error uploading files: {ex.Message}";
            }

            return View("Index", new UploadViewModel());
        }

        private async Task SaveFile(IFormFile file, string fileName)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Invalid file.");

            string filePath = Path.Combine(_nameLoader.NameFilesDirectory, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
    }
}

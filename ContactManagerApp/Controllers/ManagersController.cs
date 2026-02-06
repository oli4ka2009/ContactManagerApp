using ContactManagerApp.Models;
using ContactManagerApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagerApp.Controllers
{
    public class ManagersController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly ILogger<ManagersController> _logger;

        public ManagersController(IManagerService managerService, ILogger<ManagersController> logger)
        {
            _managerService = managerService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var managers = await _managerService.GetAllManagersAsync();
            return View(managers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                ModelState.AddModelError("File", "Please select a file.");
                return View("Index", await _managerService.GetAllManagersAsync());
            }

            if (!file.FileName.EndsWith(".csv"))
            {
                ModelState.AddModelError("File", "Only CSV files are allowed.");
                return View("Index", await _managerService.GetAllManagersAsync());
            }

            try
            {
                using (var stream = file.OpenReadStream())
                {
                    await _managerService.UploadManagersAsync(stream);
                }

                TempData["SuccessMessage"] = "Managers uploaded successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                ModelState.AddModelError("File", $"An error occurred while uploading: {ex.Message}");
                return View("Index", await _managerService.GetAllManagersAsync());
            }
        }
        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] Manager manager)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _managerService.EditManagerAsync(manager);
                return Ok(new { message = "Updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating manager");
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromBody] int id)
        {
            try
            {
                await _managerService.DeleteManagerAsync(id);
                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting manager");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

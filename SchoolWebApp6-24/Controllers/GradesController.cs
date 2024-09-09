using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolWebApp6_24.DTO;
using SchoolWebApp6_24.Services;
using SchoolWebApp6_24.ViewModels;

namespace SchoolWebApp6_24.Controllers {

    public class GradesController : Controller {
        private readonly GradeService _gradeService;

        public GradesController(GradeService gradeService) {
            _gradeService = gradeService;
        }

        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> Create() {
            await FillSelectsAsync();
            return View();
        }

        [Authorize(Roles = "Teacher, Admin")]
        private async Task FillSelectsAsync() {
            var gradesDropdownsData = await _gradeService.GetGradesDropdownsData();
            ViewBag.Students = new SelectList(gradesDropdownsData.Students, "Id", "FullName");
            ViewBag.Subjects = new SelectList(gradesDropdownsData.Subjects, "Id", "Name");
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(GradeDTO gradeDTO) {
            var currentCount = await _gradeService.GetGradesCountAsync();
            if (currentCount >= 15) {
                ModelState.AddModelError(string.Empty, "Nelze vytvořit více, než 15 záznamů, nejdříve některé ostraňte.");
                await FillSelectsAsync();
                return View(gradeDTO);
            }

            if (!ModelState.IsValid) {
                await FillSelectsAsync();
                return View(gradeDTO);
            }
            await _gradeService.CreateAsync(gradeDTO);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Index() {
            var allGrades = await _gradeService.GetAllVMsAsync();
            ViewBag.GradeCount = allGrades.Count();
            return View(allGrades);
        }

        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> Update(int id) {
            var gradeToEdit = await _gradeService.GetByIdAsync(id);
            if (gradeToEdit == null) {
                return View("NotFound");
            }
            var gradeDto = _gradeService.ModelToDto(gradeToEdit);
            await FillSelectsAsync();
            return View(gradeDto);
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(int id, GradeDTO gradeDTO) {
            if (!ModelState.IsValid) {
                await FillSelectsAsync();
                return View(gradeDTO);
            }
            await _gradeService.UpdateAsync(id, gradeDTO);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> DeleteAsync(int id) {
            await _gradeService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}

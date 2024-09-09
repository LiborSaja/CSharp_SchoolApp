using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp6_24.DTO;
using SchoolWebApp6_24.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolWebApp6_24.Controllers {
    [Authorize]
    public class SubjectsController : Controller {
        private readonly SubjectService _subjectService;

        public SubjectsController(SubjectService subjectService) {
            _subjectService = subjectService;
        }

        public IActionResult Index() {
            IEnumerable<SubjectDTO> allSubjects = _subjectService.GetSubjects();
            ViewBag.SubjectCount = allSubjects.Count();
            return View(allSubjects);
        }

        [Authorize(Roles = "Teacher, Admin")]
        public IActionResult Create() {
            return View();
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(SubjectDTO subjectDTO) {
            var currentCount = await _subjectService.GetSubjectCountAsync();      // Získání aktuálního počtu subjektů     
            if (currentCount >= 10) {                                             // Kontrola, zda počet subjektů nepřesahuje limit
                ModelState.AddModelError(string.Empty, "Nelze vytvořit více, než 10 předmětů. Nejdříve některé odstraňte.");// Přidání chybové zprávy
                return View(subjectDTO);
            }
            if (!ModelState.IsValid) {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Toto pole musí být vyplněno.");
                return View(subjectDTO);
            }
            var existingSubject = await _subjectService.GetByNameAsync(subjectDTO.Name);
            if (existingSubject != null) {
                ModelState.AddModelError(string.Empty, "Předmět s tímto názvem již existuje.");
                return View(subjectDTO);
            }

            await _subjectService.AddSubjectAsync(subjectDTO);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> UpdateAsync(int id) {
            var subjectToEdit = await _subjectService.GetByIdAsync(id);
            if (subjectToEdit == null) {
                return View("NotFound");
            }
            return View(subjectToEdit);
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(SubjectDTO subjectDTO, int id) {
            if (!ModelState.IsValid) {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Toto pole musí být vyplněno.");
                return View(subjectDTO);
            }
            await _subjectService.UpdateAsync(id, subjectDTO);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id) {
            var subjectToDelete = await _subjectService.GetByIdAsync(id);
            if (subjectToDelete == null) {
                return View("NotFound");
            }
            await _subjectService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}

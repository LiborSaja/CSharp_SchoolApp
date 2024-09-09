using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp6_24.DTO;
using SchoolWebApp6_24.Models;
using SchoolWebApp6_24.Services;

namespace SchoolWebApp6_24.Controllers {
    [Authorize]
    public class StudentsController : Controller {
        private StudentService _studentService;
        private UserManager<AppUser> _userManager;

        public StudentsController(StudentService studentService, UserManager<AppUser> userManager) {
            _studentService = studentService;
            _userManager = userManager;
        }

        public IActionResult Index() {
            IEnumerable<StudentDTO> allStudents = _studentService.GetStudents();
            ViewBag.StudentCount = allStudents.Count();
            return View(allStudents);
        }

        [Authorize(Roles = "Teacher, Admin")]
        public IActionResult Create() {
            return View();
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(StudentDTO studentDTO) {
            // Check if the current count of students exceeds the limit
            var currentCount = await _studentService.GetStudentCountAsync();
            if (currentCount >= 30) {
                ModelState.AddModelError(string.Empty, "Nelze vytvořit více, než 30 studentů, nejdříve některé ostraňte.");
                return View(studentDTO);
            }

            // Validate the model state
            if (!ModelState.IsValid) {
                ModelState.AddModelError(string.Empty, "Všechna pole musí být vyplněna.");
                return View(studentDTO);
            }

            // Validate for diacritics in username
            string username = $"{studentDTO.FirstName}{studentDTO.LastName}";
            if (username.Any(c => c > 127)) {
                ModelState.AddModelError(string.Empty, "Nelze používat diakritiku, či jiné znaky.");
                return View(studentDTO);
            }

            // Create a new user for the student
            var user = new AppUser {
                UserName = username,
                Email = $"{username}@{username}.com" // You can generate or require an email input
            };

            var userCreationResult = await _userManager.CreateAsync(user, "DefaultPassword123!"); // You might want to prompt for a password or generate one
            if (!userCreationResult.Succeeded) {
                foreach (var error in userCreationResult.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(studentDTO);
            }

            // Add the student only if the user creation is successful
            await _studentService.AddStudentAsync(studentDTO);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> UpdateAsync(int id) {
            var studentToEdit = await _studentService.GetByIdAsync(id);
            if (studentToEdit == null) {
                return View("NotFound");
            }
            return View(studentToEdit);
        }

        [Authorize(Roles = "Teacher, Admin")]
        [HttpPost]
        public async Task<IActionResult> Update(StudentDTO studentDTO, int id) {
            if (!ModelState.IsValid) {
                ModelState.Clear();
                ModelState.AddModelError(string.Empty, "Všechna pole musí být vyplněna.");
                return View(studentDTO);
            }
            await _studentService.UpdateAsync(id, studentDTO);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<IActionResult> Delete(int id) {
            var studentToDelete = await _studentService.GetByIdAsync(id);
            if (studentToDelete == null) {
                return View("NotFound");
            }
            await _studentService.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SchoolWebApp6_24.Models;
using SchoolWebApp6_24.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SchoolWebApp6_24.Controllers {
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        private readonly List<string> _protectedUserIds;

        public UsersController(
            UserManager<AppUser> userManager,
            IPasswordHasher<AppUser> passwordHasher,
            IPasswordValidator<AppUser> passwordValidator,
            IConfiguration configuration) {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _passwordValidator = passwordValidator;
            _protectedUserIds = configuration.GetSection("ProtectedUsers").Get<List<string>>();
        }

        public IActionResult Index() {
            ViewBag.UserCount = _userManager.Users.Count();
            ViewBag.ProtectedUserIds = _protectedUserIds;
            return View(_userManager.Users);
        }

        public ViewResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(UserViewModel userViewModel) {
            if (ModelState.IsValid) {
                AppUser newUser = new AppUser {
                    UserName = userViewModel.Name,
                    Email = userViewModel.Email
                };
                IdentityResult identityResult = await _userManager.CreateAsync(newUser, userViewModel.Password);
                if (identityResult.Succeeded)
                    return RedirectToAction("Index");
                else {
                    AddErrors(identityResult);
                }
            }
            return View(userViewModel);
        }

        public async Task<IActionResult> Update(string id) {
            if (_protectedUserIds.Contains(id)) {
                ModelState.AddModelError("", "Tento uživatel nelze upravovat.");
                return RedirectToAction("Index");
            }

            AppUser userToUpdate = await _userManager.FindByIdAsync(id);
            if (userToUpdate == null) {
                return View("NotFound");
            }
            return View(userToUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsync(string id, string email, string password) {
            AppUser userToUpdate = await _userManager.FindByIdAsync(id);
            if (userToUpdate != null) {
                IdentityResult validPass = null;
                if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(password)) {
                    userToUpdate.Email = email;
                    validPass = await _passwordValidator.ValidateAsync(_userManager, userToUpdate, password);
                    if (validPass.Succeeded) {
                        userToUpdate.PasswordHash = _passwordHasher.HashPassword(userToUpdate, password);
                        IdentityResult identityResult = await _userManager.UpdateAsync(userToUpdate);
                        if (identityResult.Succeeded) {
                            return RedirectToAction("Index");
                        }
                        else {
                            AddErrors(identityResult);
                        }
                    }
                    else {
                        AddErrors(validPass);
                    }
                }
            }
            else {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(userToUpdate);
        }

        private void AddErrors(IdentityResult identityResult) {
            foreach (var error in identityResult.Errors) {
                ModelState.AddModelError("", error.Description);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id) {
            if (_protectedUserIds.Contains(id)) {
                ModelState.AddModelError("", "Tento uživatel nelze smazat.");
                return RedirectToAction("Index");
            }

            AppUser userToDelete = await _userManager.FindByIdAsync(id);
            if (userToDelete != null) {
                IdentityResult result = await _userManager.DeleteAsync(userToDelete);
                if (result.Succeeded) {
                    return RedirectToAction("Index");
                }
                else {
                    AddErrors(result);
                }
            }
            else {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", _userManager.Users);
        }
    }
}

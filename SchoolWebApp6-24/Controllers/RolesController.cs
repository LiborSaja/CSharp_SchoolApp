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
    public class RolesController : Controller {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly List<string> _protectedRoleIds;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager, IConfiguration configuration) {
            _roleManager = roleManager;
            _userManager = userManager;
            _protectedRoleIds = configuration.GetSection("ProtectedRoles").Get<List<string>>();
        }

        public IActionResult Index() {
            ViewBag.ProtectedRoleIds = _protectedRoleIds;
            return View(_roleManager.Roles);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string name) {
            if (string.IsNullOrWhiteSpace(name)) {
                ModelState.AddModelError(string.Empty, "Název role je povinný.");
                return View();
            }

            var existingRole = await _roleManager.FindByNameAsync(name);
            if (existingRole != null) {
                ModelState.AddModelError("", "Tato role je již vytvořena.");
                return View();
            }

            var identityResult = await _roleManager.CreateAsync(new IdentityRole(name));
            if (identityResult.Succeeded) {
                return RedirectToAction("Index");
            }
            else {
                AddErrors(identityResult);
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAsync(string id) {
            if (_protectedRoleIds.Contains(id)) {
                ModelState.AddModelError("", "Tuto roli nelze smazat.");
                return RedirectToAction("Index");
            }

            var roleToDelete = await _roleManager.FindByIdAsync(id);
            if (roleToDelete != null) {
                IdentityResult identityResult = await _roleManager.DeleteAsync(roleToDelete);
                if (identityResult.Succeeded) {
                    return RedirectToAction("Index");
                }
                else {
                    AddErrors(identityResult);
                }
            }
            ModelState.AddModelError("", "Role nenalezena");
            return View("Index", _roleManager.Roles);
        }

        public async Task<IActionResult> Edit(string id) {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) {
                return NotFound();
            }

            var model = new EditRoleViewModel {
                Id = role.Id,
                Name = role.Name
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditRoleViewModel model) {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null) {
                return NotFound();
            }

            var existingRole = await _roleManager.FindByNameAsync(model.Name);
            if (existingRole != null && existingRole.Id != model.Id) {
                ModelState.AddModelError("", "Tato role je již vytvořena.");
                return View(model);
            }

            if (ModelState.IsValid) {
                role.Name = model.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded) {
                    return RedirectToAction("Index");
                }
                else {
                    AddErrors(result);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Update(string id) {
            var role = await _roleManager.FindByIdAsync(id);
            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();
            foreach (AppUser user in _userManager.Users) {
                var list = await _userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }
            return View(new RoleEdit {
                Role = role,
                RoleMembers = members,
                RoleNonMembers = nonMembers
            });
        }

        [HttpPost]
        public async Task<IActionResult> Update(RoleModifications roleModifications) {
            AppUser appUser;
            IdentityResult identityResult;
            foreach (var id in roleModifications.AddIds ?? new string[] { }) {
                appUser = await _userManager.FindByIdAsync(id);
                if (appUser != null) {
                    identityResult = await _userManager.AddToRoleAsync(appUser, roleModifications.RoleName);
                    if (identityResult != IdentityResult.Success) {
                        AddErrors(identityResult);
                    }
                }
            }
            foreach (var id in roleModifications.DeleteIds ?? new string[] { }) {
                appUser = await _userManager.FindByIdAsync(id);
                if (appUser != null) {
                    identityResult = await _userManager.RemoveFromRoleAsync(appUser, roleModifications.RoleName);
                    if (identityResult != IdentityResult.Success) {
                        AddErrors(identityResult);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        private void AddErrors(IdentityResult identityResult) {
            foreach (var error in identityResult.Errors) {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}

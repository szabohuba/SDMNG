using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public class RoleController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RoleController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    public IActionResult Index()
    {
        var roles = _roleManager.Roles.ToList();
        return View(roles);
    }

    [HttpGet]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRole(string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
        {
            ModelState.AddModelError("", "Role name is required.");
            return View();
        }

        var result = await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));

        if (result.Succeeded)
        {
            return RedirectToAction("Index");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
        {
            return NotFound();
        }

        var result = await _roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return RedirectToAction("Index");
    }


}

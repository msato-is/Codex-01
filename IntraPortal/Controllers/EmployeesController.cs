using IntraPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntraPortal.Controllers;

public class EmployeesController(PortalDbContext db) : Controller
{
    public async Task<IActionResult> Search(string? q, string? dept, string sort = "name", string dir = "asc", int page = 1, int pageSize = 10)
    {
        q = q?.Trim();
        dept = string.IsNullOrWhiteSpace(dept) ? null : dept.Trim();

        var departments = await db.Employees
            .Select(e => e.Department)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();
        ViewBag.Departments = departments;

        var qry = db.Employees.AsQueryable();
        if (!string.IsNullOrEmpty(q))
        {
            if (q.Length > 50) { ModelState.AddModelError("q", "検索語は50文字以内"); }
            qry = qry.Where(e => e.Name.Contains(q));
        }
        if (!string.IsNullOrEmpty(dept))
        {
            qry = qry.Where(e => e.Department == dept);
        }

        qry = (sort, dir.ToLower()) switch
        {
            ("dept", "desc") => qry.OrderByDescending(e => e.Department).ThenBy(e => e.Name),
            ("dept", _) => qry.OrderBy(e => e.Department).ThenBy(e => e.Name),
            ("name", "desc") => qry.OrderByDescending(e => e.Name),
            _ => qry.OrderBy(e => e.Name)
        };

        var total = await qry.CountAsync();
        var items = await qry.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        ViewBag.Query = q;
        ViewBag.SelectedDept = dept;
        ViewBag.Sort = sort; ViewBag.Dir = dir; ViewBag.Page = page; ViewBag.Total = total; ViewBag.PageSize = pageSize;
        return View(items);
    }

    [HttpGet("/Employees/{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var emp = await db.Employees.FindAsync(id);
        if (emp is null) return NotFound();
        return Json(emp);
    }
}


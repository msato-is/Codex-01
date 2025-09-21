using IntraPortal.Data;
using IntraPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntraPortal.Controllers;

public class NoticesController(PortalDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var list = await db.Notices
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NoticeListVm
            {
                Id = n.Id,
                Title = n.Title,
                Author = n.Author,
                CreatedAt = n.CreatedAt,
                ReadCount = db.NoticeReads.Count(r => r.NoticeId == n.Id)
            })
            .ToListAsync();
        return View(list);
    }

    public async Task<IActionResult> Details(int id, string reader = "ゲスト")
    {
        var n = await db.Notices.FindAsync(id);
        if (n is null) return NotFound();
        db.NoticeReads.Add(new NoticeRead { NoticeId = id, Reader = reader, ReadAt = DateTimeOffset.Now });
        await db.SaveChangesAsync();
        ViewBag.Reader = reader;
        return View(n);
    }

    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(Notice m)
    {
        if (string.IsNullOrWhiteSpace(m.Title) || m.Title.Length is < 1 or > 80)
            ModelState.AddModelError("Title", "タイトルは1–80文字");
        if (string.IsNullOrWhiteSpace(m.Body) || m.Body.Length is < 1 or > 2000)
            ModelState.AddModelError("Body", "本文は1–2000文字");
        if (!ModelState.IsValid)
            return View(m);
        m.CreatedAt = DateTimeOffset.Now;
        db.Notices.Add(m);
        await db.SaveChangesAsync();
        TempData["Toast"] = "投稿しました";
        return RedirectToAction(nameof(Index));
    }

    public class NoticeListVm
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public int ReadCount { get; set; }
    }
}


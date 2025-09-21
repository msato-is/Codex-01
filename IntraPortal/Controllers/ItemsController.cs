using IntraPortal.Data;
using IntraPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntraPortal.Controllers;

public class ItemsController(PortalDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var items = await db.Items.OrderBy(i => i.Name).ToListAsync();
        var recent = await db.ItemRequests.Include(r => r.Item)
            .OrderByDescending(r => r.RequestedAt).Take(20).ToListAsync();
        ViewBag.Recent = recent;
        return View(items);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitRequest(ItemRequest m)
    {
        if (m.Quantity < 1 || m.Quantity > 99)
            ModelState.AddModelError("Quantity", "数量は1〜99");
        if (!ModelState.IsValid)
        {
            TempData["Toast"] = "入力内容を確認してください";
            return RedirectToAction(nameof(Index));
        }

        m.RequestedAt = DateTimeOffset.Now;
        db.ItemRequests.Add(m);
        await db.SaveChangesAsync();

        var item = await db.Items.FindAsync(m.ItemId);
        if (item is not null && m.Quantity > item.Stock)
        {
            TempData["Toast"] = $"在庫不足: 在庫 {item.Stock} に対して {m.Quantity} を申請";
        }
        else
        {
            TempData["Toast"] = "申請を保存しました";
        }
        return RedirectToAction(nameof(Index));
    }
}

using IntraPortal.Data;
using IntraPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace IntraPortal.Controllers;

public class LunchController(PortalDbContext db, IConfiguration config) : Controller
{
    public async Task<IActionResult> Index()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var menus = await db.LunchMenus.Where(m => m.Date == today).OrderBy(m => m.Code).ToListAsync();
        ViewBag.Menus = menus;
        ViewBag.Accepting = IsAccepting();
        return View();
    }

    private bool IsAccepting()
    {
        if (string.Equals(config["LUNCH_ORDER_DEMO"], "true", StringComparison.OrdinalIgnoreCase)) return true;
        var now = DateTime.Now;
        return now.Hour < 11 || (now.Hour == 11 && now.Minute == 0);
    }

    [HttpPost]
    public async Task<IActionResult> Order(int menuId, string employeeName, int quantity)
    {
        if (!IsAccepting())
        {
            TempData["Toast"] = "受付時間外です";
            return RedirectToAction(nameof(Index));
        }
        if (string.IsNullOrWhiteSpace(employeeName) || quantity < 1 || quantity > 10)
        {
            TempData["Toast"] = "氏名と個数(1–10)を入力してください";
            return RedirectToAction(nameof(Index));
        }
        var menu = await db.LunchMenus.FindAsync(menuId);
        if (menu is null)
        {
            TempData["Toast"] = "メニューが見つかりません";
            return RedirectToAction(nameof(Index));
        }
        db.LunchOrders.Add(new LunchOrder { MenuId = menuId, EmployeeName = employeeName.Trim(), Quantity = quantity, OrderedAt = DateTimeOffset.Now });
        await db.SaveChangesAsync();
        TempData["Toast"] = "注文を受け付けました";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Summary()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var query = from o in db.LunchOrders
                    join m in db.LunchMenus on o.MenuId equals m.Id
                    where m.Date == today
                    group new { o, m } by new { m.Code, m.Name, m.Price } into g
                    select new LunchSummaryRow
                    {
                        Code = g.Key.Code,
                        Name = g.Key.Name,
                        Price = g.Key.Price,
                        Quantity = g.Sum(x => x.o.Quantity),
                        Total = g.Sum(x => x.o.Quantity) * g.Key.Price
                    };
        var rows = await query.OrderBy(r => r.Code).ToListAsync();
        return View(rows);
    }

    public async Task<IActionResult> Csv()
    {
        var today = DateOnly.FromDateTime(DateTime.Now);
        var query = from o in db.LunchOrders
                    join m in db.LunchMenus on o.MenuId equals m.Id
                    where m.Date == today
                    orderby o.OrderedAt
                    select new { o.EmployeeName, m.Code, m.Name, m.Price, o.Quantity, o.OrderedAt };
        var list = await query.ToListAsync();
        var sb = new StringBuilder();
        sb.AppendLine("社員名,コード,メニュー,価格,数量,日時");
        foreach (var x in list)
        {
            sb.AppendLine($"{x.EmployeeName},{x.Code},{x.Name},{x.Price},{x.Quantity},{x.OrderedAt:yyyy-MM-dd HH:mm:ss}");
        }
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        return File(bytes, "text/csv; charset=UTF-8", $"lunch_{today:yyyyMMdd}.csv");
    }

    public class LunchSummaryRow
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }
    }
}


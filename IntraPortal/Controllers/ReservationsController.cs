using IntraPortal.Data;
using IntraPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IntraPortal.Controllers;

public class ReservationsController(PortalDbContext db) : Controller
{
    public async Task<IActionResult> Index(DateOnly? date, int? roomId)
    {
        var rooms = await db.MeetingRooms.OrderBy(r => r.Name).ToListAsync();
        ViewBag.Rooms = rooms;
        var d = date ?? DateOnly.FromDateTime(DateTime.Today);
        ViewBag.Date = d;
        ViewBag.RoomId = roomId;

        var query = db.Reservations.Include(r => r.Room).Where(r => r.Date == d);
        if (roomId is not null) query = query.Where(r => r.RoomId == roomId);
        var list = await query.OrderBy(r => r.StartTime).ToListAsync();

        ViewBag.FreeSlots = CalcFreeSlots(list);
        return View(list);
    }

    private static List<(TimeOnly start, TimeOnly end)> CalcFreeSlots(List<Reservation> exists)
    {
        var open = new TimeOnly(9, 0);
        var close = new TimeOnly(18, 0);
        var slots = new List<(TimeOnly, TimeOnly)>();
        var cur = open;
        foreach (var r in exists.OrderBy(r => r.StartTime))
        {
            if (r.StartTime > cur) slots.Add((cur, r.StartTime));
            if (r.EndTime > cur) cur = r.EndTime;
        }
        if (cur < close) slots.Add((cur, close));
        return slots;
    }

    [HttpPost]
    public async Task<IActionResult> Create(Reservation m)
    {
        if (m.StartTime >= m.EndTime)
            ModelState.AddModelError("StartTime", "開始は終了より前である必要があります");
        if (m.Date < DateOnly.FromDateTime(DateTime.Today))
            ModelState.AddModelError("Date", "当日以降のみ予約可能です");
        if (!ModelState.IsValid)
        {
            TempData["Toast"] = "入力内容を確認してください";
            return RedirectToAction(nameof(Index), new { date = m.Date, roomId = m.RoomId });
        }

        var overlap = await db.Reservations.AnyAsync(r => r.RoomId == m.RoomId && r.Date == m.Date &&
            !(m.EndTime <= r.StartTime || r.EndTime <= m.StartTime));
        if (overlap)
        {
            TempData["Toast"] = "重複する時間帯があるため予約できません";
            return RedirectToAction(nameof(Index), new { date = m.Date, roomId = m.RoomId });
        }

        db.Reservations.Add(m);
        await db.SaveChangesAsync();
        TempData["Toast"] = "予約を保存しました";
        return RedirectToAction(nameof(Index), new { date = m.Date, roomId = m.RoomId });
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id, string reservedBy)
    {
        var r = await db.Reservations.FindAsync(id);
        if (r is null) return NotFound();
        if (!string.Equals(r.ReservedBy, reservedBy, StringComparison.Ordinal))
        {
            TempData["Toast"] = "自分の予約のみ削除可能です";
            return RedirectToAction(nameof(Index), new { date = r.Date, roomId = r.RoomId });
        }
        db.Reservations.Remove(r);
        await db.SaveChangesAsync();
        TempData["Toast"] = "予約を削除しました";
        return RedirectToAction(nameof(Index), new { date = r.Date, roomId = r.RoomId });
    }
}


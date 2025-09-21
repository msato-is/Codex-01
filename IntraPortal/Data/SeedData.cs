using IntraPortal.Models;

namespace IntraPortal.Data;

public static class SeedData
{
    public static async Task RunAsync(PortalDbContext db)
    {
        if (!db.Employees.Any())
        {
            db.Employees.AddRange(
                new Employee { Name = "田中 太郎", Kana = "タナカ タロウ", Department = "情報システム", Ext = "1234", Email = "t.tanaka@example.com" },
                new Employee { Name = "佐藤 花子", Kana = "サトウ ハナコ", Department = "総務", Ext = "2345", Email = "h.sato@example.com" },
                new Employee { Name = "鈴木 次郎", Kana = "スズキ ジロウ", Department = "物流", Ext = "3456", Email = "j.suzuki@example.com" }
            );
        }

        if (!db.MeetingRooms.Any())
        {
            db.MeetingRooms.AddRange(
                new MeetingRoom { Name = "A会議室", Capacity = 8 },
                new MeetingRoom { Name = "B会議室", Capacity = 4 }
            );
        }

        if (!db.Items.Any())
        {
            db.Items.AddRange(
                new Item { Name = "ノートPC", Stock = 5, Location = "5F-倉庫", Manager = "情報システム" },
                new Item { Name = "マウス", Stock = 20, Location = "5F-倉庫", Manager = "情報システム" }
            );
        }

        var today = DateOnly.FromDateTime(DateTime.Now);
        if (!db.LunchMenus.Any(m => m.Date == today))
        {
            db.LunchMenus.AddRange(
                new LunchMenu { Date = today, Code = "A", Name = "唐揚げ弁当", Price = 600 },
                new LunchMenu { Date = today, Code = "B", Name = "生姜焼き", Price = 650 },
                new LunchMenu { Date = today, Code = "C", Name = "野菜カレー", Price = 550 }
            );
        }

        if (!db.Notices.Any())
        {
            db.Notices.Add(new Notice { Title = "明日の停電点検について", Body = "明日9:00-10:00に停電点検を実施します。ご協力ください。", Author = "総務", CreatedAt = DateTimeOffset.Now });
        }

        await db.SaveChangesAsync();
    }
}


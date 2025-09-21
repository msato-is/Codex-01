namespace IntraPortal.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Kana { get; set; }
    public string Department { get; set; } = string.Empty;
    public string? Ext { get; set; }
    public string? Email { get; set; }
}

public class MeetingRoom
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
}

public class Reservation
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public MeetingRoom? Room { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string? Purpose { get; set; }
    public string ReservedBy { get; set; } = string.Empty;
}

public class Item
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Stock { get; set; }
    public string? Location { get; set; }
    public string? Manager { get; set; }
}

public class ItemRequest
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public Item? Item { get; set; }
    public int Quantity { get; set; }
    public string? Purpose { get; set; }
    public string RequestedBy { get; set; } = string.Empty;
    public DateTimeOffset RequestedAt { get; set; }
}

public class LunchMenu
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public string Code { get; set; } = "A"; // A/B/C
    public string Name { get; set; } = string.Empty;
    public int Price { get; set; }
}

public class LunchOrder
{
    public int Id { get; set; }
    public int MenuId { get; set; }
    public LunchMenu? Menu { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTimeOffset OrderedAt { get; set; }
}

public class Notice
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public string? FileUrl { get; set; }
}

public class NoticeRead
{
    public int Id { get; set; }
    public int NoticeId { get; set; }
    public Notice? Notice { get; set; }
    public string Reader { get; set; } = string.Empty;
    public DateTimeOffset ReadAt { get; set; }
}


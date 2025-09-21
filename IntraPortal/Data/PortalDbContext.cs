using IntraPortal.Models;
using Microsoft.EntityFrameworkCore;

namespace IntraPortal.Data;

public class PortalDbContext(DbContextOptions<PortalDbContext> options) : DbContext(options)
{
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<ItemRequest> ItemRequests => Set<ItemRequest>();
    public DbSet<LunchMenu> LunchMenus => Set<LunchMenu>();
    public DbSet<LunchOrder> LunchOrders => Set<LunchOrder>();
    public DbSet<Notice> Notices => Set<Notice>();
    public DbSet<NoticeRead> NoticeReads => Set<NoticeRead>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Employee>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Kana).HasMaxLength(80);
            e.Property(x => x.Department).HasMaxLength(80).IsRequired();
            e.Property(x => x.Ext).HasMaxLength(10);
            e.Property(x => x.Email).HasMaxLength(120);
            e.HasIndex(x => new { x.Name, x.Department });
        });

        model.Entity<MeetingRoom>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(40).IsRequired();
        });

        model.Entity<Reservation>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Purpose).HasMaxLength(200);
            e.Property(x => x.ReservedBy).HasMaxLength(80).IsRequired();
            e.HasOne(x => x.Room).WithMany().HasForeignKey(x => x.RoomId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => new { x.RoomId, x.Date, x.StartTime, x.EndTime });
        });

        model.Entity<Item>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.Property(x => x.Location).HasMaxLength(80);
            e.Property(x => x.Manager).HasMaxLength(80);
        });

        model.Entity<ItemRequest>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Purpose).HasMaxLength(200);
            e.Property(x => x.RequestedBy).HasMaxLength(80).IsRequired();
            e.HasOne(x => x.Item).WithMany().HasForeignKey(x => x.ItemId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.RequestedAt);
        });

        model.Entity<LunchMenu>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Code).HasMaxLength(1).IsRequired();
            e.Property(x => x.Name).HasMaxLength(80).IsRequired();
            e.HasIndex(x => new { x.Date, x.Code }).IsUnique();
        });

        model.Entity<LunchOrder>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.EmployeeName).HasMaxLength(80).IsRequired();
            e.HasOne(x => x.Menu).WithMany().HasForeignKey(x => x.MenuId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.OrderedAt);
        });

        model.Entity<Notice>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Title).HasMaxLength(80).IsRequired();
            e.Property(x => x.Body).HasMaxLength(2000).IsRequired();
            e.Property(x => x.Author).HasMaxLength(80).IsRequired();
            e.Property(x => x.FileUrl).HasMaxLength(200);
            e.HasIndex(x => x.CreatedAt);
        });

        model.Entity<NoticeRead>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Reader).HasMaxLength(80).IsRequired();
            e.HasOne(x => x.Notice).WithMany().HasForeignKey(x => x.NoticeId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}


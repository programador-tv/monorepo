using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : DbContext(options)
{
    public DbSet<Perfil> Perfils { get; set; } = null!;
    public DbSet<Tag> Tags { get; set; } = null!;
    public DbSet<Live> Lives { get; set; } = null!;
    public DbSet<NotifyUserLiveEarly> NotifyUserLiveEarlies { get; set; } = null!;
    public DbSet<Visualization> Visualizations { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Like> Likes { get; set; } = null!;
    public DbSet<Follow> Follows { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;
    public DbSet<Presentes> Presentes { get; set; } = null!;
    public DbSet<PresentesOpenRoom> PresentesOpenRooms { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<FeedbackTimeSelection> FeedbackTimeSelections { get; set; } = null!;
    public DbSet<FeedbackJoinTime> FeedbackJoinTimes { get; set; } = null!;
    public DbSet<TimeSelection> TimeSelections { get; set; } = null!;
    public DbSet<JoinTime> JoinTimes { get; set; } = null!;
    public DbSet<FreeTimeBackstage> FreeTimeBackstages { get; set; } = null!;
    public DbSet<LiveBackstage> LiveBackstages { get; set; } = null!;
    public DbSet<HelpBackstage> HelpBackstages { get; set; } = null!;
    public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
    public DbSet<Log> Logs { get; set; } = null!;
}

using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<FeedbackJoinTime> FeedbackJoinTimes { get; set; }
        public DbSet<FeedbackTimeSelection> FeedbackTimeSelections { get; set; }
        public DbSet<JoinTime> JoinTimes { get; set; }
        public DbSet<TimeSelection> TimeSelections { get; set; }
        public DbSet<LiveBackstage> LiveBackstages { get; set; }
        public DbSet<FreeTimeBackstage> FreeTimeBackstages { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Presentes> Presentes { get; set; }
        public DbSet<PresentesOpenRoom> PresentesOpenRooms { get; set; }
        public DbSet<Live> Lives { get; set; }
        public DbSet<Visualization> Visualizations { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<NotifyUserLiveEarly> NotifyUserLiveEarlies { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<HelpBackstage> HelpBackstages { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<FeedbackTimeSelection>()
                .Property(f => f.EstimativaSalarioAvaliado)
                .HasColumnType("decimal(18,2)");
        }
    }
}

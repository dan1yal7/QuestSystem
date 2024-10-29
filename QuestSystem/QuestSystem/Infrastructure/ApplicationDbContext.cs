using Microsoft.EntityFrameworkCore;
using QuestSystem.Models;

namespace QuestSystem.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Player> players { get; set; }
        public DbSet<Quest> quests { get; set; }
        public DbSet<PlayerQuest> questsPlayers { get; set; }

    }
}

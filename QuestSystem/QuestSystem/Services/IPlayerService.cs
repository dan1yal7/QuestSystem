using Microsoft.EntityFrameworkCore;
using QuestSystem.Infrastructure;
using QuestSystem.Models;

namespace QuestSystem.Services
{
    public interface IPlayerService
    {
        #region Contract 
        Task<Player> GetPlayerByIdAsync(Guid playerId);
        Task<IEnumerable<PlayerQuest>> GetPlayerQuestsAsync(Guid playerId);
        Task AddPlayerQuestAsync(PlayerQuest playerQuest);
        Task SaveChangesAsync();
        #endregion

    }

    public class PlayerService : IPlayerService
    {
        #region Fields
        private readonly ApplicationDbContext _context;
        #endregion

        #region Ctor
        public PlayerService(ApplicationDbContext context)
        {
            context = _context;
        }
        #endregion


        #region Methods
        public async Task<Player> GetPlayerByIdAsync(Guid playerId)
        {
            return await _context.players.FindAsync(playerId);
        }

        public async Task<IEnumerable<PlayerQuest>> GetPlayerQuestsAsync(Guid playerId)
        { 
            return await _context.questsPlayers
                 .Where(qp => qp.PlayerId == playerId).ToListAsync();
        }

        public async Task AddPlayerQuestAsync(PlayerQuest playerQuest)
        {
           await _context.questsPlayers.AddAsync(playerQuest);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}

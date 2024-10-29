using Microsoft.EntityFrameworkCore;
using QuestSystem.Infrastructure;
using QuestSystem.Models;

namespace QuestSystem.Services
{
    public interface IQuestService
    {
        #region Contract
        Task<Quest> GetQuestByIdAsync(Guid questId);
        Task<IEnumerable<Quest>> GetAllAvailableQuestsAsync(Guid playerId, int playerLevel);
        Task<string> AcceptQuestAsync(Guid questId, Guid playerId);
        Task<object> UpdateQuestProgressAsync(Guid questId, Guid playerId, int progressIncrement); 
        Task<object> CompletedQuestAsync(Guid questId, Guid playerId);

        #endregion 
    }
    public class QuestService : IQuestService
    {
        #region Fields
        private readonly ApplicationDbContext _context;
        private readonly IPlayerService _playerService;
        #endregion

        #region Ctor
        public QuestService(ApplicationDbContext context, IPlayerService playerService)
        {
            _context = context;
            _playerService = playerService;
        }
        #endregion


        #region Methods
        public async Task<Quest> GetQuestByIdAsync(Guid questId)
        {
            return await _context.quests.FindAsync(questId);
        }

        public async Task<IEnumerable<Quest>> GetAllAvailableQuestsAsync(Guid playerId, int playerLevel)
        {
            return await _context.quests.Where(q => q.RequiredLevel <= playerLevel && !_context.questsPlayers
                 .Any(qp => qp.PlayerId == playerId && qp.QuestId == q.QuestId && qp.Status != QuestStatus.Finished)).ToListAsync();
        }
         
        public async Task<string> AcceptQuestAsync(Guid questId, Guid playerId)
        {
            var player = await _playerService.GetPlayerByIdAsync(playerId);
            var quest = await GetQuestByIdAsync(questId); 
            if(player == null || quest == null)
            {
                return "Player or quest not found";
            }
            var activeQuest = (await _playerService.GetPlayerQuestsAsync(playerId))
               .Count(qp => qp.Status == QuestStatus.Accepted || qp.Status == QuestStatus.InProgress);
            if(activeQuest >= 10)
            return "Player has reached the maximum number of active quests";
            var playerQuest = new PlayerQuest
            {
                PlayerQuestId = Guid.NewGuid(),
                PlayerId = playerId,
                QuestId = questId,
                Status = QuestStatus.Accepted,
                Progress = 0
            };
            await _playerService.AddPlayerQuestAsync(playerQuest);
            await _playerService.SaveChangesAsync();

            return "Quest accepted successfuly";
        }

        public async Task<object> UpdateQuestProgressAsync(Guid questId, Guid playerId, int progressIncrement)
        { 
            var playerQuest = (await _playerService.GetPlayerQuestsAsync(playerId))
                .FirstOrDefault(qp => qp.QuestId == questId);
            if(playerQuest == null)
            {
                return "quest not found :("; 
            }
            if(playerQuest.Status == QuestStatus.Completed || playerQuest.Status == QuestStatus.Finished)
            return new {Progress = playerQuest.Progress, Status = playerQuest.Status};
            playerQuest.Progress += progressIncrement;
            if(playerQuest.Progress >= 100)
            {
                playerQuest.Status = QuestStatus.Completed;

            }
            else if(playerQuest.Status == QuestStatus.Accepted)
                playerQuest.Status = QuestStatus.InProgress;
            await _playerService.SaveChangesAsync();
            return new {Progress = playerQuest.Progress, Starus = playerQuest.Status};
        }

        public async Task<object> CompletedQuestAsync(Guid questId, Guid playerId)
        {
            var playerQuest = (await _playerService.GetPlayerQuestsAsync(playerId))
                .FirstOrDefault(qp => qp.QuestId == questId);
            if(playerQuest == null)
            {
                return new { Message = "Player Quest not found" };
            }
            if(playerQuest.Status != QuestStatus.Finished)
            {
                return new { Message = "Quest is fully completed" }; 
            }
            if(playerQuest.Status != QuestStatus.Completed)
            {
                return new { Message = "Quest conditions or rules are not fully met, please finish them and try again" };
            }
            playerQuest.Status = QuestStatus.Finished;
            await _playerService.SaveChangesAsync();
            var quest = await GetQuestByIdAsync(questId);
            return new { Message = "Quest completed successfully. Reward received.", Reward = quest.Reward };
        }
        #endregion
    }
}

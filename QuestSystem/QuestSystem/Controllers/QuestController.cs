using Microsoft.AspNetCore.Mvc;
using QuestSystem.Services;

namespace QuestSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestController : Controller
    {
        #region Fields
        private readonly IPlayerService _playerService;
        private readonly IQuestService _questService;
        private readonly ILogger _logger;
        #endregion

        #region Ctor
        public QuestController(IPlayerService playerService, IQuestService questService, ILogger logger)
        {
            _playerService = playerService;
            _questService = questService;
            _logger = logger;
        }
        #endregion

        #region Methods 
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("available/{playerId}")]
        public async Task<IActionResult> GetAllAvailableQuestsAsync(Guid playerId, int playerLevel)
        { 
           return await TryExecuteAsync(() => FetchAllAvailableQuestsAsync(playerId, playerLevel));
        }
        private async Task<IActionResult> FetchAllAvailableQuestsAsync(Guid playerId, int playerLevel)
        {
            var availableQuests = await _questService.GetAllAvailableQuestsAsync(playerId, playerLevel);
            return Ok(availableQuests);
        } 
        private async Task<T> TryExecuteAsync<T>(Func<Task<T>> func)
        {
            try
            {
                return await func();
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return default;
            }
        }



        #endregion
    }
}

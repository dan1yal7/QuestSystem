namespace QuestSystem.Models
{
    public class PlayerQuest
    {
        public Guid PlayerQuestId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid QuestId { get; set; }
        public QuestStatus Status { get; set; }
        public int Progress { get; set; }
    }
    public enum QuestStatus
    {
        Accepted,
        InProgress,
        Completed,
        Finished
    }
}

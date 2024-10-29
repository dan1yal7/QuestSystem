namespace QuestSystem.Models
{
    public class Quest
    {
        public Guid QuestId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int RequiredLevel { get; set; }
        public string Reward { get; set; }
    }
}

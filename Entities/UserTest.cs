namespace EmotionalIntelligence.Entities
{
    public class UserTest:BaseEntity
    {
        public int UserId { get; set; }
        public int TestId { get; set; }
        public bool IsFinished { get; set; }
        public AppUser User { get; set; }
        public Question Question { get; set; }
    }
}

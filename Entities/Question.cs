namespace EmotionalIntelligence.Entities
{
    public class Question: BaseEntity
    {
        public string Description { get; set; }
        public int Order { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
    }
}

namespace EmotionalIntelligence.Entities
{
    public class Answer: BaseEntity
    {
        public string Description { get; set; }
        public int Value { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
    }
}

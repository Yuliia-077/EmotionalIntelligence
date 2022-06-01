namespace EmotionalIntelligence.Entities
{
    public class ScaleForResult: BaseEntity
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public int TestId { get; set; }
        public Test Test { get; set; }
    }
}

namespace EmotionalIntelligence.Entities
{
    public class ScaleQuestion:BaseEntity
    {
        public int ScaleForResultId { get; set; }
        public int QuestionId { get; set; }
        public ScaleForResult ScaleForResult { get; set; }
        public Question Question { get; set; }
    }
}

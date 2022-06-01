using EmotionalIntelligence.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmotionalIntelligence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<ScaleForResult> SvaleForResults { get; set; }
        public DbSet<ScaleQuestion> ScaleQuestions { get; set; }
        public DbSet<UserTest> UserTests { get; set; }
        public DbSet<UserTestAnswer> UserTestAnswers { get; set; }
    }
}

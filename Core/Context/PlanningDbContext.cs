using Microsoft.EntityFrameworkCore;
using Core.Entities;

namespace Core.Context
{
    public class PlanningDbContext : DbContext
    {

        public PlanningDbContext(DbContextOptions<PlanningDbContext> options)
           : base(options)
        {
        }

        public DbSet<Plan> Plans { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Dictionary> Dictionaries { get; set; }

        public DbSet<File> Files { get; set; }

        public DbSet<StepResponsible> StepResponsibles { get; set; }

        /// <summary>
        /// Step structure tables
        /// </summary>

        public DbSet<StepBlock> StepBlocks { get; set; }

        public DbSet<Introduction> Introductions { get; set; }

        public DbSet<Question> Questions { get; set; }

        public DbSet<Option> Options { get; set; }


        /// <summary>
        /// Answers' tables
        /// </summary>

        public DbSet<StepTask> StepTasks { get; set; }

        public DbSet<UserStepResult> UserStepResults { get; set; }

        public DbSet<StepTaskAnswer> StepTaskAnswers { get; set; }

        public DbSet<BooleanAnswer> BooleanAnswers { get; set; }

        public DbSet<IssueOptionAnswer> IssueOptionAnswers { get; set; }

        public DbSet<SelectAnswer> SelectAnswers { get; set; }

        public DbSet<PreparingAnswer> PreparingAnswers { get; set; }

        public DbSet<TextAnswer> TextAnswers { get; set; }

        public DbSet<StrategicIssueAnswer> StrategicIssueAnswers { get; set; }

        public DbSet<StakeholderRatingAnswer> StakeholderRatingAnswers { get; set; }

        public DbSet<FileAnswer> FileAnswers { get; set; }

        public DbSet<ValueAnswer> ValueAnswers { get; set; }

        public DbSet<StakeholderAnswer> StakeholderAnswers { get; set; }

        public DbSet<SWOTAnswer> SWOTAnswers { get; set; }

        /// <summary>
        /// Relations' tables
        /// </summary>

        public DbSet<IssueOptionAnswerToResource> IssueOptionAnswersToResources { get; set; }

        public DbSet<StakeholderRatingAnswerToDictionary> StakeholderRatingAnswersToDictionaries { get; set; }

        public DbSet<UserToPlan> UsersToPlans { get; set; }

        public DbSet<UserToIssueOptionAnswer> UsersToIssueOptionAnswers { get; set; }

        /// <summary>
        /// Settings
        /// </summary>

        public DbSet<Setting> Settings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlanningDbContext).Assembly);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder contextOptionsBuilder)
        {
            contextOptionsBuilder.EnableSensitiveDataLogging(true);
        }

    }
}

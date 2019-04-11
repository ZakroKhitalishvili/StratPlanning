
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Core.Configurations;

namespace Core.Context
{
    class PlanningDbContext : DbContext
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

        public DbSet<BooleanAnswer> BooleanAnswers { get; set; }

        public DbSet<IssueOptionAnswer> IssueOptionAnswers { get; set; }

        public DbSet<SelectAnswer> SelectAnswers { get; set; }

        public DbSet<PreparingAnswer> PreparingAnswers { get; set; }

        public DbSet<StepAnswer> StepAnswers { get; set; }

        public DbSet<TextAnswer> TextAnswers { get; set; }

        public DbSet<StrategicIssueAnswer> StrategicIssueAnswers { get; set; }

        public DbSet<StakeholderRatingAnswer> StakeholderRatingAnswers { get; set; }

        /// <summary>
        /// Relations' tables
        /// </summary>
        
        public DbSet<IssueOptionAnswerToResource> IssueOptionAnswersToResources { get; set; }

        public DbSet<StakeholderRatingAnswerToDictionary> StakeholderRatingAnswersToDictionaries { get; set; }

        public DbSet<UserToPlan> UsersToPlans { get; set; }

        public DbSet<UserToIssueOptionAnswer> UsersToIssueOptionAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PlanningDbContext).Assembly);
        }




    }
}

using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using PIP.Domain.Companion;
using PIP.Domain.Deelplatform;
using Pip.Domain.Flow;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using PIP.Domain.User;
using PIP.Domain.WebApplication;

namespace PIP.DAL.EF;

public class PhygitalDbContext : IdentityDbContext<IdentityUser>
{
    public PhygitalDbContext(DbContextOptions<PhygitalDbContext> options) : base(options)
    {
    }

    public DbSet<FlowSession> Sessions { get; set; }
    public DbSet<CirculaireFlowStrategy> CirculaireFlowStrategies{ get; set; }
    public DbSet<Note> Notes { get; set; }
    public DbSet<Subplatform> Subplatforms { get; set; }
    public DbSet<Installation> Installations { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<WebappUser> WebappUsers { get; set; }
    public DbSet<Idea> Ideas { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    public DbSet<Flow> Flows { get; set; }
    public DbSet<FlowStep> FlowSteps { get; set; }

    public DbSet<Info> Infos { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<MultipleChoiceQuestion> MultipleChoiceQuestions { get; set; }
    public DbSet<ClosedQuestion> ClosedQuestions { get; set; }

    public DbSet<OpenQuestion> OpenQuestions { get; set; }
    public DbSet<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; }
    public DbSet<ClosedAnswer> ClosedAnswers { get; set; }
    public DbSet<OpenAnswer> OpenAnswers { get; set; }

    public DbSet<Theme> Themes { get; set; }
    public DbSet<Participant> Participants { get; set; }
    public DbSet<Response> Responses { get; set; }
    public DbSet<RangeQuestion> RangeQuestions { get; set; }
    public DbSet<FlowSession> FlowSessions { get; set; }

    public DbSet<ConditionalPoint> ConditionalPoints { get; set; }
    public DbSet<Subtheme> Subthemes { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<SubPlatformAdministrator> Deelplatformadministrators { get; set; }
    public DbSet<Companion> Companions { get; set; }

    protected override async void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string secretIdSql = "connectionstring";
        
        
        SecretManager secretManager = new SecretManager();

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlite("Data Source=PhygitalDbContext.db");
        }
        else
        {
            string connectionString = await secretManager.GetSecretAsync(secretIdSql);
            optionsBuilder.UseNpgsql(connectionString);
        }

        // configure logging: write to debug output window
        optionsBuilder.LogTo(message => Debug.WriteLine(message), LogLevel.Information);

        // configure lazy-loading
        optionsBuilder.UseLazyLoadingProxies(false);
    }

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<WebappUser>().HasOne(w => w.Flow);
    modelBuilder.Entity<FlowStep>()
        .HasDiscriminator<string>("flow_step_type")
        .HasValue<FlowStep>("flow_step_base")
        .HasValue<Question>("flow_step_question")
        .HasValue<Info>("flow_step_info");

    modelBuilder.Entity<Question>()
        .HasDiscriminator<string>("question_type")
        .HasValue<Question>("question_base")
        .HasValue<MultipleChoiceQuestion>("question_multiple_choice")
        .HasValue<ClosedQuestion>("question_closed")
        .HasValue<OpenQuestion>("question_open")
        .HasValue<RangeQuestion>("question_range");

    modelBuilder.Entity<Answer>()
        .HasDiscriminator<string>("answer_type")
        .HasValue<Answer>("answer_base")
        .HasValue<MultipleChoiceAnswer>("answer_multiple_choice")
        .HasValue<ClosedAnswer>("answer_closed")
        .HasValue<OpenAnswer>("answer_open")
        .HasValue<RangeAnswer>("answer_range");

    modelBuilder.Entity<ConditionalPoint>()
        .HasOne(cp => cp.Criteria)
        .WithOne(a => a.ConditionalPoint)
        .HasForeignKey<Answer>(a => a.ConditionalPointId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<ConditionalPoint>()
        .HasOne(cp => cp.Question)
        .WithMany(a => a.QuestionConditionalPoints)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade); // Added cascade delete

    modelBuilder.Entity<ConditionalPoint>()
        .HasOne(cp => cp.FollowUpStep)
        .WithOne(a => a.ConditionalPoint)
        .HasForeignKey<FlowStep>(a => a.ConditionalPointId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade); // Added cascade delete

    modelBuilder.Entity<Response>()
        .HasOne(r => r.Answer)
        .WithOne(a => a.Response)
        .HasForeignKey<Response>(r => r.AnswerId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade); // Added cascade delete

    modelBuilder.Entity<Answer>()
        .HasOne(a => a.Response)
        .WithOne(r => r.Answer)
        .HasForeignKey<Answer>(a => a.ResponseId)
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade); // Added cascade delete

    modelBuilder.Entity<Response>()
        .HasOne(r => r.FlowSession)
        .WithMany(fs => fs.Responses)
        .HasForeignKey(r => r.FlowSessionId)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Project>()
        .HasMany(p => p.Flows)
        .WithOne(f => f.Project)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<CirculaireFlowStrategy>()
        .HasMany(p => p.Flows)
        .WithOne()
        .OnDelete(DeleteBehavior.Cascade); 
    modelBuilder.Entity<CirculaireFlowStrategy>()
        .HasMany(p => p.FlowSessions)
        .WithOne(fs=> fs.CirculaireFlows)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Idea>()
        .HasMany(i => i.reactions)
        .WithOne(r => r.Idea)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Theme>()
        .HasMany(t => t.SubThemes)
        .WithOne(t => t.ParentTheme)
        .HasForeignKey("ParentThemeId")
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Theme>()
        .HasOne(t => t.Flow)
        .WithMany()
        .HasForeignKey("ParentThemeId")
        .IsRequired(false)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<FlowSession>()
        .HasOne(s => s.Flow)
        .WithMany(f => f.FlowSessions)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Flow>()
        .HasMany(f => f.FlowSessions)
        .WithOne(s => s.Flow)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Note>()
        .HasOne(n => n.FlowSession)
        .WithMany(fs => fs.Notes)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<FlowSession>()
        .HasMany(fs => fs.Notes)
        .WithOne(n => n.FlowSession)
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Flow>()
        .HasKey(f => f.FlowId);
    modelBuilder.Entity<Theme>()
        .HasKey(t => t.ThemeId);
    modelBuilder.Entity<Subplatform>()
        .HasKey(s => s.SubplatformId);
    modelBuilder.Entity<Project>()
        .HasKey(p => p.ProjectId);
    modelBuilder.Entity<FlowStep>()
        .HasKey(f => f.FlowStepId);
    modelBuilder.Entity<Response>()
        .HasKey(r => r.ResponseId);
    modelBuilder.Entity<Answer>()
        .HasKey(a => a.AnswerId);
    modelBuilder.Entity<ConditionalPoint>()
        .HasKey(cp => cp.ConditionalPointId);

    modelBuilder.Entity<Theme>().ToTable("Themes");
    modelBuilder.Entity<Subplatform>().ToTable("Subplatforms");
    modelBuilder.Entity<Installation>().ToTable("Installations");
    modelBuilder.Entity<Project>().ToTable("Projects");
    modelBuilder.Entity<Flow>().ToTable("Flows");
    modelBuilder.Entity<FlowStep>().ToTable("Flowsteps");
    modelBuilder.Entity<Response>().ToTable("Responses");
    modelBuilder.Entity<Answer>().ToTable("Answers");
    modelBuilder.Entity<FlowSession>().ToTable("FlowSessions");
    modelBuilder.Entity<Subtheme>().ToTable("Subthemes");
    modelBuilder.Entity<ConditionalPoint>().ToTable("ConditionalPoints");
    modelBuilder.Entity<CirculaireFlowStrategy>().ToTable("CirculaireFlowStrategy");
    modelBuilder.Entity<Note>().ToTable("Notes");
}



    public bool CreateDatabase(bool dropDatabase)
    {
        if (dropDatabase)
        {
            try
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
            }
            catch (PostgresException ex) when (ex.SqlState == "42501")
            {
                // If we're not a superuser, just clear out the existing data instead of dropping the database
                foreach (var table in Model.GetEntityTypes())
                {
                    Database.ExecuteSqlRaw($"TRUNCATE TABLE \"{table.GetTableName()}\" CASCADE");
                }
            }
        }
        else
        {
            Database.EnsureCreated();
        }

        return true;
    }
}
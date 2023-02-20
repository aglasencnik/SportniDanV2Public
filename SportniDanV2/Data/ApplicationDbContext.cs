using Microsoft.EntityFrameworkCore;
using SportniDanV2.Enums;
using SportniDanV2.Models;

namespace SportniDanV2.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    public DbSet<ClassModel> Class { get; set; } = null!;
    public DbSet<UserModel> User { get; set; } = null!;
    public DbSet<ExerciseModel> Exercise { get; set; } = null!;
    public DbSet<UserExerciseModel> UserExercise { get; set; } = null!;
    public DbSet<AppDataModel> AppData { get; set; } = null!;
    public DbSet<GameResultModel> GameResult { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserModel>()
            .HasData
            (
                new UserModel { Id = 1, Username = "admin", Password = "nzX528vH#7", UserType = UserType.Admin },
                new UserModel { Id = 2, Username = "organizator", Password = "7#Ff5s6$32", UserType = UserType.Admin }
            );
    }
}

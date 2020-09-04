using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LagosSmartMeter
{
public class DatabaseContext : IdentityDbContext<SubscriberModel>
{
    public DbSet<MeterModel> Meters { get; set; }
    public DbSet<MeterLatestReadings> MeterReadings { get; set; }
    public DbSet<MeterMonthlyOnHours> MeterMonthlyOnHours { get; set; }
    public DbSet<MeterTariff> MeterTariffs { get; set; }
    public DbSet<MeterTimeRecords> MeterTimeRecords { get; set;}
    public DbSet<MeterType> MeterTypes { get; set; }
    public DbSet<MeterLocation> MeterLocations { get; set; }
    public DbSet<SubscriberAddress> SubscriberAddresses { get; set; }
    public DbSet<SubscriberMetersModel> SubscriberMeters { get; set; }
    public DbSet<TransactionModel> Transactions { get; set; }
    public DbSet<MeterNotification> MeterNotifications { get; set; }
    public DbSet<MeterAlert> MeterAlerts { get; set; }
    public DbSet<SubscriberNotification> SubscriberNotifications { get; set; }
    public DbSet<FireAndForgetMeterJob> FireAndForgetMeterJobs { get; set; }
    public DbSet<MeterToken> MeterTokens { get; set; }

    public DatabaseContext(DbContextOptions<DatabaseContext> options):base(options){}
    public DatabaseContext(){}

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
        builder.EnableDetailedErrors(true)
                .EnableSensitiveDataLogging(true)
                .UseSqlServer("Data Source=tcp:lagossmartmetersqlserver.database.windows.net,1433;Initial Catalog=LagosSmartMeterFinalBuild;User ID=databaseadmin;Password=Exploit90");
        base.OnConfiguring(builder);
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        // run default method
        base.OnModelCreating(builder);
        #region Property Settings
        builder.Entity<MeterModel>()
            .Property(m => m.MeterID)
            .IsRequired()
            .ValueGeneratedNever();
        
       builder.Entity<MeterTimeRecords>()
            .Property(m => m.MeterID)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Entity<MeterLatestReadings>()
            .Property(m => m.MeterID)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Entity<MeterMonthlyOnHours>()
            .Property(m => m.MeterID)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Entity<MeterTariff>()
            .Property(t => t.TarrifID)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Entity<MeterType>()
            .Property(t => t.ID)
            .IsRequired()
            .ValueGeneratedNever();

        builder.Entity<MeterLocation>()
            .Property(p => p.MeterID)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Entity<SubscriberModel>()
            .Property(s => s.Id)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Entity<SubscriberAddress>()
            .Property(p => p.SubscriberID)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Entity<SubscriberMetersModel>()
            .Property(p => p.MeterID)
            .IsRequired()
            .ValueGeneratedNever();
        
        builder.Entity<TransactionModel>()
            .Property(p => p.TransactionID)
            .IsRequired()
            .ValueGeneratedNever();

        #endregion
        
        #region Relationships
        // one-many relationship between meter and meter readings
        builder.Entity<MeterModel>()
            .HasMany<MeterLatestReadings>()
            .WithOne()
            .HasForeignKey(m => m.MeterID)
            .OnDelete(DeleteBehavior.Cascade);
        
        // one-many relationship between meter and meter time records
        builder.Entity<MeterModel>()
            .HasMany<MeterTimeRecords>()
            .WithOne()
            .HasForeignKey(m => m.MeterID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<MeterModel>()
            .HasMany<MeterLocation>()
            .WithOne()
            .HasForeignKey(m => m.MeterID)
            .OnDelete(DeleteBehavior.Cascade);
        
        // one-many relationship between subscriber and address
        builder.Entity<SubscriberModel>()
            .HasMany<SubscriberAddress>()
            .WithOne()
            .HasForeignKey(s => s.SubscriberID)
            .OnDelete(DeleteBehavior.Cascade);
            

        // many-one relationship between tariff and meter
       // builder.Entity<MeterModel>()
          //  .HasOne<MeterTariff>();
        
        // many-one relationship between meter-type and meter
        //builder.Entity<MeterModel>()
         //   .HasOne<MeterType>();
        #endregion
    }
}
    public class DatabaseDesignFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        DatabaseContext IDesignTimeDbContextFactory<DatabaseContext>.CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<DatabaseContext>();
            builder.UseSqlServer("Data Source=tcp:lagossmartmetersqlserver.database.windows.net,1433;Initial Catalog=LagosSmartMeterFinalBuild;User ID=databaseadmin;Password=Exploit90");
            return new DatabaseContext(builder.Options);
        }
    }
}
using Microsoft.EntityFrameworkCore;
using StreamScraperTest.Database.Models;

namespace StreamScraperTest.Database;

public class StreamScraperContext: DbContext
{
    private const string ConnectionString = "Server=localhost;User=alex;Password=dbtest;Database=StreamScraper";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
    }
    
    //Gateways to the tables
    public DbSet<Contents> content { get; set; }
    public DbSet<Contentinformations> contentinformation { get; set; }
    public DbSet<Contenttypes> contenttype { get; set; }
    public DbSet<Countries> country { get; set; }
    public DbSet<Genres> genre { get; set; }
    
    //Database migration
    protected override void OnModelCreating(ModelBuilder builder)
    {
        //Soll die many to many Relation angeblich automatisch erkennen
        
        builder.Entity<Contents>().ToTable("Contents").HasKey(table => table.ContentId);
        builder.Entity<Contents>().Property(content => content.Name).IsRequired();
        builder.Entity<Contents>().HasOne<Contentinformations>(elem => elem.Contentinformation)
            .WithOne(coninf => coninf.Content).HasForeignKey<Contentinformations>(coninf => coninf.ContentId);

        builder.Entity<Contentinformations>().ToTable("Contentinformations")
            .HasKey(coninf => coninf.ContentinformationId);
        builder.Entity<Contentinformations>().Property(coninf => coninf.AvailableNetflix).IsRequired();
        builder.Entity<Contentinformations>().Property(coninf => coninf.ContentId).IsRequired();
        builder.Entity<Contentinformations>().Property(coninf => coninf.Fullname);
        builder.Entity<Contentinformations>().HasOne<Contenttypes>(coninf => coninf.Contenttype)
            .WithMany(contype => contype.Contentinformations).HasForeignKey(coninf => coninf.ContenttypeId);

        builder.Entity<Contenttypes>().ToTable("Contenttypes").HasKey(contentype => contentype.ContenttypeId);
        builder.Entity<Contenttypes>().Property(contenttype => contenttype.ContenttypeDescription)
            .HasColumnType("char(10)");
        
        builder.Entity<Countries>().ToTable("Countries").HasKey(country => country.CountryId);
        builder.Entity<Countries>().Property(country => country.CountryName).HasColumnType("char(20)");

        builder.Entity<Genres>().ToTable("Genres").HasKey(genre => genre.GenreId);
        builder.Entity<Genres>().Property(genre => genre.GenreName).HasColumnType("char(30)");


    }

}
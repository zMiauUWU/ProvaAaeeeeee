using Microsoft.EntityFrameworkCore;
using API;
public class AppDataContext : DbContext
{
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<IMC> IMCs { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=RafaelZacharkim.db");
    }
}
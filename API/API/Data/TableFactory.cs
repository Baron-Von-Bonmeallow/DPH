using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using API.Data;

public class TableFactory : IDesignTimeDbContextFactory<DataContext>
{
    public DataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
        optionsBuilder.UseSqlServer("Server=localhost;Database=DPH;Trusted_Connection=True;TrustServerCertificate=True;");

        return new DataContext(optionsBuilder.Options);
    }
}
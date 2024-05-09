using Microsoft.EntityFrameworkCore;
using VerificationProvider.Data.Entities;

namespace VerificationProvider.Data.Context;
public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{

    public DbSet<VerificationRequestEntity> VerificationRequests { get; set; }
}

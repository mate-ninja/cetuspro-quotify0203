using cetuspro0203.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace cetuspro0203.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Cytaty> Cytaty { get; set; }
        public DbSet<LoginRequest> User { get; set; }
    }
}

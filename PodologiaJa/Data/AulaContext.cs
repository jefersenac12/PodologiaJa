using PodologiaJa.Models;
using Microsoft.EntityFrameworkCore;

namespace PodologiaJa.Data
{
    public class AulaContext : DbContext
    {
        public AulaContext(DbContextOptions<AulaContext> options) : base(options)
        {
        }
        public DbSet<PodologiaJa.Models.Cliente> Clientes { get; set; } = default!;
    }
}

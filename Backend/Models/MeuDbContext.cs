using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class MeuDbContext : DbContext
    {
        public MeuDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        public DbSet<Usuarios> Usuarios { get; set; }

        public DbSet<Dependentes> Dependentes { get; set; }

        public DbSet<Cadastradores> Cadastradores { get; set; }

        public DbSet<Permissoes> Permissoes { get; set; }
    }
}
